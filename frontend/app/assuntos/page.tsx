"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { CalendarPlus, FileText, Pencil, Plus, Trash2 } from "lucide-react";
import { AppShell } from "@/components/app-shell";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { EmptyState, ErrorState, LoadingState } from "@/components/ui/async-state";
import { useStudyUser } from "@/components/study-user-provider";
import { apiRequest, formatApiError, meApiPath, type Assunto, type Disciplina } from "@/lib/api";

const emptyForm = { nome: "", descricao: "", dificuldade: "", ordem: "1" };
const difficultyLabels = { Low: "Baixa", Medium: "Média", High: "Alta" };

export default function AssuntosPage() {
  const { user } = useStudyUser();
  const [disciplinas, setDisciplinas] = useState<Disciplina[]>([]);
  const [assuntos, setAssuntos] = useState<(Assunto & { disciplinaNome: string })[]>([]);
  const [disciplinaId, setDisciplinaId] = useState("");
  const [editing, setEditing] = useState<string | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [error, setError] = useState("");
  const [loaded, setLoaded] = useState(false);
  const [busy, setBusy] = useState(false);

  const load = useCallback(async () => {
    if (!user) return;
    try {
      const allDisciplines = await apiRequest<Disciplina[]>(meApiPath("/disciplinas"));
      setError("");
      setDisciplinas(allDisciplines);
      if (!disciplinaId && allDisciplines[0]) setDisciplinaId(allDisciplines[0].id);
      const activeDisciplines = allDisciplines.filter((item) => item.ativa);
      const groups = await Promise.all(activeDisciplines.map(async (disciplina) => {
        const items = await apiRequest<Assunto[]>(
          meApiPath(`/disciplinas/${disciplina.id}/assuntos`),
        );
        return items.map((assunto) => ({ ...assunto, disciplinaNome: disciplina.nome }));
      }));
      setAssuntos(groups.flat());
    } catch (requestError) {
      setError(formatApiError(requestError));
    } finally {
      setLoaded(true);
    }
  }, [user, disciplinaId]);

  useEffect(() => { void load(); }, [load]);

  async function save(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!user || !disciplinaId) return;
    setBusy(true);
    setError("");
    const body = {
      nome: form.nome,
      descricao: form.descricao || null,
      dificuldade: form.dificuldade || null,
      ordem: Number(form.ordem),
    };
    try {
      await apiRequest(
        meApiPath(`/disciplinas/${disciplinaId}/assuntos${editing ? `/${editing}` : ""}`),
        { method: editing ? "PUT" : "POST", body: JSON.stringify(body) },
      );
      setForm(emptyForm);
      setEditing(null);
      await load();
    } catch (requestError) {
      setError(formatApiError(requestError));
    } finally {
      setBusy(false);
    }
  }

  async function scheduleReview(assunto: Assunto) {
    if (!user) return;
    const dataAgendada = window.prompt("Data da revisão (AAAA-MM-DD):", new Date().toISOString().slice(0, 10));
    if (!dataAgendada) return;
    setError("");
    try {
      await apiRequest(meApiPath(`/assuntos/${assunto.id}/revisoes`), {
        method: "POST",
        body: JSON.stringify({ dataAgendada }),
      });
      window.alert("Revisão agendada.");
    } catch (requestError) {
      setError(formatApiError(requestError));
    }
  }

  async function deleteSubject(assunto: Assunto) {
    if (!user || !window.confirm(`Excluir "${assunto.nome}"? Só é permitido sem sessões ou revisões vinculadas.`)) return;
    setError("");
    try {
      await apiRequest(
        meApiPath(`/disciplinas/${assunto.disciplinaId}/assuntos/${assunto.id}`),
        { method: "DELETE" },
      );
      await load();
    } catch (requestError) {
      setError(formatApiError(requestError));
    }
  }

  return (
    <AppShell title="Assuntos" subtitle="Cadastre tópicos por disciplina e agende revisões.">
      {error ? <ErrorState message={error} onRetry={() => void load()} /> : null}
      <Card className="border-slate-200">
        <CardHeader><CardTitle>{editing ? "Editar assunto" : "Novo assunto"}</CardTitle></CardHeader>
        <CardContent>
          <form onSubmit={save} className="grid gap-3 md:grid-cols-2">
            <select required value={disciplinaId} onChange={(event) => setDisciplinaId(event.target.value)} className="rounded-2xl border border-slate-200 bg-white px-4 py-3">
              <option value="">Selecione uma disciplina</option>
              {disciplinas.filter((disciplina) => disciplina.ativa).map((disciplina) => <option key={disciplina.id} value={disciplina.id}>{disciplina.nome}</option>)}
            </select>
            <input required maxLength={150} value={form.nome} onChange={(event) => setForm({ ...form, nome: event.target.value })} placeholder="Nome do assunto" className="rounded-2xl border border-slate-200 px-4 py-3" />
            <input maxLength={4000} value={form.descricao} onChange={(event) => setForm({ ...form, descricao: event.target.value })} placeholder="Descrição (opcional)" className="rounded-2xl border border-slate-200 px-4 py-3" />
            <div className="grid grid-cols-[1fr_120px_auto] gap-3">
              <select value={form.dificuldade} onChange={(event) => setForm({ ...form, dificuldade: event.target.value })} className="rounded-2xl border border-slate-200 bg-white px-4 py-3">
                <option value="">Sem dificuldade</option>
                <option value="Low">Baixa</option>
                <option value="Medium">Média</option>
                <option value="High">Alta</option>
              </select>
              <input aria-label="Ordem" type="number" min={1} value={form.ordem} onChange={(event) => setForm({ ...form, ordem: event.target.value })} className="rounded-2xl border border-slate-200 px-3 py-3" />
              <button disabled={busy || !disciplinaId} className="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-3 text-sm font-medium text-white disabled:opacity-50">
                <Plus className="h-4 w-4" />{busy ? "Salvando..." : editing ? "Salvar" : "Adicionar"}
              </button>
            </div>
          </form>
          {editing ? <button type="button" onClick={() => { setEditing(null); setForm(emptyForm); }} className="mt-3 text-sm underline">Cancelar edição</button> : null}
        </CardContent>
      </Card>

      {!loaded && !error ? <LoadingState label="Carregando assuntos..." /> : null}
      {assuntos.length > 0 ? (
        <section className="grid gap-4 md:grid-cols-2">
          {assuntos.map((assunto) => (
            <Card key={assunto.id} className="border-slate-200">
              <CardHeader>
                <div className="flex items-center justify-between">
                  <div className="flex h-11 w-11 items-center justify-center rounded-2xl bg-slate-100"><FileText className="h-5 w-5 text-slate-700" /></div>
                  {assunto.dificuldade ? <span className="rounded-full bg-slate-100 px-2.5 py-1 text-xs text-slate-600">{difficultyLabels[assunto.dificuldade]}</span> : null}
                </div>
                <CardTitle className="mt-4">{assunto.nome}</CardTitle>
                <CardDescription>{assunto.disciplinaNome}</CardDescription>
              </CardHeader>
              <CardContent>
                {assunto.descricao ? <p className="mb-4 text-sm text-slate-600">{assunto.descricao}</p> : null}
                <div className="flex flex-wrap gap-2">
                  <button type="button" onClick={() => { setDisciplinaId(assunto.disciplinaId); setEditing(assunto.id); setForm({ nome: assunto.nome, descricao: assunto.descricao ?? "", dificuldade: assunto.dificuldade ?? "", ordem: String(assunto.ordem) }); }} className="inline-flex items-center gap-1 rounded-xl border border-slate-200 px-3 py-2 text-sm"><Pencil className="h-4 w-4" />Editar</button>
                  <button type="button" onClick={() => void scheduleReview(assunto)} className="inline-flex items-center gap-1 rounded-xl border border-slate-200 px-3 py-2 text-sm"><CalendarPlus className="h-4 w-4" />Agendar revisão</button>
                  <button type="button" onClick={() => void deleteSubject(assunto)} className="inline-flex items-center gap-1 rounded-xl border border-slate-200 px-3 py-2 text-sm text-red-700"><Trash2 className="h-4 w-4" />Excluir</button>
                </div>
              </CardContent>
            </Card>
          ))}
        </section>
      ) : null}
      {loaded && assuntos.length === 0 && !error ? <EmptyState message="Nenhum assunto cadastrado nas disciplinas ativas." /> : null}
    </AppShell>
  );
}
