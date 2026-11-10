export interface Usuario {
  id: string;
  nome: string;
  email: string;
  criadoEm: string;
}

export interface PerfilUsuario {
  objetivo: string | null;
  nivel: string | null;
  cidade: string | null;
  instituicao: string | null;
  bio: string | null;
  dataNascimento: string | null;
}

export interface Disciplina {
  id: string;
  nome: string;
  cor: string;
  icone: string;
  ativa: boolean;
  criadaEm: string;
  atualizadaEm: string;
}

export interface Assunto {
  id: string;
  disciplinaId: string;
  nome: string;
  descricao: string | null;
  dificuldade: "Low" | "Medium" | "High" | null;
  ordem: number;
  criadoEm: string;
  atualizadoEm: string;
}

export interface SessaoEstudo {
  id: string;
  disciplinaId: string;
  assuntoId: string | null;
  data: string;
  tempoEstudadoMinutos: number;
  questoes: number;
  acertos: number;
  erros: number;
  banca: string | null;
  observacoes: string | null;
  criadaEm: string;
}

export interface Meta {
  id: string;
  tipo: "StudyMinutes" | "Questions" | "StudySessions" | "Reviews";
  nome: string;
  quantidade: number;
  unidade: string;
  dataInicio: string;
  dataFim: string;
  ativa: boolean;
  criadaEm: string;
  atualizadaEm: string;
}

export interface Revisao {
  id: string;
  assuntoId: string;
  dataAgendada: string;
  dataConclusao: string | null;
  status: "Pending" | "Completed" | "Cancelled";
  observacoes: string | null;
  criadaEm: string;
  atualizadaEm: string;
}

export interface Dashboard {
  totalDisciplinas: number;
  totalAssuntos: number;
  totalSessoes: number;
  questoes: number;
  acertos: number;
  erros: number;
  tempoEstudadoMinutos: number;
  revisoesPendentes: number;
  ultimosSeteDias: { data: string; tempoEstudadoMinutos: number; questoes: number; acertos: number }[];
}

export interface ConfiguracaoUsuario {
  tema: string;
  idioma: string;
  notificacoesAtivadas: boolean;
  objetivoDiarioMinutos: number;
  disciplinaFocoId: string | null;
}

export class ApiError extends Error {
  constructor(
    message: string,
    readonly status: number,
  ) {
    super(message);
    this.name = "ApiError";
  }
}

const configuredApiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL?.replace(/\/$/, "");

function getApiBaseUrl() {
  if (configuredApiBaseUrl) return configuredApiBaseUrl;
  if (typeof window !== "undefined") {
    return `${window.location.protocol}//${window.location.hostname}:5259`;
  }
  return "http://localhost:5259";
}

export async function apiRequest<T>(path: string, init?: RequestInit): Promise<T> {
  let response: Response;

  try {
    const method = (init?.method ?? "GET").toUpperCase();
    const headers = new Headers(init?.headers);
    headers.set("Accept", "application/json");
    if (init?.body) headers.set("Content-Type", "application/json");
    if (["POST", "PUT", "PATCH", "DELETE"].includes(method)) {
      headers.set("X-AIStudyLab-Request", "1");
    }

    response = await fetch(`${getApiBaseUrl()}${path}`, {
      ...init,
      headers,
      credentials: "include",
      cache: "no-store",
    });
  } catch {
    throw new ApiError("Não foi possível conectar à API. Verifique se o backend está em execução.", 0);
  }

  if (!response.ok) {
    let message = `A API retornou erro ${response.status}.`;

    try {
      const problem = (await response.json()) as { detail?: string; title?: string };
      message = problem.detail ?? problem.title ?? message;
    } catch {
      // Responses such as 204 or plain-text errors have no Problem Details body.
    }

    throw new ApiError(message, response.status);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

export function meApiPath(suffix = ""): string {
  return `/api/me${suffix}`;
}

export function formatApiError(error: unknown): string {
  return error instanceof Error ? error.message : "Ocorreu um erro inesperado.";
}
