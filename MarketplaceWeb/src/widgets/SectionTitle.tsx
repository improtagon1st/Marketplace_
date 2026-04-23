interface SectionTitleProps {
  eyebrow?: string
  title: string
  subtitle?: string
}

export function SectionTitle({ eyebrow, title, subtitle }: SectionTitleProps) {
  return (
    <div className="mb-6 space-y-1">
      {eyebrow ? <p className="text-xs font-semibold uppercase tracking-widest text-sunset">{eyebrow}</p> : null}
      <h2 className="text-2xl font-extrabold text-ink md:text-3xl">{title}</h2>
      {subtitle ? <p className="text-sm text-slate-600 md:text-base">{subtitle}</p> : null}
    </div>
  )
}
