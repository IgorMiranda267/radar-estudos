"use client";

import { useCallback, useEffect, useState } from "react";
import { AppShell } from "@/components/app-shell";
import { Card, CardContent, CardDescription } from "@/components/ui/card";
import { EmptyState, ErrorState, LoadingState } from "@/components/ui/async-state";
import { useStudyUser } from "@/components/study-user-provider";
import { apiRequest, formatApiError, meApiPath, type Assunto, type Disciplina, type SessaoEstudo } from "@/lib/api";

interface HistoricoItem extends SessaoEstudo {
  disciplinaNome: string;
  assuntoNome: string | null;
}

export default function HistoricoPage() {
  const { user } = useStudyUser();
  const [history, setHistory] = useState<HistoricoItem[]>([]);
  const [error, setError] = useState("");
  const [loaded, setLoaded] = useState(false);

  const load = useCallback(async () => {
    if (!user) return;
    try {
      const [sessions, disciplines] = await Promise.all([
        apiRequest<SessaoEstudo[]>(meApiPath("/sessoes-estudo")),
        apiRequest<Disciplina[]>(meApiPath("/disciplinas")),
      ]);
      setError("");
      const subjectsByDiscipline = await Promise.all(disciplines.map(async (discipline) => {
        const subjects = await apiRequest<Assunto[]>(
          meApiPath(`/disciplinas/${discipline.id}/assuntos`),
        );
        return [discipline.id, subjects] as const;
      }));
      const disciplineNames = new Map(disciplines.map((item) => [item.id, item.nome]));
      const subjectNames = new Map(subjectsByDiscipline.flatMap(([, subjects]) =>
        subjects.map((subject) => [subject.id, subject.nome] as const)));
      setHistory(sessions.map((session) => ({
        ...session,
        disciplinaNome: disciplineNames.get(session.disciplinaId) ?? "Disciplina removida",
        assuntoNome: session.assuntoId ? subjectNames.get(session.assuntoId) ?? "Assunto removido" : null,
      })).sort((a, b) => b.data.localeCompare(a.data) || b.criadaEm.localeCompare(a.criadaEm)));
    } catch (requestError) {
      setError(formatApiError(requestError));
    } finally {
      setLoaded(true);
    }
  }, [user]);

  useEffect(() => { void load(); }, [load]);

  return (
    <AppShell title="Histórico" subtitle="Suas sessões gravadas no PostgreSQL, da mais recente à mais antiga.">
      {error ? <ErrorState message={error} onRetry={() => void load()} /> : null}
      {!loaded && !error ? <LoadingState label="Carregando histórico..." /> : null}
      {history.map((item) => (
        <Card key={item.id} className="border-slate-200">
          <CardContent className="flex flex-col gap-3 py-5 md:flex-row md:items-center md:justify-between">
            <div>
              <p className="text-lg font-semibold text-slate-950">{item.disciplinaNome} — {item.assuntoNome ?? "Sessão geral"}</p>
              <CardDescription>{new Intl.DateTimeFormat("pt-BR", { dateStyle: "long", timeZone: "UTC" }).format(new Date(`${item.data}T12:00:00Z`))}</CardDescription>
              {item.observacoes ? <p className="mt-2 text-sm text-slate-600">{item.observacoes}</p> : null}
            </div>
            <div className="flex flex-wrap items-center gap-3 text-sm text-slate-600">
              <span>{Math.floor(item.tempoEstudadoMinutos / 60)}h {item.tempoEstudadoMinutos % 60}m</span>
              <span>{item.questoes} questões</span>
              <span>{item.acertos} acertos</span>
              <span className="rounded-full bg-slate-100 px-3 py-1 font-medium text-slate-800">
                {item.questoes ? Math.round((item.acertos / item.questoes) * 100) : 0}%
              </span>
            </div>
          </CardContent>
        </Card>
      ))}
      {loaded && history.length === 0 && !error ? <EmptyState message="Seu histórico ainda está vazio. Registre uma sessão de estudo para começar." /> : null}
    </AppShell>
  );
}
