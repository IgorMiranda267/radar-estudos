"use client";

import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import { CheckCircle2, Clock3, Pause, Play, Save, TimerReset } from "lucide-react";
import { AppShell } from "@/components/app-shell";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { EmptyState, ErrorState, LoadingState } from "@/components/ui/async-state";
import { useStudyUser } from "@/components/study-user-provider";
import { apiRequest, formatApiError, meApiPath, type Assunto, type Disciplina, type SessaoEstudo } from "@/lib/api";

function localDateValue() {
  const now = new Date();
  return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, "0")}-${String(now.getDate()).padStart(2, "0")}`;
}

function formatClock(totalSeconds: number) {
  const hours = Math.floor(totalSeconds / 3600);
  const minutes = Math.floor((totalSeconds % 3600) / 60);
  const seconds = totalSeconds % 60;
  return [hours, minutes, seconds].map((value) => String(value).padStart(2, "0")).join(":");
}

export default function SessaoPage() {
  const { user } = useStudyUser();
  const [disciplinas, setDisciplinas] = useState<Disciplina[]>([]);
  const [assuntos, setAssuntos] = useState<Assunto[]>([]);
  const [disciplinaId, setDisciplinaId] = useState("");
  const [assuntoId, setAssuntoId] = useState("");
  const [data, setData] = useState(localDateValue());
  const [minutos, setMinutos] = useState("25");
  const [questoes, setQuestoes] = useState("0");
  const [acertos, setAcertos] = useState("0");
  const [erros, setErros] = useState("0");
  const [banca, setBanca] = useState("");
  const [observacoes, setObservacoes] = useState("");
  const [seconds, setSeconds] = useState(0);
  const [running, setRunning] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [busy, setBusy] = useState(false);
  const [loaded, setLoaded] = useState(false);

  const loadDisciplines = useCallback(async () => {
    if (!user) return;
    setError("");
    try {
      const items = await apiRequest<Disciplina[]>(meApiPath("/disciplinas"));
      setDisciplinas(items.filter((item) => item.ativa));
      setDisciplinaId((current) => current && items.some((item) => item.id === current && item.ativa)
        ? current
        : items.find((item) => item.ativa)?.id ?? "");
    } catch (requestError) {
      setError(formatApiError(requestError));
    } finally {
      setLoaded(true);
    }
  }, [user]);

  useEffect(() => { void loadDisciplines(); }, [loadDisciplines]);

  useEffect(() => {
    if (!running) return;
    const timer = window.setInterval(() => setSeconds((value) => value + 1), 1000);
    return () => window.clearInterval(timer);
  }, [running]);

  useEffect(() => {
    if (!user || !disciplinaId) {
      setAssuntos([]);
      setAssuntoId("");
      return;
    }
    let active = true;
    void apiRequest<Assunto[]>(meApiPath(`/disciplinas/${disciplinaId}/assuntos`))
      .then((items) => {
        if (active) setAssuntos(items);
      })
      .catch((requestError: unknown) => {
        if (active) setError(formatApiError(requestError));
      });
    return () => { active = false; };
  }, [user, disciplinaId]);

  const accuracy = Number(questoes) > 0
    ? Math.round((Number(acertos) / Number(questoes)) * 100)
    : 0;
  const selectedSubject = useMemo(() => assuntos.find((item) => item.id === assuntoId), [assuntos, assuntoId]);

  async function saveSession(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!user || !disciplinaId) return;
    setBusy(true);
    setError("");
    setSuccess("");

    const duration = seconds > 0 ? Math.max(1, Math.ceil(seconds / 60)) : Number(minutos);
    try {
      const saved = await apiRequest<SessaoEstudo>(
        meApiPath(`/disciplinas/${disciplinaId}/sessoes-estudo`),
        {
          method: "POST",
          body: JSON.stringify({
            data,
            tempoEstudadoMinutos: duration,
            questoes: Number(questoes),
            acertos: Number(acertos),
            erros: Number(erros),
            assuntoId: assuntoId || null,
            banca: banca || null,
            observacoes: observacoes || null,
          }),
        },
      );
      setSuccess(`Sessão salva com ${saved.tempoEstudadoMinutos} minutos de estudo.`);
      setRunning(false);
      setSeconds(0);
      setMinutos("25");
      setQuestoes("0");
      setAcertos("0");
      setErros("0");
      setObservacoes("");
    } catch (requestError) {
      setError(formatApiError(requestError));
    } finally {
      setBusy(false);
    }
  }

  return (
    <AppShell title="Sessão de Estudo" subtitle="Use o cronômetro ou informe a duração e registre o resultado da sessão.">
      {error ? <ErrorState message={error} onRetry={() => void loadDisciplines()} /> : null}
      {success ? <p role="status" className="rounded-2xl bg-emerald-50 p-4 text-sm text-emerald-800">{success}</p> : null}
      {!loaded ? <LoadingState label="Carregando disciplinas..." /> : null}
      {loaded && disciplinas.length === 0 ? <EmptyState message="Cadastre uma disciplina antes de iniciar uma sessão." /> : null}
      {disciplinas.length > 0 ? (
        <form onSubmit={saveSession} className="grid gap-6 lg:grid-cols-[1.2fr_0.8fr]">
          <Card className="border-slate-200">
            <CardHeader>
              <CardTitle>Registrar sessão</CardTitle>
              <CardDescription>As sessões salvas aparecem no dashboard, nas estatísticas e no histórico.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-5">
              <div className="grid gap-4 md:grid-cols-2">
                <label className="text-sm font-medium text-slate-700">Disciplina
                  <select required value={disciplinaId} onChange={(event) => { setDisciplinaId(event.target.value); setAssuntoId(""); }} className="mt-2 w-full rounded-2xl border border-slate-200 bg-white px-4 py-3">
                    {disciplinas.map((disciplina) => <option key={disciplina.id} value={disciplina.id}>{disciplina.nome}</option>)}
                  </select>
                </label>
                <label className="text-sm font-medium text-slate-700">Assunto (opcional)
                  <select value={assuntoId} onChange={(event) => setAssuntoId(event.target.value)} className="mt-2 w-full rounded-2xl border border-slate-200 bg-white px-4 py-3">
                    <option value="">Sessão geral</option>
                    {assuntos.map((assunto) => <option key={assunto.id} value={assunto.id}>{assunto.nome}</option>)}
                  </select>
                </label>
                <label className="text-sm font-medium text-slate-700">Data
                  <input required type="date" value={data} onChange={(event) => setData(event.target.value)} className="mt-2 w-full rounded-2xl border border-slate-200 px-4 py-3" />
                </label>
                <label className="text-sm font-medium text-slate-700">Duração (minutos)
                  <input required type="number" min={0} value={minutos} onChange={(event) => setMinutos(event.target.value)} className="mt-2 w-full rounded-2xl border border-slate-200 px-4 py-3" />
                </label>
              </div>

              <div className="rounded-3xl border border-slate-200 p-5">
                <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
                  <div className="flex items-center gap-3">
                    <div className="flex h-12 w-12 items-center justify-center rounded-2xl bg-sky-100 text-sky-700"><Clock3 className="h-5 w-5" /></div>
                    <div>
                      <p className="text-sm text-slate-500">{running ? "Cronômetro em andamento" : "Cronômetro"}</p>
                      <p className="font-mono text-2xl font-semibold text-slate-950">{formatClock(seconds)}</p>
                    </div>
                  </div>
                  <div className="flex gap-2">
                    <button type="button" onClick={() => setRunning((value) => !value)} className="inline-flex items-center gap-2 rounded-2xl bg-sky-600 px-4 py-2 text-sm font-medium text-white">
                      {running ? <Pause className="h-4 w-4" /> : <Play className="h-4 w-4" />}{running ? "Pausar" : "Iniciar"}
                    </button>
                    <button type="button" onClick={() => { setRunning(false); setSeconds(0); }} className="inline-flex items-center gap-2 rounded-2xl border border-slate-200 px-4 py-2 text-sm">
                      <TimerReset className="h-4 w-4" />Zerar
                    </button>
                  </div>
                </div>
                {seconds > 0 ? <p className="mt-3 text-xs text-slate-500">Ao salvar, o cronômetro será arredondado para cima em minutos e substituirá a duração informada.</p> : null}
              </div>

              <div className="grid gap-4 md:grid-cols-3">
                <label className="text-sm font-medium text-slate-700">Questões
                  <input type="number" min={0} value={questoes} onChange={(event) => setQuestoes(event.target.value)} className="mt-2 w-full rounded-2xl border border-slate-200 px-4 py-3" />
                </label>
                <label className="text-sm font-medium text-slate-700">Acertos
                  <input type="number" min={0} max={Number(questoes)} value={acertos} onChange={(event) => setAcertos(event.target.value)} className="mt-2 w-full rounded-2xl border border-slate-200 px-4 py-3" />
                </label>
                <label className="text-sm font-medium text-slate-700">Erros
                  <input type="number" min={0} max={Number(questoes)} value={erros} onChange={(event) => setErros(event.target.value)} className="mt-2 w-full rounded-2xl border border-slate-200 px-4 py-3" />
                </label>
              </div>
              <div className="grid gap-4 md:grid-cols-2">
                <label className="text-sm font-medium text-slate-700">Banca (opcional)
                  <input maxLength={100} value={banca} onChange={(event) => setBanca(event.target.value)} className="mt-2 w-full rounded-2xl border border-slate-200 px-4 py-3" />
                </label>
                <label className="text-sm font-medium text-slate-700">Observações
                  <input maxLength={4000} value={observacoes} onChange={(event) => setObservacoes(event.target.value)} className="mt-2 w-full rounded-2xl border border-slate-200 px-4 py-3" />
                </label>
              </div>
              {Number(acertos) + Number(erros) !== Number(questoes) ? <p className="text-sm text-red-700">Questões devem ser iguais à soma de acertos e erros.</p> : null}
              <button disabled={busy || Number(acertos) + Number(erros) !== Number(questoes)} className="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-5 py-3 text-sm font-medium text-white disabled:opacity-50">
                <Save className="h-4 w-4" />{busy ? "Salvando..." : "Salvar sessão"}
              </button>
            </CardContent>
          </Card>

          <div className="space-y-6">
            <Card className="border-slate-200">
              <CardHeader><CardTitle>Resumo da sessão</CardTitle></CardHeader>
              <CardContent className="space-y-4">
                <div className="rounded-3xl bg-slate-100 p-4"><p className="text-sm text-slate-600">Disciplina</p><p className="mt-1 font-semibold text-slate-950">{disciplinas.find((item) => item.id === disciplinaId)?.nome}</p><p className="text-sm text-slate-600">{selectedSubject?.nome ?? "Sessão geral"}</p></div>
                <div className="rounded-3xl bg-violet-50 p-4"><p className="text-sm text-violet-700">Questões resolvidas</p><p className="mt-1 text-2xl font-semibold text-slate-950">{questoes}</p></div>
                <div className="flex items-center gap-2 rounded-3xl bg-emerald-50 p-4 text-sm text-emerald-800"><CheckCircle2 className="h-5 w-5" /> Aproveitamento: {accuracy}%</div>
              </CardContent>
            </Card>
          </div>
        </form>
      ) : null}
    </AppShell>
  );
}
