import type { ReactNode } from "react";
import { Sidebar } from "@/components/sidebar";
import { WorkspaceContent } from "@/components/workspace-content";

interface AppShellProps {
  title: string;
  subtitle?: string;
  action?: ReactNode;
  allowAnonymous?: boolean;
  children: ReactNode;
}

export function AppShell({ title, subtitle, action, allowAnonymous = false, children }: AppShellProps) {
  return (
    <div className="min-h-screen bg-slate-50 text-slate-950">
      <div className="grid min-h-screen grid-cols-[280px_1fr] gap-8 px-6 py-6 lg:px-10">
        <Sidebar />

        <main className="flex min-h-screen flex-col gap-6">
          <section className="flex flex-col gap-2">
            <div className="flex items-center justify-between gap-4">
              <div>
                <p className="text-sm uppercase tracking-[0.24em] text-slate-500">Radar de Estudos</p>
                <h1 className="mt-2 text-3xl font-semibold text-slate-950">{title}</h1>
              </div>
              {action ? <div>{action}</div> : null}
            </div>
            {subtitle ? <p className="max-w-2xl text-sm leading-6 text-slate-600">{subtitle}</p> : null}
          </section>

          {allowAnonymous ? children : <WorkspaceContent>{children}</WorkspaceContent>}
        </main>
      </div>
    </div>
  );
}
