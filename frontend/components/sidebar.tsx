"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { BookOpen, ClipboardList, Clock3, Clock, Gauge, LayoutDashboard, ListChecks, Sparkles, UserCog, LogIn, UserRound } from "lucide-react";
import { useStudyUser } from "@/components/study-user-provider";

const navItems = [
  { label: "Dashboard", icon: LayoutDashboard, href: "/" },
  { label: "Disciplinas", icon: BookOpen, href: "/disciplinas" },
  { label: "Assuntos", icon: ClipboardList, href: "/assuntos" },
  { label: "Sessões", icon: Clock, href: "/sessao" },
  { label: "Estatísticas", icon: Gauge, href: "/estatisticas" },
  { label: "Metas", icon: Sparkles, href: "/metas" },
  { label: "Revisões", icon: ListChecks, href: "/revisoes" },
  { label: "Histórico", icon: Clock3, href: "/historico" },
  { label: "Configurações", icon: UserCog, href: "/configuracoes" },
  { label: "Perfil", icon: UserRound, href: "/perfil" },
  { label: "Conta", icon: LogIn, href: "/login" },
];

export function Sidebar() {
  const pathname = usePathname();
  const { user } = useStudyUser();

  return (
    <aside className="sticky top-6 hidden h-fit min-h-[calc(100vh-3rem)] w-full max-w-[280px] rounded-[2.5rem] border border-slate-200 bg-white p-6 shadow-sm shadow-slate-200/50 xl:block">
      <div className="mb-10">
        <p className="text-xs uppercase tracking-[0.32em] text-slate-400">Radar de Estudos</p>
        <h2 className="mt-4 text-2xl font-semibold text-slate-950">Controle de estudos</h2>
        <p className="mt-2 text-sm leading-6 text-slate-600">Acesse todas as áreas do seu acompanhamento.</p>
        {user ? <p className="mt-4 truncate text-xs font-medium text-slate-500">{user.nome}</p> : null}
      </div>
      <nav className="space-y-2">
        {navItems.map((item) => {
          const isActive = pathname === item.href;

          return (
            <Link
              key={item.label}
              href={item.href}
              className={`flex items-center gap-3 rounded-3xl px-4 py-3 text-sm font-medium transition ${
                isActive
                  ? "bg-slate-900 text-white shadow-sm"
                  : "text-slate-700 hover:bg-slate-100 hover:text-slate-950"
              }`}
            >
              <item.icon className="h-4 w-4" />
              {item.label}
            </Link>
          );
        })}
      </nav>
    </aside>
  );
}
