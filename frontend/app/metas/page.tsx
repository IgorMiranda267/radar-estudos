"use client";

import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import { Pencil, Plus, Target, Trash2, TrendingUp } from "lucide-react";
import { AppShell } from "@/components/app-shell";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { EmptyState, ErrorState, LoadingState } from "@/components/ui/async-state";
import { useStudyUser } from "@/components/study-user-provider";
import { apiRequest, formatApiError, meApiPath, type Meta, type Revisao, type SessaoEstudo } from "@/lib/api";

const today = () => {
  const date = new Date();
  return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, "0")}-${String(date.getDate()).padStart(2, "0")}`;
};
const emptyForm = { tipo: "StudyMinutes", nome: "", quantidade: "", unidade: "minutos", dataInicio: today(), dataFim: today() };
const goalTypes: Record<Meta["tipo"], { label: string; unit: string }> = {
  StudyMinutes: { label: "Tempo de estudo", unit: "minutos" },
  Questions: { label: "Questões resolvidas", unit: "questões" },
  StudySessions: { label: "Sessões de estudo", unit: "sessões" },
  Reviews: { label: "Revisões concluídas", unit: "revisões" },
};

export default function MetasPage() {
  const { user } = useStudyUser();
  const [metas, setMetas] = useState<Meta[]>([]);
  const [sessions, setSessions] = useState<SessaoEstudo[]>([]);
  const [reviews, setReviews] = useState<Revisao[]>([]);
  const [form, setForm] = useState(emptyForm);
  const [editing, setEditing] = useState<string | null>(null);
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);
  const [loaded, setLoaded] = useState(false);

  const load = useCallback(async () => {
    if (!user) return;
    try {
      const [goalData, sessionData, reviewData] = await Promise.all([
        apiRequest<Meta[]>(meApiPath("/metas")),
        apiRequest<SessaoEstudo[]>(meApiPath("/sessoes-estudo")),
        apiRequest<Revisao[]>(meApiPath("/revisoes")),
      ]);
      setError("");
      setMetas(goalData);
      setSessions(sessionData);
      setReviews(reviewData);
    } catch (requestError) {
      setError(formatApiError(requestError));
    } finally {
      setLoaded(true);
    }
  }, [user]);

  useEffect(() => { void load(); }, [load]);

  const goalProgress = useMemo(() => (meta: Meta) => {
    const goalSessions = sessions.filter((session) => session.data >= meta.dataInicio && session.data <= meta.dataFim);
    switch (meta.tipo) {
      case "StudyMinutes":
        return goalSessions.reduce((sum, session) => sum + session.tempoEstudadoMinutos, 0);
      case "Questions":
        return goalSessions.reduce((sum, session) => sum + session.questoes, 0);
      case "StudySessions":
        return goalSessions.length;
      case "Reviews":
        return reviews.filter((review) =>
          review.status === "Completed" && review.dataAgendada >= meta.dataInicio && review.dataAgendada <= meta.dataFim).length;
    }
  }, [sessions, reviews]);

  async function save(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!user) return;
    setBusy(true);
    setError("");
    try {
      await apiRequest(meApiPath(`/metas${editing ? `/${editing}` : ""}`), {
        method: editing ? "PUT" : "POST",
        body: JSON.stringify({
          tipo: form.tipo,
          nome: form.nome,
          quantidade: Number(form.quantidade),
          unidade: form.unidade,
          dataInicio: form.dataInicio,
          dataFim: form.dataFim,
          ...(editing ? { ativa: true } : {}),
        }),
      });
      setForm(emptyForm);
      setEditing(null);
      await load();
    } catch (requestError) {
      setError(formatApiError(requestError));
    } finally {
      setBusy(false);
    }
  }

  async function remove(meta: Meta) {
    if (!user || !window.confirm(`Excluir a meta "${meta.nome}"?`)) return;
    setError("");
    try {
      await apiRequest(meApiPath(`/metas/${meta.id}`), { method: "DELETE" });
      await load();
    } catch (requestError) {
      setError(formatApiError(requestError));
    }
  }

  return (
    <AppShell title="Metas" subtitle="O progresso é calculado com seus registros do período, sem valores simulados.">
      {error ? <ErrorState message={error} onRetry={() => void load()} /> : null}
      <Card className="border-slate-200">
        <CardHeader><CardTitle>{editing ? "Editar meta" : "Criar meta"}</CardTitle></CardHeader>
        <CardContent>
          <form onSubmit={save} className="grid gap-3 md:grid-cols-3">
            <select value={form.tipo} onChange={(event) => {
              const type = event.target.value as Meta["tipo"];
              setForm({ ...form, tipo: type, unidade: goalTypes[type].unit });
            }} className="rounded-2xl border border-slate-200 bg-white px-4 py-3">
              {Object.entries(goalTypes).map(([value, item]) => <option key={value} value={value}>{item.label}</option>)}
            </select>
            <input required maxLength={150} value={form.nome} onChange={(event) => setForm({ ...form, nome: event.target.value })} placeholder="Nome da meta" className="rounded-2xl border border-slate-200 px-4 py-3" />
            <input required type="number" min={1} value={form.quantidade} onChange={(event) => setForm({ ...form, quantidade: event.target.value })} placeholder={`Alvo (${form.unidade})`} className="rounded-2xl border border-slate-200 px-4 py-3" />
            <input required type="date" value={form.dataInicio} onChange={(event) => setForm({ ...form, dataInicio: event.target.value })} className="rounded-2xl border border-slate-200 px-4 py-3" />
            <input required type="date" min={form.dataInicio} value={form.dataFim} onChange={(event) => setForm({ ...form, dataFim: event.target.value })} className="rounded-2xl border border-slate-200 px-4 py-3" />
            <div className="flex gap-2">
              <button disabled={busy} className="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-3 text-sm font-medium text-white disabled:opacity-60"><Plus className="h-4 w-4" />{busy ? "Salvando..." : editing ? "Salvar" : "Adicionar meta"}</button>
              {editing ? <button type="button" onClick={() => { setEditing(null); setForm(emptyForm); }} className="rounded-2xl border border-slate-200 px-3 py-2 text-sm">Cancelar</button> : null}
            </div>
          </form>
        </CardContent>
      </Card>

      {!loaded && !error ? <LoadingState label="Carregando metas..." /> : null}
      {metas.length > 0 ? (
        <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {metas.map((meta) => {
            const current = goalProgress(meta);
            const progress = Math.min(100, Math.round((current / meta.quantidade) * 100));
            const type = goalTypes[meta.tipo];
            return (
              <Card key={meta.id} className="border-slate-200">
                <CardHeader>
                  <div className="flex items-center justify-between"><CardTitle>{meta.nome}</CardTitle><Target className="h-5 w-5 text-slate-500" /></div>
                  <p className="text-sm text-slate-500">{type.label} · {meta.dataInicio} a {meta.dataFim}</p>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="flex items-end justify-between"><span className="text-3xl font-semibold text-slate-950">{current} <span className="text-base font-normal">{meta.unidade}</span></span><span className="text-sm text-slate-500">Alvo: {meta.quantidade}</span></div>
                  <div className="h-2.5 w-full rounded-full bg-slate-100"><div className="h-2.5 rounded-full bg-emerald-500" style={{ width: `${progress}%` }} /></div>
                  <div className="flex items-center justify-between text-sm text-slate-600"><span className="inline-flex items-center gap-2"><TrendingUp className="h-4 w-4 text-emerald-500" />{progress}% concluído</span><span>{meta.ativa ? "Ativa" : "Inativa"}</span></div>
                  <div className="flex gap-2">
                    <button type="button" onClick={() => { setEditing(meta.id); setForm({ tipo: meta.tipo, nome: meta.nome, quantidade: String(meta.quantidade), unidade: meta.unidade, dataInicio: meta.dataInicio, dataFim: meta.dataFim }); }} className="inline-flex items-center gap-1 rounded-xl border border-slate-200 px-3 py-2 text-sm"><Pencil className="h-4 w-4" />Editar</button>
                    <button type="button" onClick={() => void remove(meta)} className="inline-flex items-center gap-1 rounded-xl border border-slate-200 px-3 py-2 text-sm text-red-700"><Trash2 className="h-4 w-4" />Excluir</button>
                  </div>
                </CardContent>
              </Card>
            );
          })}
        </section>
      ) : null}
      {loaded && metas.length === 0 && !error ? <EmptyState message="Nenhuma meta cadastrada. Crie uma para acompanhar seus objetivos." /> : null}
    </AppShell>
  );
}
