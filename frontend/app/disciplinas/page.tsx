"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { BookOpen, Pencil, Plus, Power } from "lucide-react";
import { AppShell } from "@/components/app-shell";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { EmptyState, ErrorState, LoadingState } from "@/components/ui/async-state";
import { useStudyUser } from "@/components/study-user-provider";
import { apiRequest, formatApiError, meApiPath, type Disciplina } from "@/lib/api";

const initialForm = { nome: "", cor: "#2563EB", icone: "book-open" };

export default function DisciplinasPage() {
  const { user } = useStudyUser();
  const [disciplinas, setDisciplinas] = useState<Disciplina[]>([]);
  const [form, setForm] = useState(initialForm);
  const [editing, setEditing] = useState<string | null>(null);
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);
  const [loaded, setLoaded] = useState(false);

  const load = useCallback(async () => {
    if (!user) return;
    try {
      const items = await apiRequest<Disciplina[]>(meApiPath("/disciplinas"));
      setError("");
      setDisciplinas(items);
    } catch (requestError) {
      setError(formatApiError(requestError));
    } finally {
      setLoaded(true);
    }
  }, [user]);

  useEffect(() => { void load(); }, [load]);

  async function save(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!user) return;
    setBusy(true);
    setError("");
    try {
      await apiRequest(
        meApiPath(`/disciplinas${editing ? `/${editing}` : ""}`),
        { method: editing ? "PUT" : "POST", body: JSON.stringify(form) },
      );
      setForm(initialForm);
      setEditing(null);
      await load();
    } catch (requestError) {
      setError(formatApiError(requestError));
    } finally {
      setBusy(false);
    }
  }

  async function deactivate(disciplina: Disciplina) {
    if (!user || !window.confirm(`Desativar "${disciplina.nome}"? O histórico será mantido.`)) return;
    setError("");
    try {
      await apiRequest(meApiPath(`/disciplinas/${disciplina.id}`), { method: "DELETE" });
      await load();
    } catch (requestError) {
      setError(formatApiError(requestError));
    }
  }

  return (
    <AppShell title="Disciplinas" subtitle="Cadastre e organize suas áreas de estudo.">
      {error ? <ErrorState message={error} onRetry={() => void load()} /> : null}
      <Card className="border-slate-200">
        <CardHeader><CardTitle>{editing ? "Editar disciplina" : "Nova disciplina"}</CardTitle></CardHeader>
        <CardContent>
          <form onSubmit={save} className="grid gap-3 md:grid-cols-[1fr_150px_180px_auto]">
            <input required maxLength={150} value={form.nome} onChange={(event) => setForm({ ...form, nome: event.target.value })} placeholder="Nome da disciplina" className="rounded-2xl border border-slate-200 px-4 py-3" />
            <label className="flex items-center gap-2 rounded-2xl border border-slate-200 px-3 py-2 text-sm text-slate-600">
              Cor <input type="color" value={form.cor} onChange={(event) => setForm({ ...form, cor: event.target.value })} className="h-8 w-10 cursor-pointer border-0 bg-transparent" />
            </label>
            <input required maxLength={50} value={form.icone} onChange={(event) => setForm({ ...form, icone: event.target.value })} placeholder="Ícone (ex.: book-open)" className="rounded-2xl border border-slate-200 px-4 py-3" />
            <div className="flex gap-2">
              <button disabled={busy} className="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-3 text-sm font-medium text-white disabled:opacity-60">
                <Plus className="h-4 w-4" />{busy ? "Salvando..." : editing ? "Salvar" : "Adicionar"}
              </button>
              {editing ? <button type="button" onClick={() => { setEditing(null); setForm(initialForm); }} className="rounded-2xl border border-slate-200 px-3 py-2 text-sm">Cancelar</button> : null}
            </div>
          </form>
        </CardContent>
      </Card>

      {!loaded && !error ? <LoadingState label="Carregando disciplinas..." /> : null}
      {disciplinas.length > 0 ? (
        <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {disciplinas.map((disciplina) => (
            <Card key={disciplina.id} className="border-slate-200">
              <CardHeader>
                <div className="flex items-center justify-between">
                  <div className="flex h-11 w-11 items-center justify-center rounded-2xl" style={{ backgroundColor: disciplina.cor }}>
                    <BookOpen className="h-5 w-5 text-white" />
                  </div>
                  <span className={`rounded-full px-3 py-1 text-xs font-medium ${disciplina.ativa ? "bg-emerald-50 text-emerald-700" : "bg-slate-100 text-slate-500"}`}>
                    {disciplina.ativa ? "Ativa" : "Inativa"}
                  </span>
                </div>
                <CardTitle className="mt-4">{disciplina.nome}</CardTitle>
              </CardHeader>
              <CardContent className="flex gap-2">
                <button type="button" onClick={() => { setEditing(disciplina.id); setForm({ nome: disciplina.nome, cor: disciplina.cor, icone: disciplina.icone }); }} className="inline-flex items-center gap-2 rounded-xl border border-slate-200 px-3 py-2 text-sm">
                  <Pencil className="h-4 w-4" />Editar
                </button>
                {disciplina.ativa ? (
                  <button type="button" onClick={() => void deactivate(disciplina)} className="inline-flex items-center gap-2 rounded-xl border border-slate-200 px-3 py-2 text-sm text-red-700">
                    <Power className="h-4 w-4" />Desativar
                  </button>
                ) : null}
              </CardContent>
            </Card>
          ))}
        </section>
      ) : null}
      {loaded && disciplinas.length === 0 && !error ? <EmptyState message="Ainda não há disciplinas. Cadastre a primeira acima." /> : null}
    </AppShell>
  );
}
