"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { Bell, Save, SlidersHorizontal } from "lucide-react";
import { AppShell } from "@/components/app-shell";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { ErrorState, LoadingState } from "@/components/ui/async-state";
import { useStudyUser } from "@/components/study-user-provider";
import { apiRequest, formatApiError, meApiPath, type ConfiguracaoUsuario, type Disciplina } from "@/lib/api";

const initialSettings: ConfiguracaoUsuario = {
  tema: "system",
  idioma: "pt-BR",
  notificacoesAtivadas: true,
  objetivoDiarioMinutos: 120,
  disciplinaFocoId: null,
};

export default function ConfiguracoesPage() {
  const { user } = useStudyUser();
  const [settings, setSettings] = useState(initialSettings);
  const [disciplines, setDisciplines] = useState<Disciplina[]>([]);
  const [error, setError] = useState("");
  const [loaded, setLoaded] = useState(false);
  const [busy, setBusy] = useState(false);
  const [saved, setSaved] = useState(false);

  const load = useCallback(async () => {
    if (!user) return;
    try {
      const [preferences, disciplineData] = await Promise.all([
        apiRequest<ConfiguracaoUsuario>(meApiPath("/configuracoes")),
        apiRequest<Disciplina[]>(meApiPath("/disciplinas")),
      ]);
      setError("");
      setSettings(preferences);
      setDisciplines(disciplineData.filter((discipline) => discipline.ativa));
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
    setSaved(false);
    setError("");
    try {
      const updated = await apiRequest<ConfiguracaoUsuario>(meApiPath("/configuracoes"), {
        method: "PUT",
        body: JSON.stringify(settings),
      });
      setSettings(updated);
      setSaved(true);
    } catch (requestError) {
      setError(formatApiError(requestError));
    } finally {
      setBusy(false);
    }
  }

  return (
    <AppShell title="Configurações" subtitle="Preferências salvas na sua conta.">
      {error ? <ErrorState message={error} onRetry={() => void load()} /> : null}
      {!loaded && !error ? <LoadingState label="Carregando preferências..." /> : null}
      {loaded ? (
        <form onSubmit={save} className="space-y-5">
          <Card className="border-slate-200">
            <CardHeader>
              <div className="flex items-center gap-3">
                <div className="flex h-11 w-11 items-center justify-center rounded-2xl bg-slate-100"><Bell className="h-5 w-5 text-slate-700" /></div>
                <div><CardTitle>Notificações</CardTitle><CardDescription>Lembretes de estudo e revisões.</CardDescription></div>
              </div>
            </CardHeader>
            <CardContent>
              <label className="flex items-center justify-between gap-4">
                <span className="text-sm text-slate-700">Ativar notificações</span>
                <input type="checkbox" checked={settings.notificacoesAtivadas} onChange={(event) => setSettings({ ...settings, notificacoesAtivadas: event.target.checked })} className="h-5 w-5 accent-slate-900" />
              </label>
            </CardContent>
          </Card>

          <Card className="border-slate-200">
            <CardHeader>
              <div className="flex items-center gap-3">
                <div className="flex h-11 w-11 items-center justify-center rounded-2xl bg-violet-100"><SlidersHorizontal className="h-5 w-5 text-violet-700" /></div>
                <CardTitle>Preferências de estudo</CardTitle>
              </div>
            </CardHeader>
            <CardContent className="grid gap-4 md:grid-cols-2">
              <label className="grid gap-2 text-sm text-slate-600">
                Objetivo diário (minutos)
                <input type="number" min={0} max={1440} required value={settings.objetivoDiarioMinutos} onChange={(event) => setSettings({ ...settings, objetivoDiarioMinutos: Number(event.target.value) })} className="rounded-2xl border border-slate-200 px-4 py-3 text-slate-900" />
              </label>
              <label className="grid gap-2 text-sm text-slate-600">
                Disciplina de foco
                <select value={settings.disciplinaFocoId ?? ""} onChange={(event) => setSettings({ ...settings, disciplinaFocoId: event.target.value || null })} className="rounded-2xl border border-slate-200 bg-white px-4 py-3 text-slate-900">
                  <option value="">Nenhuma selecionada</option>
                  {disciplines.map((discipline) => <option key={discipline.id} value={discipline.id}>{discipline.nome}</option>)}
                </select>
              </label>
              <label className="grid gap-2 text-sm text-slate-600">
                Tema
                <select value={settings.tema} onChange={(event) => setSettings({ ...settings, tema: event.target.value })} className="rounded-2xl border border-slate-200 bg-white px-4 py-3 text-slate-900">
                  <option value="system">Sistema</option>
                  <option value="light">Claro</option>
                  <option value="dark">Escuro</option>
                </select>
              </label>
              <label className="grid gap-2 text-sm text-slate-600">
                Idioma
                <select value={settings.idioma} onChange={(event) => setSettings({ ...settings, idioma: event.target.value })} className="rounded-2xl border border-slate-200 bg-white px-4 py-3 text-slate-900">
                  <option value="pt-BR">Português (Brasil)</option>
                  <option value="en">English</option>
                </select>
              </label>
            </CardContent>
          </Card>

          <p className="text-sm text-slate-500">Tema e idioma são armazenados na API; a interface ainda é exibida em português e tema claro.</p>
          <div className="flex items-center gap-4">
            <button disabled={busy} className="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-5 py-3 text-sm font-medium text-white disabled:opacity-60"><Save className="h-4 w-4" />{busy ? "Salvando..." : "Salvar preferências"}</button>
            {saved ? <span role="status" className="text-sm text-emerald-700">Preferências salvas.</span> : null}
          </div>
        </form>
      ) : null}
    </AppShell>
  );
}
