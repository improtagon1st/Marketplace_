import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { getOrders } from '../entities/order/api/ordersApi'
import { formatCurrency, formatDate } from '../shared/lib/format'
import { Badge } from '../shared/ui/Badge'
import { EmptyState } from '../shared/ui/EmptyState'
import { Loader } from '../shared/ui/Loader'

export function OrdersPage() {
  const { data: orders, isLoading } = useQuery({
    queryKey: ['orders'],
    queryFn: getOrders,
  })

  if (isLoading) {
    return <Loader label="Загружаем ваши заказы..." />
  }

  if (!orders || orders.length === 0) {
    return (
      <EmptyState
        title="У вас пока нет заказов"
        description="Оформите первый заказ в каталоге, и он появится здесь."
      />
    )
  }

  return (
    <div className="space-y-4">
      <h1 className="text-3xl font-extrabold text-ink">Мои заказы</h1>
      {orders.map((order) => (
        <article key={order.id} className="rounded-2xl border border-slate-200 bg-white p-4">
          <div className="flex flex-wrap items-center justify-between gap-3">
            <div>
              <p className="text-lg font-bold text-ink">Заказ #{order.id}</p>
              <p className="text-sm text-slate-600">Создан: {formatDate(order.createdAt)}</p>
            </div>
            <Badge status={order.status} />
          </div>

          <div className="mt-3 grid gap-2 text-sm text-slate-700 md:grid-cols-2">
            <p>
              Сумма: <strong>{formatCurrency(order.totalPrice)}</strong>
            </p>
            <p>
              ПВЗ: <strong>{order.pickupPointName}</strong>
            </p>
          </div>

          <div className="mt-4">
            <Link to={`/orders/${order.id}`} className="text-sm font-semibold text-ink hover:text-slate-700">
              Открыть детали заказа
            </Link>
          </div>
        </article>
      ))}
    </div>
  )
}
