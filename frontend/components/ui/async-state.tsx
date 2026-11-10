export function LoadingState({ label = "Carregando dados..." }: { label?: string }) {
  return <p className="rounded-3xl border border-slate-200 bg-white p-6 text-sm text-slate-600">{label}</p>;
}

export function ErrorState({ message, onRetry }: { message: string; onRetry?: () => void }) {
  return (
    <div role="alert" className="rounded-3xl border border-red-200 bg-red-50 p-5 text-sm text-red-800">
      <p>{message}</p>
      {onRetry ? (
        <button type="button" onClick={onRetry} className="mt-3 font-semibold underline">
          Tentar novamente
        </button>
      ) : null}
    </div>
  );
}

export function EmptyState({ message }: { message: string }) {
  return <p className="rounded-3xl border border-dashed border-slate-300 bg-white p-8 text-center text-sm text-slate-500">{message}</p>;
}
