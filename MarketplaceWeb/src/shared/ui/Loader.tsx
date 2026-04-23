export function Loader({ label = 'Загрузка...' }: { label?: string }) {
  return (
    <div className="flex min-h-[200px] items-center justify-center">
      <div className="rounded-xl border border-slate-200 bg-white px-4 py-3 text-sm text-slate-700 shadow-sm">
        {label}
      </div>
    </div>
  )
}
