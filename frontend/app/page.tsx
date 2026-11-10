"use client";

import { useCallback, useEffect, useState } from "react";
import { BookOpen, ChartBar, CheckCircle2, Clock3, LayoutDashboard, Sparkles } from "lucide-react";
import { AppShell } from "@/components/app-shell";
import { StudyChart } from "@/components/dashboard/study-chart";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { EmptyState, ErrorState, LoadingState } from "@/components/ui/async-state";
import { useStudyUser } from "@/components/study-user-provider";
import { apiRequest, formatApiError, meApiPath, type Dashboard } from "@/lib/api";

function formatHours(minutes: number) {
  return `${(minutes / 60).toFixed(1)}h`;
}

export default function Home() {
  const { user } = useStudyUser();
  const [dashboard, setDashboard] = useState<Dashboard | null>(null);
  const [error, setError] = useState("");

  const loadDashboard = useCallback(async () => {
    if (!user) return;
    try {
      const data = await apiRequest<Dashboard>(meApiPath("/dashboard"));
      setError("");
      setDashboard(data);
    } catch (requestError) {
      setError(formatApiError(requestError));
    }
  }, [user]);

  useEffect(() => {
    void loadDashboard();
  }, [loadDashboard]);

  const accuracy = dashboard?.questoes
    ? Math.round((dashboard.acertos / dashboard.questoes) * 100)
    : 0;
  const chartData = dashboard?.ultimosSeteDias.map((day) => ({
    name: new Intl.DateTimeFormat("pt-BR", { weekday: "short", timeZone: "UTC" })
      .format(new Date(`${day.data}T12:00:00Z`))
      .replace(".", ""),
    horas: Math.round((day.tempoEstudadoMinutos / 60) * 10) / 10,
  })) ?? [];

  return (
    <AppShell
      title="Painel de desempenho"
      subtitle="Indicadores atualizados a partir dos seus registros no backend."
      action={
        <div className="rounded-3xl bg-slate-900 px-4 py-3 text-white shadow-sm shadow-slate-200/10">
          <p className="text-xs uppercase tracking-[0.24em] text-slate-300">Fonte</p>
          <p className="mt-1 text-sm font-semibold text-white">API .NET + PostgreSQL</p>
        </div>
      }
    >
      {error ? <ErrorState message={error} onRetry={() => void loadDashboard()} /> : null}
      {!dashboard && !error ? <LoadingState /> : null}
      {dashboard ? (
        <>
          <section className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
            {[
              { label: "Questões", value: dashboard.questoes, description: "Registradas nas sessões.", icon: LayoutDashboard },
              { label: "Horas estudadas", value: formatHours(dashboard.tempoEstudadoMinutos), description: "Tempo total registrado.", icon: Sparkles },
              { label: "Aproveitamento", value: `${accuracy}%`, description: "Acertos sobre questões.", icon: CheckCircle2 },
              { label: "Disciplinas ativas", value: dashboard.totalDisciplinas, description: "Áreas de estudo.", icon: BookOpen },
              { label: "Sessões", value: dashboard.totalSessoes, description: "Registros de estudo.", icon: ChartBar },
              { label: "Assuntos", value: dashboard.totalAssuntos, description: "Tópicos organizados.", icon: Clock3 },
            ].map((item) => (
              <Card key={item.label} className="border-slate-200">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2 text-slate-900">
                    <item.icon className="h-4 w-4 text-slate-500" />
                    {item.label}
                  </CardTitle>
                </CardHeader>
                <CardContent className="mt-2 text-3xl font-semibold text-slate-950">{item.value}</CardContent>
                <CardDescription>{item.description}</CardDescription>
              </Card>
            ))}
          </section>

          <section className="grid gap-6 xl:grid-cols-[1.5fr_1fr]">
            <StudyChart data={chartData} />
            <Card className="border-slate-200">
              <CardHeader>
                <CardTitle>Resumo rápido</CardTitle>
                <CardDescription>Dados consolidados do seu histórico.</CardDescription>
              </CardHeader>
              <CardContent className="space-y-4 pt-2">
                <div className="rounded-3xl bg-slate-100 p-4">
                  <p className="text-sm text-slate-600">Revisões pendentes</p>
                  <p className="mt-2 text-2xl font-semibold text-slate-950">{dashboard.revisoesPendentes}</p>
                </div>
                <div className="rounded-3xl bg-slate-100 p-4">
                  <p className="text-sm text-slate-600">Acertos / erros</p>
                  <p className="mt-2 text-2xl font-semibold text-slate-950">{dashboard.acertos} / {dashboard.erros}</p>
                </div>
              </CardContent>
            </Card>
          </section>
          {dashboard.totalSessoes === 0 ? <EmptyState message="Ainda não há sessões registradas. Inicie pela tela Sessão." /> : null}
        </>
      ) : null}
    </AppShell>
  );
}
