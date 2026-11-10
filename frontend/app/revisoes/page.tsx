"use client";

import { useCallback, useEffect, useState } from "react";
import { CalendarClock, CheckCheck, CircleDashed, Trash2 } from "lucide-react";
import { AppShell } from "@/components/app-shell";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { EmptyState, ErrorState, LoadingState } from "@/components/ui/async-state";
import { useStudyUser } from "@/components/study-user-provider";
import { apiRequest, formatApiError, meApiPath, type Assunto, type Disciplina, type Revisao } from "@/lib/api";

interface ReviewItem extends Revisao {
  assuntoNome: string;
  disciplinaNome: string;
}

const statusNames: Record<Revisao["status"], string> = {
  Pending: "Pendente",
  Completed: "Concluída",
  Cancelled: "Cancelada",
};

export default function RevisoesPage() {
  const { user } = useStudyUser();
  const [reviews, setReviews] = useState<ReviewItem[]>([]);
  const [error, setError] = useState("");
  const [loaded, setLoaded] = useState(false);
  const [busyId, setBusyId] = useState<string | null>(null);

  const load = useCallback(async () => {
    if (!user) return;
    try {
      const [reviewData, disciplines] = await Promise.all([
        apiRequest<Revisao[]>(meApiPath("/revisoes")),
        apiRequest<Disciplina[]>(meApiPath("/disciplinas")),
      ]);
      setError("");
      const groups = await Promise.all(disciplines.map(async (discipline) => ({
        discipline,
        subjects: await apiRequest<Assunto[]>(
          meApiPath(`/disciplinas/${discipline.id}/assuntos`),
        ),
      })));
      const lookup = new Map(groups.flatMap(({ discipline, subjects }) =>
        subjects.map((subject) => [subject.id, { assuntoNome: subject.nome, disciplinaNome: discipline.nome }] as const)));
      setReviews(reviewData
        .map((review) => ({
          ...review,
          ...(lookup.get(review.assuntoId) ?? { assuntoNome: "Assunto removido", disciplinaNome: "Disciplina removida" }),
        }))
        .sort((a, b) => a.dataAgendada.localeCompare(b.dataAgendada)));
    } catch (requestError) {
      setError(formatApiError(requestError));
    } finally {
      setLoaded(true);
    }
  }, [user]);

  useEffect(() => { void load(); }, [load]);

  async function changeStatus(review: ReviewItem, status: Revisao["status"]) {
    if (!user) return;
    setBusyId(review.id);
    setError("");
    try {
      await apiRequest(meApiPath(`/revisoes/${review.id}`), {
        method: "PUT",
        body: JSON.stringify({
          dataAgendada: review.dataAgendada,
          status,
          observacoes: review.observacoes,
        }),
      });
      await load();
    } catch (requestError) {
      setError(formatApiError(requestError));
    } finally {
      setBusyId(null);
    }
  }

  async function reschedule(review: ReviewItem) {
    if (!user) return;
    const dataAgendada = window.prompt("Nova data da revisão (AAAA-MM-DD):", review.dataAgendada);
    if (!dataAgendada) return;
    setBusyId(review.id);
    setError("");
    try {
      await apiRequest(meApiPath(`/revisoes/${review.id}`), {
        method: "PUT",
        body: JSON.stringify({ dataAgendada, status: review.status, observacoes: review.observacoes }),
      });
      await load();
    } catch (requestError) {
      setError(formatApiError(requestError));
    } finally {
      setBusyId(null);
    }
  }

  async function remove(review: ReviewItem) {
    if (!user || !window.confirm(`Excluir a revisão de "${review.assuntoNome}"?`)) return;
    setError("");
    try {
      await apiRequest(meApiPath(`/revisoes/${review.id}`), { method: "DELETE" });
      await load();
    } catch (requestError) {
      setError(formatApiError(requestError));
    }
  }

  return (
    <AppShell title="Revisões" subtitle="Acompanhe, conclua e reagende as revisões vinculadas aos seus assuntos.">
      {error ? <ErrorState message={error} onRetry={() => void load()} /> : null}
      {!loaded && !error ? <LoadingState label="Carregando revisões..." /> : null}
      <section className="grid gap-4 md:grid-cols-2">
        {reviews.map((review) => (
          <Card key={review.id} className="border-slate-200">
            <CardHeader>
              <div className="flex items-center justify-between gap-3">
                <div>
                  <CardTitle className="text-slate-950">{review.assuntoNome}</CardTitle>
                  <CardDescription className="mt-1">{review.disciplinaNome}</CardDescription>
                </div>
                {review.status === "Completed" ? <CheckCheck className="h-5 w-5 shrink-0 text-emerald-500" /> : <CircleDashed className="h-5 w-5 shrink-0 text-amber-500" />}
              </div>
            </CardHeader>
            <CardContent>
              <div className="flex items-center justify-between">
                <span className="text-sm text-slate-500">Agendada para {new Intl.DateTimeFormat("pt-BR", { dateStyle: "long", timeZone: "UTC" }).format(new Date(`${review.dataAgendada}T12:00:00Z`))}</span>
                <span className={`rounded-full px-2.5 py-1 text-xs font-medium ${review.status === "Completed" ? "bg-emerald-100 text-emerald-700" : review.status === "Pending" ? "bg-amber-100 text-amber-700" : "bg-slate-100 text-slate-600"}`}>
                  {statusNames[review.status]}
                </span>
              </div>
              {review.observacoes ? <p className="mt-3 text-sm text-slate-600">{review.observacoes}</p> : null}
              <div className="mt-4 flex flex-wrap gap-2">
                {review.status === "Pending" ? (
                  <>
                    <button disabled={busyId === review.id} onClick={() => void changeStatus(review, "Completed")} className="inline-flex items-center gap-1 rounded-xl bg-emerald-700 px-3 py-2 text-sm text-white disabled:opacity-50"><CheckCheck className="h-4 w-4" />Concluir</button>
                    <button disabled={busyId === review.id} onClick={() => void reschedule(review)} className="inline-flex items-center gap-1 rounded-xl border border-slate-200 px-3 py-2 text-sm"><CalendarClock className="h-4 w-4" />Reagendar</button>
                    <button disabled={busyId === review.id} onClick={() => void changeStatus(review, "Cancelled")} className="rounded-xl border border-slate-200 px-3 py-2 text-sm disabled:opacity-50">Cancelar revisão</button>
                  </>
                ) : review.status === "Completed" ? (
                  <button disabled={busyId === review.id} onClick={() => void changeStatus(review, "Pending")} className="rounded-xl border border-slate-200 px-3 py-2 text-sm disabled:opacity-50">Reabrir revisão</button>
                ) : (
                  <button disabled={busyId === review.id} onClick={() => void changeStatus(review, "Pending")} className="rounded-xl border border-slate-200 px-3 py-2 text-sm disabled:opacity-50">Reativar revisão</button>
                )}
                <button onClick={() => void remove(review)} className="inline-flex items-center gap-1 rounded-xl border border-slate-200 px-3 py-2 text-sm text-red-700"><Trash2 className="h-4 w-4" />Excluir</button>
              </div>
            </CardContent>
          </Card>
        ))}
      </section>
      {loaded && reviews.length === 0 && !error ? <EmptyState message="Nenhuma revisão agendada. Agende uma revisão pela tela de assuntos." /> : null}
    </AppShell>
  );
}
