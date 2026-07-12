"use client";

import { ResponsiveContainer, AreaChart, Area, XAxis, YAxis, Tooltip, CartesianGrid } from "recharts";

const sampleData = [
  { name: "Seg", horas: 2.5 },
  { name: "Ter", horas: 3.0 },
  { name: "Qua", horas: 1.75 },
  { name: "Qui", horas: 2.25 },
  { name: "Sex", horas: 3.5 },
  { name: "Sáb", horas: 4.0 },
  { name: "Dom", horas: 1.0 },
];

export function StudyChart() {
  return (
    <div className="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm shadow-slate-200/50">
      <div className="mb-4 flex items-center justify-between">
        <div>
          <p className="text-sm uppercase tracking-[0.24em] text-slate-500">Progresso semanal</p>
          <h2 className="text-lg font-semibold text-slate-950">Horas estudadas</h2>
        </div>
        <p className="text-sm text-slate-500">Últimos 7 dias</p>
      </div>
      <div className="h-72">
        <ResponsiveContainer width="100%" height="100%">
          <AreaChart data={sampleData} margin={{ top: 12, right: 8, left: -16, bottom: 0 }}>
            <defs>
              <linearGradient id="studyGradient" x1="0" y1="0" x2="0" y2="1">
                <stop offset="0%" stopColor="#2563eb" stopOpacity={0.25} />
                <stop offset="100%" stopColor="#2563eb" stopOpacity={0} />
              </linearGradient>
            </defs>
            <CartesianGrid stroke="#e2e8f0" strokeDasharray="3 3" vertical={false} />
            <XAxis dataKey="name" axisLine={false} tickLine={false} tick={{ fill: "#64748b", fontSize: 12 }} />
            <YAxis axisLine={false} tickLine={false} tick={{ fill: "#64748b", fontSize: 12 }} />
            <Tooltip contentStyle={{ borderRadius: 16, border: "1px solid #e2e8f0" }} />
            <Area type="monotone" dataKey="horas" stroke="#2563eb" fill="url(#studyGradient)" strokeWidth={3} />
          </AreaChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
}
