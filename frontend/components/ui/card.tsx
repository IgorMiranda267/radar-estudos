import type { ReactNode } from "react";

interface CardProps {
  children: ReactNode;
  className?: string;
}

export function Card({ children, className = "" }: CardProps) {
  return (
    <div className={`rounded-3xl border border-slate-200 bg-white shadow-sm shadow-slate-200/50 p-6 ${className}`}>
      {children}
    </div>
  );
}

export function CardHeader({ children, className = "" }: CardProps) {
  return <div className={`mb-4 ${className}`}>{children}</div>;
}

export function CardTitle({ children, className = "" }: CardProps) {
  return <h2 className={`text-base font-semibold tracking-tight text-slate-900 ${className}`}>{children}</h2>;
}

export function CardDescription({ children, className = "" }: CardProps) {
  return <p className={`text-sm text-slate-500 ${className}`}>{children}</p>;
}

export function CardContent({ children, className = "" }: CardProps) {
  return <div className={`mt-1 ${className}`}>{children}</div>;
}
