"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { Save } from "lucide-react";
import { AppShell } from "@/components/app-shell";
import { useStudyUser } from "@/components/study-user-provider";
import { ErrorState, LoadingState } from "@/components/ui/async-state";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { apiRequest, formatApiError, meApiPath, type PerfilUsuario } from "@/lib/api";

const emptyProfile: PerfilUsuario = {
  objetivo: null,
  nivel: null,
  cidade: null,
  instituicao: null,
  bio: null,
  dataNascimento: null,
};

function localDateValue() {
  const date = new Date();
  return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, "0")}-${String(date.getDate()).padStart(2, "0")}`;
}

export default function PerfilPage() {
  const { user } = useStudyUser();
  const [profile, setProfile] = useState(emptyProfile);
  const [error, setError] = useState("");
  const [loaded, setLoaded] = useState(false);
  const [busy, setBusy] = useState(false);
  const [saved, setSaved] = useState(false);

  const load = useCallback(async () => {
    if (!user) return;
    try {
      const currentProfile = await apiRequest<PerfilUsuario>(meApiPath("/perfil"));
      setError("");
      setProfile(currentProfile);
    } catch (requestError) {
      setError(formatApiError(requestError));
    } finally {
      setLoaded(true);
    }
  }, [user]);

  useEffect(() => { void load(); }, [load]);

  async function save(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setBusy(true);
    setSaved(false);
    setError("");
    try {
      const updatedProfile = await apiRequest<PerfilUsuario>(meApiPath("/perfil"), {
        method: "PUT",
        body: JSON.stringify(profile),
      });
      setProfile(updatedProfile);
      setSaved(true);
    } catch (requestError) {
      setError(formatApiError(requestError));
    } finally {
      setBusy(false);
    }
  }

  return (
    <AppShell title="Perfil" subtitle="Gerencie as informações complementares da sua conta.">
      {error ? <ErrorState message={error} onRetry={() => void load()} /> : null}
      {!loaded && !error ? <LoadingState label="Carregando perfil..." /> : null}
      {loaded ? (
        <form onSubmit={save} className="space-y-5">
          <Card className="border-slate-200">
            <CardHeader>
              <CardTitle>Dados da conta</CardTitle>
              <CardDescription>Nome e e-mail associados à sessão autenticada.</CardDescription>
            </CardHeader>
            <CardContent className="grid gap-4 md:grid-cols-2">
              <div className="rounded-2xl bg-slate-50 p-4">
                <p className="text-sm text-slate-500">Nome</p>
                <p className="mt-1 font-medium text-slate-900">{user?.nome}</p>
              </div>
              <div className="rounded-2xl bg-slate-50 p-4">
                <p className="text-sm text-slate-500">E-mail</p>
                <p className="mt-1 font-medium text-slate-900">{user?.email}</p>
              </div>
            </CardContent>
          </Card>

          <Card className="border-slate-200">
            <CardHeader>
              <CardTitle>Informações pessoais e de estudo</CardTitle>
              <CardDescription>Todos os campos são opcionais.</CardDescription>
            </CardHeader>
            <CardContent className="grid gap-4 md:grid-cols-2">
              <label className="grid gap-2 text-sm text-slate-600">
                Objetivo de estudo
                <input maxLength={300} value={profile.objetivo ?? ""} onChange={(event) => setProfile({ ...profile, objetivo: event.target.value || null })} className="rounded-2xl border border-slate-200 px-4 py-3 text-slate-900" />
              </label>
              <label className="grid gap-2 text-sm text-slate-600">
                Nível
                <input maxLength={50} value={profile.nivel ?? ""} onChange={(event) => setProfile({ ...profile, nivel: event.target.value || null })} className="rounded-2xl border border-slate-200 px-4 py-3 text-slate-900" />
              </label>
              <label className="grid gap-2 text-sm text-slate-600">
                Cidade
                <input maxLength={120} value={profile.cidade ?? ""} onChange={(event) => setProfile({ ...profile, cidade: event.target.value || null })} className="rounded-2xl border border-slate-200 px-4 py-3 text-slate-900" />
              </label>
              <label className="grid gap-2 text-sm text-slate-600">
                Instituição
                <input maxLength={200} value={profile.instituicao ?? ""} onChange={(event) => setProfile({ ...profile, instituicao: event.target.value || null })} className="rounded-2xl border border-slate-200 px-4 py-3 text-slate-900" />
              </label>
              <label className="grid gap-2 text-sm text-slate-600">
                Data de nascimento
                <input type="date" max={localDateValue()} value={profile.dataNascimento ?? ""} onChange={(event) => setProfile({ ...profile, dataNascimento: event.target.value || null })} className="rounded-2xl border border-slate-200 px-4 py-3 text-slate-900" />
              </label>
              <label className="grid gap-2 text-sm text-slate-600 md:col-span-2">
                Sobre mim
                <textarea maxLength={2000} rows={4} value={profile.bio ?? ""} onChange={(event) => setProfile({ ...profile, bio: event.target.value || null })} className="rounded-2xl border border-slate-200 px-4 py-3 text-slate-900" />
              </label>
            </CardContent>
          </Card>

          <div className="flex items-center gap-4">
            <button disabled={busy} className="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-5 py-3 text-sm font-medium text-white disabled:opacity-60">
              <Save className="h-4 w-4" />{busy ? "Salvando..." : "Salvar perfil"}
            </button>
            {saved ? <span role="status" className="text-sm text-emerald-700">Perfil salvo.</span> : null}
          </div>
        </form>
      ) : null}
    </AppShell>
  );
}
