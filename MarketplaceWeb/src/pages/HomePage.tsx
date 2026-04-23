import { useQuery } from '@tanstack/react-query'
import { type FormEvent, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { getCategories } from '../entities/category/api/categoriesApi'
import { getProducts } from '../entities/product/api/productsApi'
import { useAddToCart } from '../features/cart/lib/useAddToCart'
import { Button } from '../shared/ui/Button'
import { Input } from '../shared/ui/Input'
import { Loader } from '../shared/ui/Loader'
import { ProductCard } from '../shared/ui/ProductCard'
import { SectionTitle } from '../widgets/SectionTitle'

export function HomePage() {
  const [search, setSearch] = useState('')
  const navigate = useNavigate()
  const { addProduct } = useAddToCart()

  const { data: categories, isLoading: categoriesLoading } = useQuery({
    queryKey: ['categories'],
    queryFn: getCategories,
  })

  const { data: products, isLoading: productsLoading } = useQuery({
    queryKey: ['products', 'home'],
    queryFn: () => getProducts(),
  })

  const handleSearch = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    const params = new URLSearchParams()
    if (search.trim()) params.set('search', search.trim())
    navigate(`/catalog?${params.toString()}`)
  }

  return (
    <div className="space-y-10">
      <section className="overflow-hidden rounded-3xl border border-slate-200 bg-gradient-to-br from-slate-900 via-slate-800 to-slate-700 px-6 py-10 text-white md:px-10">
        <p className="text-xs uppercase tracking-[0.2em] text-slate-300">Онлайн-маркетплейс</p>
        <h1 className="mt-3 max-w-3xl text-3xl font-extrabold leading-tight md:text-5xl">
          Покупайте любимые товары с выдачей в удобном ПВЗ
        </h1>
        <p className="mt-4 max-w-2xl text-sm text-slate-200 md:text-base">
          Быстрый выбор, прозрачные цены и заказ в несколько кликов. Оплата при получении в пункте выдачи.
        </p>

        <form onSubmit={handleSearch} className="mt-6 flex flex-col gap-3 sm:flex-row">
          <Input
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Найти товар по названию"
            className="border-slate-300/50"
          />
          <Button type="submit" variant="secondary" className="sm:w-auto">
            Найти в каталоге
          </Button>
        </form>
      </section>

      <section>
        <SectionTitle
          eyebrow="Категории"
          title="Выберите направление"
          subtitle="Категории подтягиваются напрямую из вашего API"
        />

        {categoriesLoading ? (
          <Loader label="Загружаем категории..." />
        ) : (
          <div className="flex flex-wrap gap-2">
            {(categories ?? []).map((category) => (
              <Link
                key={category.id}
                to={`/catalog?categoryId=${category.id}`}
                className="rounded-full border border-slate-300 bg-white px-4 py-2 text-sm font-medium transition hover:border-ink hover:text-ink"
              >
                {category.name}
              </Link>
            ))}
          </div>
        )}
      </section>

      <section>
        <SectionTitle
          eyebrow="Популярное"
          title="Товары витрины"
          subtitle="Первые товары из общего каталога"
        />

        {productsLoading ? (
          <Loader label="Загружаем витрину..." />
        ) : (
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
            {(products ?? []).slice(0, 8).map((product) => (
              <ProductCard key={product.id} product={product} onAddToCart={addProduct} />
            ))}
          </div>
        )}
      </section>
    </div>
  )
}
