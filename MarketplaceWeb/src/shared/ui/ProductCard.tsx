import { Link } from 'react-router-dom'
import type { Product } from '../types/api'
import { formatCurrency } from '../lib/format'
import { Button } from './Button'
import { SafeImage } from './SafeImage'

interface ProductCardProps {
  product: Product
  onAddToCart: (product: Product) => void
}

export function ProductCard({ product, onAddToCart }: ProductCardProps) {
  const isOutOfStock = product.stock <= 0

  return (
    <article className="group overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm transition hover:-translate-y-0.5 hover:shadow-card">
      <Link to={`/product/${product.id}`} className="block">
        <div className="flex h-52 w-full items-center justify-center border-b border-slate-100 bg-white p-4">
          <SafeImage
            src={product.imageUrl}
            alt={product.name}
            className="max-h-full max-w-full object-contain transition duration-300 group-hover:scale-[1.03]"
            fallbackClassName="h-full w-full rounded-xl bg-slate-100"
            loading="lazy"
          />
        </div>
      </Link>
      <div className="space-y-3 p-4">
        <div>
          <p className="text-xs uppercase tracking-wide text-slate-500">{product.categoryName}</p>
          <Link to={`/product/${product.id}`} className="line-clamp-2 text-base font-bold text-ink hover:text-slate-700">
            {product.name}
          </Link>
        </div>
        <div className="flex items-end justify-between gap-2">
          <div>
            <p className="text-lg font-extrabold text-ink">{formatCurrency(product.price)}</p>
            <p className="text-xs text-slate-500">Остаток: {product.stock}</p>
          </div>
          <Button
            variant="secondary"
            onClick={() => onAddToCart(product)}
            disabled={isOutOfStock}
            className="shrink-0"
          >
            {isOutOfStock ? 'Нет в наличии' : 'В корзину'}
          </Button>
        </div>
      </div>
    </article>
  )
}
