import { cn } from '../lib/cn'

interface BadgeProps {
  status: string
}

const statusMap: Record<string, string> = {
  Created: 'bg-blue-100 text-blue-700',
  Delivered: 'bg-amber-100 text-amber-700',
  PickedUp: 'bg-emerald-100 text-emerald-700',
}

const labels: Record<string, string> = {
  Created: 'Создан',
  Delivered: 'Доставлен в ПВЗ',
  PickedUp: 'Выдан',
}

export function Badge({ status }: BadgeProps) {
  return (
    <span
      className={cn(
        'inline-flex rounded-full px-2.5 py-1 text-xs font-semibold',
        statusMap[status] ?? 'bg-slate-200 text-slate-700',
      )}
    >
      {labels[status] ?? status}
    </span>
  )
}
