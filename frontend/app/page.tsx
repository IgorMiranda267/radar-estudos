import { BookOpen, ChartBar, CheckCircle2, Clock3, LayoutDashboard, Sparkles } from "lucide-react";
import prisma from "@/lib/prisma";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { StudyChart } from "@/components/dashboard/study-chart";
import { Sidebar } from "@/components/sidebar";
import { formatHours } from "@/lib/utils/format";

async function getDashboardStats() {
  const totalSessions = await prisma.sessaoEstudo.count();
  const totalAssuntos = await prisma.assunto.count();
  const totalDisciplinas = await prisma.disciplina.count();
  const totals = await prisma.sessaoEstudo.aggregate({
    _sum: {
      tempoEstudado: true,
      questoes: true,
      acertos: true,
      erros: true,
    },
  });

  return {
    totalSessions,
    totalAssuntos,
    totalDisciplinas,
    totalStudyHours: totals._sum.tempoEstudado ?? 0,
    totalQuestions: totals._sum.questoes ?? 0,
    totalCorrect: totals._sum.acertos ?? 0,
    totalWrong: totals._sum.erros ?? 0,
  };
}

export default async function Home() {
  const stats = await getDashboardStats();
  const accuracy = stats.totalQuestions
    ? Math.round((stats.totalCorrect / stats.totalQuestions) * 100)
    : 0;

  return (
    <div className="min-h-screen bg-slate-50 text-slate-950">
      <div className="grid min-h-screen grid-cols-[280px_1fr] gap-8 px-6 py-6 lg:px-10">
        <Sidebar />

        <main className="flex min-h-screen flex-col gap-6">
          <section className="flex flex-col gap-2">
            <div className="flex items-center justify-between gap-4">
              <div>
                <p className="text-sm uppercase tracking-[0.24em] text-slate-500">Radar de Estudos</p>
                <h1 className="mt-2 text-3xl font-semibold text-slate-950">Painel de desempenho</h1>
              </div>
              <div className="rounded-3xl bg-slate-900 px-4 py-3 text-white shadow-sm shadow-slate-200/10">
                <p className="text-xs uppercase tracking-[0.24em] text-slate-300">Status</p>
                <p className="mt-1 text-sm font-semibold text-white">Dados em tempo real</p>
              </div>
            </div>
            <p className="max-w-2xl text-sm leading-6 text-slate-600">
              Monitore sua evolução de estudos, metas e sessões com uma visão clara e organizada.
            </p>
          </section>

          <section className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
            <Card className="border-slate-200">
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-slate-900">
                  <LayoutDashboard className="h-4 w-4 text-slate-500" />
                  Questões Hoje
                </CardTitle>
              </CardHeader>
              <CardContent className="mt-2 text-3xl font-semibold text-slate-950">{stats.totalQuestions}</CardContent>
              <CardDescription>Todos os exercícios cadastrados nas sessões.</CardDescription>
            </Card>
            <Card className="border-slate-200">
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-slate-900">
                  <Sparkles className="h-4 w-4 text-slate-500" />
                  Horas Estudadas
                </CardTitle>
              </CardHeader>
              <CardContent className="mt-2 text-3xl font-semibold text-slate-950">
                {formatHours(stats.totalStudyHours)}
              </CardContent>
              <CardDescription>Tempo total registrado nas sessões.</CardDescription>
            </Card>
            <Card className="border-slate-200">
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-slate-900">
                  <CheckCircle2 className="h-4 w-4 text-slate-500" />
                  Aproveitamento Geral
                </CardTitle>
              </CardHeader>
              <CardContent className="mt-2 text-3xl font-semibold text-slate-950">{accuracy}%</CardContent>
              <CardDescription>Baseado em acertos e erros nas sessões.</CardDescription>
            </Card>
            <Card className="border-slate-200">
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-slate-900">
                  <BookOpen className="h-4 w-4 text-slate-500" />
                  Disciplinas
                </CardTitle>
              </CardHeader>
              <CardContent className="mt-2 text-3xl font-semibold text-slate-950">{stats.totalDisciplinas}</CardContent>
              <CardDescription>Áreas de estudo ativas.</CardDescription>
            </Card>
            <Card className="border-slate-200">
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-slate-900">
                  <ChartBar className="h-4 w-4 text-slate-500" />
                  Sessões
                </CardTitle>
              </CardHeader>
              <CardContent className="mt-2 text-3xl font-semibold text-slate-950">{stats.totalSessions}</CardContent>
              <CardDescription>Total de registros de estudo.</CardDescription>
            </Card>
            <Card className="border-slate-200">
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-slate-900">
                  <Clock3 className="h-4 w-4 text-slate-500" />
                  Assuntos
                </CardTitle>
              </CardHeader>
              <CardContent className="mt-2 text-3xl font-semibold text-slate-950">{stats.totalAssuntos}</CardContent>
              <CardDescription>Conteúdos organizados por disciplina.</CardDescription>
            </Card>
          </section>

          <section className="grid gap-6 xl:grid-cols-[1.5fr_1fr]">
            <StudyChart />
            <Card className="border-slate-200">
              <CardHeader>
                <CardTitle>Resumo rápido</CardTitle>
                <CardDescription>Indicadores essenciais para manter a rotina de estudos.</CardDescription>
              </CardHeader>
              <CardContent className="space-y-4 pt-2">
                <div className="rounded-3xl bg-slate-100 p-4">
                  <p className="text-sm text-slate-600">Sessões registradas</p>
                  <p className="mt-2 text-2xl font-semibold text-slate-950">{stats.totalSessions}</p>
                </div>
                <div className="rounded-3xl bg-slate-100 p-4">
                  <p className="text-sm text-slate-600">Tópicos cadastrados</p>
                  <p className="mt-2 text-2xl font-semibold text-slate-950">{stats.totalAssuntos}</p>
                </div>
              </CardContent>
            </Card>
          </section>
        </main>
      </div>
    </div>
  );
}
