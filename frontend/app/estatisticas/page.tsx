"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { AppShell } from "@/components/app-shell";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { EmptyState, ErrorState, LoadingState } from "@/components/ui/async-state";
import { useStudyUser } from "@/components/study-user-provider";
import { apiRequest, formatApiError, meApiPath, type Disciplina, type SessaoEstudo } from "@/lib/api";

export default function EstatisticasPage() {
  const { user } = useStudyUser();
  const [sessions, setSessions] = useState<SessaoEstudo[]>([]);
  const [disciplines, setDisciplines] = useState<Disciplina[]>([]);
  const [error, setError] = useState("");
  const [loaded, setLoaded] = useState(false);

  const load = useCallback(async () => {
    if (!user) return;
    try {
      const [sessionData, disciplineData] = await Promise.all([
        apiRequest<SessaoEstudo[]>(meApiPath("/sessoes-estudo")),
        apiRequest<Disciplina[]>(meApiPath("/disciplinas")),
      ]);
      setError("");
      setSessions(sessionData);
      setDisciplines(disciplineData);
    } catch (requestError) {
      setError(formatApiError(requestError));
    } finally {
      setLoaded(true);
    }
  }, [user]);

  useEffect(() => { void load(); }, [load]);

  const metrics = useMemo(() => {
    const now = new Date();
    const toDateKey = (date: Date) => `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, "0")}-${String(date.getDate()).padStart(2, "0")}`;
    const todayKey = toDateKey(now);
    const start = new Date(now);
    start.setDate(now.getDate() - 29);
    const startKey = toDateKey(start);
    const lastMonth = sessions.filter((session) => session.data >= startKey && session.data <= todayKey);
    const questions = lastMonth.reduce((sum, session) => sum + session.questoes, 0);
    const correct = lastMonth.reduce((sum, session) => sum + session.acertos, 0);
    const studyMinutes = lastMonth.reduce((sum, session) => sum + session.tempoEstudadoMinutos, 0);
    const days = Array.from({ length: 7 }, (_, index) => {
      const date = new Date();
      date.setDate(date.getDate() - (6 - index));
      const key = toDateKey(date);
      const daySessions = sessions.filter((session) => session.data === key);
      return {
        key,
        label: new Intl.DateTimeFormat("pt-BR", { weekday: "short" }).format(date).replace(".", ""),
        questions: daySessions.reduce((sum, session) => sum + session.questoes, 0),
        correct: daySessions.reduce((sum, session) => sum + session.acertos, 0),
        minutes: daySessions.reduce((sum, session) => sum + session.tempoEstudadoMinutos, 0),
      };
    });
    const activeDays = new Set(lastMonth.map((session) => session.data)).size;
    return {
      questions,
      accuracy: questions ? Math.round((correct / questions) * 100) : 0,
      studyMinutes,
      activeDays,
      days,
      byDiscipline: disciplines.map((discipline) => {
        const group = lastMonth.filter((session) => session.disciplinaId === discipline.id);
        const totalQuestions = group.reduce((sum, session) => sum + session.questoes, 0);
        const totalCorrect = group.reduce((sum, session) => sum + session.acertos, 0);
        return {
          ...discipline,
          sessions: group.length,
          minutes: group.reduce((sum, session) => sum + session.tempoEstudadoMinutos, 0),
          accuracy: totalQuestions ? Math.round((totalCorrect / totalQuestions) * 100) : 0,
        };
      }).filter((discipline) => discipline.sessions > 0),
    };
  }, [sessions, disciplines]);

  const maxMinutes = Math.max(1, ...metrics.days.map((day) => day.minutes));

  return (
    <AppShell title="Estatísticas" subtitle="Indicadores calculados a partir das sessões registradas nos últimos 30 dias.">
      {error ? <ErrorState message={error} onRetry={() => void load()} /> : null}
      {!loaded && !error ? <LoadingState label="Calculando estatísticas..." /> : null}
      {loaded && sessions.length === 0 && !error ? <EmptyState message="Registre algumas sessões para ver suas estatísticas." /> : null}
      {sessions.length > 0 ? (
        <>
          <section className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
            {[
              { title: "Taxa de acerto", value: `${metrics.accuracy}%`, detail: "No período de 30 dias" },
              { title: "Tempo total", value: `${(metrics.studyMinutes / 60).toFixed(1)}h`, detail: "Estudadas no período" },
              { title: "Questões", value: metrics.questions, detail: "Resolvidas no período" },
              { title: "Dias ativos", value: `${metrics.activeDays}/30`, detail: "Com sessões registradas" },
            ].map((item) => (
              <Card key={item.title} className="border-slate-200">
                <CardHeader><CardTitle>{item.title}</CardTitle></CardHeader>
                <CardContent className="text-3xl font-semibold text-slate-950">{item.value}</CardContent>
                <CardDescription>{item.detail}</CardDescription>
              </Card>
            ))}
          </section>

          <Card className="border-slate-200">
            <CardHeader><CardTitle>Desempenho semanal</CardTitle><CardDescription>Tempo e aproveitamento por dia, conforme seus registros.</CardDescription></CardHeader>
            <CardContent>
              <div className="flex h-64 items-end gap-3">
                {metrics.days.map((day) => (
                  <div key={day.key} className="flex h-full flex-1 flex-col items-center justify-end gap-2">
                    <span className="text-xs text-slate-500">{day.minutes ? `${(day.minutes / 60).toFixed(1)}h` : ""}</span>
                    <div className="w-full min-h-1 rounded-t-2xl bg-gradient-to-t from-sky-600 to-violet-400" style={{ height: `${Math.max(1, (day.minutes / maxMinutes) * 78)}%` }} title={`${day.questions} questões; ${day.questions ? Math.round((day.correct / day.questions) * 100) : 0}% de acerto`} />
                    <span className="text-xs text-slate-500">{day.label}</span>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>

          <Card className="border-slate-200">
            <CardHeader><CardTitle>Por disciplina</CardTitle><CardDescription>Comparação de tempo e acertos no período.</CardDescription></CardHeader>
            <CardContent className="space-y-3">
              {metrics.byDiscipline.map((discipline) => (
                <div key={discipline.id} className="grid gap-2 rounded-2xl border border-slate-100 p-4 sm:grid-cols-[1fr_auto_auto_auto] sm:items-center">
                  <p className="font-medium text-slate-900">{discipline.nome}</p>
                  <span className="text-sm text-slate-600">{discipline.sessions} sessões</span>
                  <span className="text-sm text-slate-600">{(discipline.minutes / 60).toFixed(1)}h</span>
                  <span className="rounded-full bg-emerald-50 px-3 py-1 text-sm font-medium text-emerald-700">{discipline.accuracy}% acertos</span>
                </div>
              ))}
              {metrics.byDiscipline.length === 0 ? <p className="text-sm text-slate-500">Nenhuma sessão no período.</p> : null}
            </CardContent>
          </Card>
        </>
      ) : null}
    </AppShell>
  );
}
