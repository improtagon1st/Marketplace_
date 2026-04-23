import { useMutation, useQuery } from '@tanstack/react-query'
import { useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { queryClient } from '../app/providers'
import { checkoutCart, getCart } from '../entities/cart/api/cartApi'
import { getPickupPoints } from '../entities/pickup/api/pickupApi'
import { formatCurrency } from '../shared/lib/format'
import { Button } from '../shared/ui/Button'
import { EmptyState } from '../shared/ui/EmptyState'
import { Loader } from '../shared/ui/Loader'
import { SafeImage } from '../shared/ui/SafeImage'
import { toast } from '../shared/ui/Toast'

export function CheckoutPage() {
  const navigate = useNavigate()
  const [pickupPointId, setPickupPointId] = useState<number | null>(null)
  const [isOrderCreated, setIsOrderCreated] = useState(false)
  const [createdOrderId, setCreatedOrderId] = useState<number | null>(null)
  const [createdQrCode, setCreatedQrCode] = useState<string>('')

  const { data: cart, isLoading: cartLoading } = useQuery({
    queryKey: ['cart'],
    queryFn: getCart,
  })

  const { data: pickupPoints, isLoading: pickupLoading } = useQuery({
    queryKey: ['pickupPoints'],
    queryFn: getPickupPoints,
  })

  useEffect(() => {
    if (!pickupPointId && pickupPoints && pickupPoints.length > 0) {
      setPickupPointId(pickupPoints[0].id)
    }
  }, [pickupPointId, pickupPoints])

  const mutation = useMutation({
    mutationFn: checkoutCart,
    onSuccess: (response) => {
      const orderId = response.orderId ?? response.OrderId ?? null
      const qrCode = response.qrCode ?? response.QRCode ?? ''

      setCreatedOrderId(orderId)
      setCreatedQrCode(qrCode)
      setIsOrderCreated(true)
      toast.success('Заказ успешно оформлен')

      queryClient.invalidateQueries({ queryKey: ['cart'] })
      queryClient.invalidateQueries({ queryKey: ['cart', 'count'] })
      queryClient.invalidateQueries({ queryKey: ['orders'] })
    },
    onError: () => {
      toast.error('Не удалось оформить заказ')
    },
  })

  const total = useMemo(() => {
    return (cart ?? []).reduce((sum, item) => sum + item.totalPrice, 0)
  }, [cart])

  if (cartLoading || pickupLoading) {
    return <Loader label="Подготавливаем оформление заказа..." />
  }

  if (isOrderCreated) {
    return (
      <div className="mx-auto max-w-2xl rounded-3xl border border-emerald-200 bg-white p-6 text-center shadow-sm">
        <div className="mx-auto flex h-14 w-14 items-center justify-center rounded-full bg-emerald-100 text-2xl font-extrabold text-emerald-700">
          ✓
        </div>
        <h1 className="mt-4 text-3xl font-extrabold text-ink">Заказ успешно оформлен</h1>
        <p className="mt-2 text-sm leading-relaxed text-slate-600">
          Мы создали заказ и передали его в обработку. Оплата выполняется при получении в выбранном ПВЗ.
        </p>

        <div className="mx-auto mt-5 max-w-md rounded-2xl border border-slate-200 bg-slate-50 p-4 text-left">
          <div className="flex items-center justify-between gap-4 border-b border-slate-200 pb-3">
            <span className="text-sm text-slate-600">Номер заказа</span>
            <strong className="text-sm text-ink">{createdOrderId ?? '—'}</strong>
          </div>
          <div className="flex items-center justify-between gap-4 pt-3">
            <span className="text-sm text-slate-600">Код получения</span>
            <strong className="text-sm text-ink">{createdQrCode || '—'}</strong>
          </div>
        </div>

        <div className="mt-6 flex flex-col gap-2 sm:flex-row sm:justify-center">
          {createdOrderId ? (
            <Button variant="primary" onClick={() => navigate(`/orders/${createdOrderId}`)}>
              Показать QR-код
            </Button>
          ) : null}
          <Button variant="secondary" onClick={() => navigate('/orders')}>
            Перейти к заказам
          </Button>
          <Button variant="ghost" onClick={() => navigate('/catalog')}>
            Вернуться в каталог
          </Button>
        </div>
      </div>
    )
  }

  if (!cart || cart.length === 0) {
    return <EmptyState title="Корзина пуста" description="Добавьте товары, чтобы перейти к оформлению." />
  }

  return (
      <div className="mx-auto max-w-4xl space-y-5 rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
        <div>
          <h1 className="text-3xl font-extrabold text-ink">Оформление заказа</h1>
          <p className="mt-1 text-sm text-slate-600">Проверьте товары и выберите пункт выдачи.</p>
        </div>

        <div className="space-y-3 rounded-2xl border border-slate-200 bg-slate-50 p-4">
          {cart.map((item) => (
            <div key={item.id} className="flex items-center justify-between gap-4 rounded-xl bg-white p-3 text-sm">
              <div className="flex min-w-0 items-center gap-3">
                <div className="flex h-14 w-14 shrink-0 items-center justify-center rounded-lg border border-slate-100 bg-white p-1.5">
                  <SafeImage
                    src={item.productImage}
                    alt={item.productName}
                    className="max-h-full max-w-full object-contain"
                    fallbackClassName="h-full w-full rounded-md bg-slate-100 text-xs"
                    fallbackText="Нет фото"
                  />
                </div>
                <div className="min-w-0">
                  <p className="line-clamp-2 font-semibold text-ink">{item.productName}</p>
                  <p className="mt-0.5 text-xs text-slate-500">{item.quantity} шт. по {formatCurrency(item.productPrice)}</p>
                </div>
              </div>
              <strong className="shrink-0 text-ink">{formatCurrency(item.totalPrice)}</strong>
            </div>
          ))}
          <div className="mt-3 flex items-center justify-between border-t border-slate-200 pt-3 text-lg font-extrabold text-ink">
            <span>Итого</span>
            <span>{formatCurrency(total)}</span>
          </div>
        </div>

        <label className="block space-y-1.5">
          <span className="text-sm font-medium text-slate-700">Пункт выдачи</span>
          <select
            className="w-full rounded-xl border border-slate-300 px-3 py-2.5 text-sm"
            value={pickupPointId ?? ''}
            onChange={(event) => setPickupPointId(Number(event.target.value))}
          >
            {(pickupPoints ?? []).map((point) => (
              <option key={point.id} value={point.id}>
                {point.name} - {point.address}
              </option>
            ))}
          </select>
        </label>

        <div className="rounded-2xl border border-emerald-200 bg-emerald-50 p-4 text-sm text-emerald-900">
          Оплата выполняется при получении заказа в ПВЗ.
        </div>

        <div className="flex flex-wrap gap-2">
          <Button
            variant="secondary"
            onClick={() => pickupPointId && mutation.mutate(pickupPointId)}
            disabled={!pickupPointId || mutation.isPending}
          >
            {mutation.isPending ? 'Оформляем...' : 'Подтвердить заказ'}
          </Button>
          <Button variant="ghost" onClick={() => navigate('/cart')}>
            Вернуться в корзину
          </Button>
        </div>
      </div>
  )
}
