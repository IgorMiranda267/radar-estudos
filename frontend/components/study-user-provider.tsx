"use client";

import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";
import type { ReactNode } from "react";
import { ApiError, apiRequest, type Usuario } from "@/lib/api";

interface StudyUserContextValue {
  user: Usuario | null;
  hydrated: boolean;
  authError: string | null;
  setAuthenticatedUser: (user: Usuario) => void;
  logout: () => Promise<void>;
}

const StudyUserContext = createContext<StudyUserContextValue | null>(null);

export function StudyUserProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<Usuario | null>(null);
  const [hydrated, setHydrated] = useState(false);
  const [authError, setAuthError] = useState<string | null>(null);

  useEffect(() => {
    let active = true;
    void apiRequest<Usuario>("/api/auth/me")
      .then((authenticatedUser) => {
        if (active) setUser(authenticatedUser);
      })
      .catch((error: unknown) => {
        if (active && (!(error instanceof ApiError) || error.status !== 401)) {
          setAuthError(error instanceof Error ? error.message : "Não foi possível verificar a sessão.");
        }
      })
      .finally(() => {
        if (active) setHydrated(true);
      });
    return () => { active = false; };
  }, []);

  const setAuthenticatedUser = useCallback((nextUser: Usuario) => {
    setAuthError(null);
    setUser(nextUser);
  }, []);

  const logout = useCallback(async () => {
    await apiRequest<void>("/api/auth/logout", { method: "POST" });
    setUser(null);
    setAuthError(null);
  }, []);

  const value = useMemo(
    () => ({ user, hydrated, authError, setAuthenticatedUser, logout }),
    [user, hydrated, authError, setAuthenticatedUser, logout],
  );

  return <StudyUserContext.Provider value={value}>{children}</StudyUserContext.Provider>;
}

export function useStudyUser() {
  const context = useContext(StudyUserContext);
  if (!context) {
    throw new Error("useStudyUser deve ser usado dentro de StudyUserProvider.");
  }
  return context;
}
