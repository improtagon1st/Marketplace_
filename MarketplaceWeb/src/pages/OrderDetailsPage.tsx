import { useQuery } from '@tanstack/react-query'
import { useEffect, useMemo } from 'react'
import { Link, useParams } from 'react-router-dom'
import { getOrderById, getOrderQrBlob } from '../entities/order/api/ordersApi'
import { formatCurrency, formatDate } from '../shared/lib/format'
import { Badge } from '../shared/ui/Badge'
import { Loader } from '../shared/ui/Loader'

export function OrderDetailsPage() {
  const params = useParams()
  const id = Number(params.id)

  const { data: order, isLoading } = useQuery({
    queryKey: ['order', id],
    queryFn: () => getOrderById(id),
    enabled: Number.isFinite(id),
  })

  const { data: qrBlob } = useQuery({
    queryKey: ['order', id, 'qr'],
    queryFn: () => getOrderQrBlob(id),
    enabled: Number.isFinite(id),
  })

  const qrUrl = useMemo(() => {
    if (!qrBlob) return null
    return URL.createObjectURL(qrBlob)
  }, [qrBlob])

  useEffect(() => {
    return () => {
      if (qrUrl) {
        URL.revokeObjectURL(qrUrl)
      }
    }
  }, [qrUrl])

  if (!Number.isFinite(id)) {
    return <p className="text-sm text-red-600">Некорректный номер заказа.</p>
  }

  if (isLoading) {
    return <Loader label="Загружаем детали заказа..." />
  }

  if (!order) {
    return <p className="text-sm text-red-600">Заказ не найден.</p>
  }

  return (
    <div className="space-y-5 rounded-3xl border border-slate-200 bg-white p-6">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h1 className="text-2xl font-extrabold text-ink">Заказ #{order.id}</h1>
          <p className="text-sm text-slate-600">Создан: {formatDate(order.createdAt)}</p>
        </div>
        <Badge status={order.status} />
      </div>

      <div className="grid gap-4 md:grid-cols-2">
        <div className="space-y-2 rounded-2xl border border-slate-200 p-4">
          <h2 className="text-base font-bold text-ink">Состав заказа</h2>
          {order.items.map((item, index) => (
            <div key={`${item.productName}-${index}`} className="flex items-center justify-between text-sm text-slate-700">
              <span>
                {item.productName} x {item.quantity}
              </span>
              <strong>{formatCurrency(item.priceAtOrder * item.quantity)}</strong>
            </div>
          ))}
          <div className="border-t border-slate-200 pt-2 text-base font-bold">Итого: {formatCurrency(order.totalPrice)}</div>
        </div>

        <div className="space-y-3 rounded-2xl border border-slate-200 p-4">
          <h2 className="text-base font-bold text-ink">Получение</h2>
          <p className="text-sm text-slate-700">
            ПВЗ: <strong>{order.pickupPointName}</strong>
          </p>
          <p className="text-sm text-slate-700">Адрес: {order.pickupPointAddress}</p>
          <p className="text-sm text-slate-700">
            Код получения: <strong>{order.qrCode}</strong>
          </p>
          {qrUrl ? <img src={qrUrl} alt={`QR заказа ${order.id}`} className="mt-2 h-44 w-44 rounded-xl border border-slate-200" /> : null}
        </div>
      </div>

      <Link to="/orders" className="text-sm font-semibold text-ink hover:text-slate-700">
        Вернуться к списку заказов
      </Link>
    </div>
  )
}
