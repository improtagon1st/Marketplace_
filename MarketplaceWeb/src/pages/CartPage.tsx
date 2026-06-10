import { useMutation, useQuery } from '@tanstack/react-query'
import { Link, useNavigate } from 'react-router-dom'
import { queryClient } from '../app/providers'
import { clearCart, getCart, removeCartItem, updateCartItem } from '../entities/cart/api/cartApi'
import { useAuthStore } from '../features/auth/store/authStore'
import { getGuestCartTotal, useCartStore } from '../features/cart/store/cartStore'
import { formatCurrency } from '../shared/lib/format'
import { Button } from '../shared/ui/Button'
import { EmptyState } from '../shared/ui/EmptyState'
import { Loader } from '../shared/ui/Loader'
import { SafeImage } from '../shared/ui/SafeImage'
import { toast } from '../shared/ui/Toast'

export function CartPage() {
  const navigate = useNavigate()
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated)
  const user = useAuthStore((state) => state.user)
  const isCustomer = isAuthenticated && user?.role === 'Customer'

  const guestItems = useCartStore((state) => state.guestItems)
  const updateGuestQuantity = useCartStore((state) => state.updateGuestQuantity)
  const removeGuestItem = useCartStore((state) => state.removeGuestItem)
  const clearGuestCart = useCartStore((state) => state.clearGuestCart)

  const { data: serverItems, isLoading } = useQuery({
    queryKey: ['cart'],
    queryFn: getCart,
    enabled: isCustomer,
  })

  const mutateUpdate = useMutation({
    mutationFn: ({ id, quantity }: { id: number; quantity: number }) => updateCartItem(id, quantity),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['cart'] })
      queryClient.invalidateQueries({ queryKey: ['cart', 'count'] })
    },
    onError: () => {
      toast.error('Не удалось обновить количество')
    },
  })

  const mutateRemove = useMutation({
    mutationFn: (id: number) => removeCartItem(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['cart'] })
      queryClient.invalidateQueries({ queryKey: ['cart', 'count'] })
      toast.success('Позиция удалена')
    },
    onError: () => {
      toast.error('Не удалось удалить позицию')
    },
  })

  const mutateClear = useMutation({
    mutationFn: clearCart,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['cart'] })
      queryClient.invalidateQueries({ queryKey: ['cart', 'count'] })
      toast.success('Корзина очищена')
    },
    onError: () => {
      toast.error('Не удалось очистить корзину')
    },
  })

  if (isCustomer && isLoading) {
    return <Loader label="Загружаем корзину..." />
  }

  const serverTotal = (serverItems ?? []).reduce((sum, item) => sum + item.totalPrice, 0)
  const guestTotal = getGuestCartTotal(guestItems)
  const itemsCount = isCustomer ? serverItems?.length ?? 0 : guestItems.length

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-extrabold text-ink">Корзина</h1>
        <p className="mt-1 text-sm text-slate-600">Товаров: {itemsCount}</p>
      </div>

      {itemsCount === 0 ? (
        <EmptyState title="Корзина пуста" description="Добавьте товары из каталога, чтобы оформить заказ." />
      ) : null}

      {isCustomer ? (
        <div className="grid gap-5 lg:grid-cols-[minmax(0,1fr)_360px]">
          <div className="space-y-3">
            {(serverItems ?? []).map((item) => (
              <div
                key={item.id}
                className="grid gap-4 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm md:grid-cols-[minmax(0,1fr)_auto] md:items-center"
              >
                <div className="flex min-w-0 items-center gap-4">
                  <div className="flex h-20 w-20 shrink-0 items-center justify-center rounded-xl border border-slate-100 bg-white p-2">
                    <SafeImage
                      src={item.productImage}
                      alt={item.productName}
                      className="max-h-full max-w-full object-contain"
                      fallbackClassName="h-full w-full rounded-lg bg-slate-100 text-xs"
                      fallbackText="Нет фото"
                    />
                  </div>
                  <div className="min-w-0">
                    <Link to={`/product/${item.productId}`} className="line-clamp-2 font-semibold text-ink hover:text-slate-700">
                      {item.productName}
                    </Link>
                    <p className="mt-1 text-sm text-slate-600">{formatCurrency(item.productPrice)} за шт.</p>
                    <p className="mt-1 text-xs text-slate-500">Доступно: {item.availableStock}</p>
                  </div>
                </div>

                <div className="flex items-center justify-between gap-4 md:justify-end">
                  <div className="flex items-center rounded-xl border border-slate-300 bg-white">
                    <button
                      type="button"
                      className="h-10 w-10 text-lg text-slate-700 transition hover:bg-slate-100"
                      onClick={() => mutateUpdate.mutate({ id: item.id, quantity: Math.max(item.quantity - 1, 1) })}
                    >
                      -
                    </button>
                    <span className="w-10 text-center text-sm font-semibold">{item.quantity}</span>
                    <button
                      type="button"
                      className="h-10 w-10 text-lg text-slate-700 transition hover:bg-slate-100"
                      onClick={() => mutateUpdate.mutate({ id: item.id, quantity: item.quantity + 1 })}
                    >
                      +
                    </button>
                  </div>

                  <div className="min-w-28 text-right">
                    <p className="text-lg font-bold text-ink">{formatCurrency(item.totalPrice)}</p>
                    <Button variant="ghost" onClick={() => mutateRemove.mutate(item.id)} className="mt-2 px-3 py-2">
                      Удалить
                    </Button>
                  </div>
                </div>
              </div>
            ))}
          </div>

          {(serverItems?.length ?? 0) > 0 ? (
            <aside className="h-fit rounded-2xl border border-slate-200 bg-white p-5 shadow-sm lg:sticky lg:top-24">
              <p className="text-sm font-medium text-slate-600">Итого</p>
              <p className="mt-1 text-3xl font-extrabold text-ink">{formatCurrency(serverTotal)}</p>
              <p className="mt-3 text-sm leading-relaxed text-slate-600">Оплата при получении в выбранном ПВЗ.</p>
              <div className="mt-5 space-y-2">
                <Button variant="secondary" fullWidth onClick={() => navigate('/checkout')}>
                  Оформить заказ
                </Button>
                <Button variant="ghost" fullWidth onClick={() => mutateClear.mutate()}>
                  Очистить корзину
                </Button>
              </div>
            </aside>
          ) : null}
        </div>
      ) : (
        <div className="grid gap-5 lg:grid-cols-[minmax(0,1fr)_360px]">
          <div className="space-y-3">
            {guestItems.map((item) => (
              <div
                key={item.productId}
                className="grid gap-4 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm md:grid-cols-[minmax(0,1fr)_auto] md:items-center"
              >
                <div className="flex min-w-0 items-center gap-4">
                  <div className="flex h-20 w-20 shrink-0 items-center justify-center rounded-xl border border-slate-100 bg-white p-2">
                    <SafeImage
                      src={item.productImage}
                      alt={item.productName}
                      className="max-h-full max-w-full object-contain"
                      fallbackClassName="h-full w-full rounded-lg bg-slate-100 text-xs"
                      fallbackText="Нет фото"
                    />
                  </div>
                  <div className="min-w-0">
                    <Link to={`/product/${item.productId}`} className="line-clamp-2 font-semibold text-ink hover:text-slate-700">
                      {item.productName}
                    </Link>
                    <p className="mt-1 text-sm text-slate-600">{formatCurrency(item.productPrice)} за шт.</p>
                    <p className="mt-1 text-xs text-slate-500">Доступно: {item.availableStock}</p>
                  </div>
                </div>

                <div className="flex items-center justify-between gap-4 md:justify-end">
                  <div className="flex items-center rounded-xl border border-slate-300 bg-white">
                    <button
                      type="button"
                      className="h-10 w-10 text-lg text-slate-700 transition hover:bg-slate-100"
                      onClick={() => updateGuestQuantity(item.productId, Math.max(item.quantity - 1, 1))}
                    >
                      -
                    </button>
                    <span className="w-10 text-center text-sm font-semibold">{item.quantity}</span>
                    <button
                      type="button"
                      className="h-10 w-10 text-lg text-slate-700 transition hover:bg-slate-100"
                      onClick={() => updateGuestQuantity(item.productId, item.quantity + 1)}
                    >
                      +
                    </button>
                  </div>

                  <div className="min-w-28 text-right">
                    <p className="text-lg font-bold text-ink">{formatCurrency(item.productPrice * item.quantity)}</p>
                    <Button variant="ghost" onClick={() => removeGuestItem(item.productId)} className="mt-2 px-3 py-2">
                      Удалить
                    </Button>
                  </div>
                </div>
              </div>
            ))}
          </div>

          {guestItems.length > 0 ? (
            <aside className="h-fit rounded-2xl border border-slate-200 bg-white p-5 shadow-sm lg:sticky lg:top-24">
              <p className="text-sm font-medium text-slate-600">Итого</p>
              <p className="mt-1 text-3xl font-extrabold text-ink">{formatCurrency(guestTotal)}</p>
              <p className="mt-3 text-sm leading-relaxed text-slate-600">
                Для оформления заказа нужно войти в аккаунт. После входа товары из корзины будут синхронизированы с вашим профилем.
              </p>
              <div className="mt-5 space-y-2">
                <Button variant="secondary" fullWidth onClick={() => navigate(`/login?redirect=${encodeURIComponent('/checkout')}`)}>
                  Войти и оформить
                </Button>
                <Button variant="ghost" fullWidth onClick={clearGuestCart}>
                  Очистить корзину
                </Button>
              </div>
            </aside>
          ) : null}
        </div>
      )}
    </div>
  )
}
