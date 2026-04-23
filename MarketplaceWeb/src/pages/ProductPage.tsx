import { useQuery } from '@tanstack/react-query'
import { useMemo, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { getProduct } from '../entities/product/api/productsApi'
import { useAddToCart } from '../features/cart/lib/useAddToCart'
import { formatCurrency } from '../shared/lib/format'
import { Button } from '../shared/ui/Button'
import { Loader } from '../shared/ui/Loader'
import { SafeImage } from '../shared/ui/SafeImage'

export function ProductPage() {
  const params = useParams()
  const id = Number(params.id)
  const [quantity, setQuantity] = useState(1)
  const { addProduct, isPending } = useAddToCart()

  const { data: product, isLoading } = useQuery({
    queryKey: ['product', id],
    queryFn: () => getProduct(id),
    enabled: Number.isFinite(id),
  })

  const maxQuantity = useMemo(() => Math.max(product?.stock ?? 1, 1), [product?.stock])

  if (!Number.isFinite(id)) {
    return <p className="text-sm text-red-600">Некорректный идентификатор товара.</p>
  }

  if (isLoading) {
    return <Loader label="Загружаем товар..." />
  }

  if (!product) {
    return <p className="text-sm text-red-600">Товар не найден.</p>
  }

  const isOutOfStock = product.stock <= 0

  return (
    <div className="grid gap-6 rounded-3xl border border-slate-200 bg-white p-6 md:grid-cols-2">
      <div className="flex min-h-80 items-center justify-center overflow-hidden rounded-2xl border border-slate-200 bg-white p-6">
        <SafeImage
          src={product.imageUrl}
          alt={product.name}
          className="max-h-[520px] max-w-full object-contain"
          fallbackClassName="h-full min-h-80 w-full rounded-xl bg-slate-100"
        />
      </div>

      <div className="space-y-5">
        <div>
          <p className="text-xs uppercase tracking-widest text-slate-500">{product.categoryName}</p>
          <h1 className="mt-1 text-3xl font-extrabold text-ink">{product.name}</h1>
        </div>

        <p className="text-sm leading-relaxed text-slate-700">
          {product.description?.trim() || 'Описание товара пока не добавлено.'}
        </p>

        <p className="text-3xl font-extrabold text-ink">{formatCurrency(product.price)}</p>
        <p className="text-sm text-slate-600">Остаток на складе: {product.stock}</p>

        <div className="flex items-end gap-3">
          <label className="space-y-1">
            <span className="text-sm font-medium text-slate-700">Количество</span>
            <input
              type="number"
              min={1}
              max={maxQuantity}
              value={quantity}
              onChange={(event) => {
                const next = Number(event.target.value)
                if (!Number.isFinite(next)) return
                setQuantity(Math.max(1, Math.min(next, maxQuantity)))
              }}
              className="w-28 rounded-xl border border-slate-300 px-3 py-2.5 text-sm"
            />
          </label>

          <Button
            variant="secondary"
            onClick={() => addProduct(product, quantity)}
            disabled={isOutOfStock || isPending}
          >
            {isOutOfStock ? 'Нет в наличии' : 'Добавить в корзину'}
          </Button>
        </div>

        <div>
          <Link to="/catalog" className="text-sm font-semibold text-slate-600 hover:text-ink">
            Вернуться в каталог
          </Link>
        </div>
      </div>
    </div>
  )
}
