"use client";

import { FormEvent, useState } from "react";
import { ArrowRight, LockKeyhole, Mail, UserRound } from "lucide-react";
import { AppShell } from "@/components/app-shell";
import { useStudyUser } from "@/components/study-user-provider";
import { ApiError, apiRequest, formatApiError, type Usuario } from "@/lib/api";

export default function LoginPage() {
  const { user, setAuthenticatedUser, logout } = useStudyUser();
  const [mode, setMode] = useState<"register" | "login">("login");
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");
    setBusy(true);

    try {
      const account = await apiRequest<Usuario>(
        mode === "register" ? "/api/auth/register" : "/api/auth/login",
        {
          method: "POST",
          body: JSON.stringify(
            mode === "register"
              ? { nome: name, email, senha: password }
              : { email, senha: password },
          ),
        },
      );
      setAuthenticatedUser(account);
      setPassword("");
    } catch (requestError) {
      if (requestError instanceof ApiError && requestError.status === 401) {
        setError("E-mail ou senha incorretos.");
      } else {
        setError(formatApiError(requestError));
      }
    } finally {
      setBusy(false);
    }
  }

  async function handleLogout() {
    setError("");
    setBusy(true);
    try {
      await logout();
    } catch (requestError) {
      setError(formatApiError(requestError));
    } finally {
      setBusy(false);
    }
  }

  return (
    <AppShell
      title="Acesso à conta"
      subtitle="Entre ou crie sua conta para acessar seu espaço de estudos."
      allowAnonymous
    >
      <section className="grid gap-6 lg:grid-cols-[1.2fr_0.8fr]">
        <div className="rounded-[2rem] border border-slate-200 bg-white p-8 shadow-sm shadow-slate-200/40">
          {user ? (
            <div>
              <p className="text-sm uppercase tracking-[0.2em] text-emerald-700">Sessão ativa</p>
              <h2 className="mt-3 text-3xl font-semibold text-slate-950">{user.nome}</h2>
              <p className="mt-2 text-sm text-slate-600">{user.email}</p>
              {error ? <p role="alert" className="mt-4 text-sm text-red-700">{error}</p> : null}
              <button
                type="button"
                disabled={busy}
                onClick={() => void handleLogout()}
                className="mt-6 rounded-2xl border border-slate-200 px-4 py-3 text-sm font-medium text-slate-700 disabled:opacity-60"
              >
                {busy ? "Saindo..." : "Sair da conta"}
              </button>
            </div>
          ) : (
            <>
              <p className="text-sm uppercase tracking-[0.2em] text-slate-500">
                {mode === "register" ? "Nova conta" : "Bem-vindo de volta"}
              </p>
              <h2 className="mt-3 text-3xl font-semibold text-slate-950">
                {mode === "register" ? "Criar conta" : "Entrar"}
              </h2>

              <form className="mt-8 space-y-5" onSubmit={handleSubmit}>
                {mode === "register" ? (
                  <label className="block text-sm font-medium text-slate-700">
                    Nome
                    <div className="mt-2 flex items-center gap-3 rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3">
                      <UserRound className="h-4 w-4 text-slate-500" />
                      <input
                        required
                        maxLength={150}
                        value={name}
                        onChange={(event) => setName(event.target.value)}
                        placeholder="Seu nome"
                        className="w-full bg-transparent text-slate-900 outline-none placeholder:text-slate-400"
                      />
                    </div>
                  </label>
                ) : null}

                <label className="block text-sm font-medium text-slate-700">
                  E-mail
                  <div className="mt-2 flex items-center gap-3 rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3">
                    <Mail className="h-4 w-4 text-slate-500" />
                    <input
                      required
                      type="email"
                      maxLength={255}
                      value={email}
                      onChange={(event) => setEmail(event.target.value)}
                      placeholder="seu@email.com"
                      className="w-full bg-transparent text-slate-900 outline-none placeholder:text-slate-400"
                    />
                  </div>
                </label>

                <label className="block text-sm font-medium text-slate-700">
                  Senha
                  <div className="mt-2 flex items-center gap-3 rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3">
                    <LockKeyhole className="h-4 w-4 text-slate-500" />
                    <input
                      required
                      type="password"
                      minLength={mode === "register" ? 12 : 1}
                      maxLength={256}
                      value={password}
                      onChange={(event) => setPassword(event.target.value)}
                      placeholder={mode === "register" ? "Mínimo de 12 caracteres" : "Sua senha"}
                      className="w-full bg-transparent text-slate-900 outline-none placeholder:text-slate-400"
                    />
                  </div>
                </label>

                {error ? <p role="alert" className="text-sm text-red-700">{error}</p> : null}

                <button
                  disabled={busy}
                  className="flex w-full items-center justify-center gap-2 rounded-2xl bg-slate-900 px-4 py-3 text-sm font-medium text-white disabled:opacity-60"
                >
                  {busy ? "Aguarde..." : mode === "register" ? "Criar conta" : "Entrar"}
                  <ArrowRight className="h-4 w-4" />
                </button>
              </form>

              <button
                type="button"
                onClick={() => {
                  setMode(mode === "register" ? "login" : "register");
                  setError("");
                }}
                className="mt-5 text-sm font-medium text-slate-700 underline"
              >
                {mode === "register" ? "Já tenho uma conta" : "Quero criar uma conta"}
              </button>
            </>
          )}
        </div>

        <div className="rounded-[2rem] border border-sky-200 bg-sky-50 p-8">
          <p className="text-sm uppercase tracking-[0.2em] text-sky-800">Sessão protegida</p>
          <h3 className="mt-4 text-2xl font-semibold text-slate-950">Seus dados são vinculados à sua conta</h3>
          <p className="mt-4 text-sm leading-6 text-slate-700">
            A autenticação usa um cookie HttpOnly; as operações de estudo identificam o usuário pela sessão autenticada e não por um identificador enviado pelo navegador.
          </p>
        </div>
      </section>
    </AppShell>
  );
}
