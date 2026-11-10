"use client";

import type { ReactNode } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { useStudyUser } from "@/components/study-user-provider";

export function WorkspaceContent({ children }: { children: ReactNode }) {
  const { user, hydrated, authError } = useStudyUser();
  const pathname = usePathname();

  if (pathname === "/login") {
    return <>{children}</>;
  }

  if (!hydrated) {
    return <div className="rounded-3xl border border-slate-200 bg-white p-8 text-sm text-slate-600">Verificando sessão...</div>;
  }

  if (authError) {
    return (
      <div role="alert" className="rounded-3xl border border-red-200 bg-red-50 p-8">
        <h2 className="text-xl font-semibold text-slate-950">Não foi possível verificar sua sessão</h2>
        <p className="mt-2 text-sm text-red-800">{authError}</p>
        <Link href="/login" className="mt-5 inline-flex rounded-2xl bg-slate-900 px-4 py-3 text-sm font-medium text-white">
          Ir para o login
        </Link>
      </div>
    );
  }

  if (!user) {
    return (
      <div className="rounded-3xl border border-amber-200 bg-amber-50 p-8">
        <h2 className="text-xl font-semibold text-slate-950">Entre na sua conta</h2>
        <p className="mt-2 max-w-2xl text-sm leading-6 text-slate-700">
          Entre na sua conta para acessar as telas de estudo. Sua sessão é protegida por cookie HttpOnly.
        </p>
        <Link href="/login" className="mt-5 inline-flex rounded-2xl bg-slate-900 px-4 py-3 text-sm font-medium text-white">
          Entrar ou criar conta
        </Link>
      </div>
    );
  }

  return <>{children}</>;
}
