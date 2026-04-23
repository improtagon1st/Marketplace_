import { Listbox, ListboxButton, ListboxOption, ListboxOptions } from '@headlessui/react'
import { CheckIcon, ChevronDownIcon } from '@heroicons/react/24/solid'
import { useQuery } from '@tanstack/react-query'
import { useMemo } from 'react'
import { useSearchParams } from 'react-router-dom'
import { getCategories } from '../entities/category/api/categoriesApi'
import { getProducts } from '../entities/product/api/productsApi'
import { useAddToCart } from '../features/cart/lib/useAddToCart'
import { EmptyState } from '../shared/ui/EmptyState'
import { Input } from '../shared/ui/Input'
import { Loader } from '../shared/ui/Loader'
import { Pagination } from '../shared/ui/Pagination'
import { ProductCard } from '../shared/ui/ProductCard'
import { SectionTitle } from '../widgets/SectionTitle'

const sortOptions = [
  { value: 'default', label: 'По умолчанию' },
  { value: 'price_asc', label: 'Сначала дешевле' },
  { value: 'price_desc', label: 'Сначала дороже' },
  { value: 'name_asc', label: 'По названию (А-Я)' },
  { value: 'name_desc', label: 'По названию (Я-А)' },
] as const

const ITEMS_PER_PAGE = 12

export function CatalogPage() {
  const [searchParams, setSearchParams] = useSearchParams()
  const { addProduct } = useAddToCart()

  const categoryIdRaw = searchParams.get('categoryId')
  const categoryId = categoryIdRaw ? Number(categoryIdRaw) : undefined
  const search = searchParams.get('search') ?? ''
  const sort = (searchParams.get('sort') ?? 'default') as (typeof sortOptions)[number]['value']
  const page = Number(searchParams.get('page') ?? '1')

  const { data: categories } = useQuery({
    queryKey: ['categories'],
    queryFn: getCategories,
  })

  const { data: products, isLoading } = useQuery({
    queryKey: ['products', 'catalog', categoryId, search],
    queryFn: () =>
      getProducts({
        categoryId: Number.isFinite(categoryId) ? categoryId : undefined,
        search,
      }),
  })

  const sortedProducts = useMemo(() => {
    const list = [...(products ?? [])]
    switch (sort) {
      case 'price_asc':
        return list.sort((a, b) => a.price - b.price)
      case 'price_desc':
        return list.sort((a, b) => b.price - a.price)
      case 'name_asc':
        return list.sort((a, b) => a.name.localeCompare(b.name, 'ru'))
      case 'name_desc':
        return list.sort((a, b) => b.name.localeCompare(a.name, 'ru'))
      default:
        return list
    }
  }, [products, sort])

  const totalPages = Math.max(1, Math.ceil(sortedProducts.length / ITEMS_PER_PAGE))
  const currentPage = Math.min(Math.max(page, 1), totalPages)
  const pagedProducts = sortedProducts.slice(
    (currentPage - 1) * ITEMS_PER_PAGE,
    currentPage * ITEMS_PER_PAGE,
  )

  const selectedSort = sortOptions.find((option) => option.value === sort) ?? sortOptions[0]

  const updateParam = (patch: Record<string, string | undefined>) => {
    const next = new URLSearchParams(searchParams)

    Object.entries(patch).forEach(([key, value]) => {
      if (!value) {
        next.delete(key)
      } else {
        next.set(key, value)
      }
    })

    if (!patch.page) {
      next.set('page', '1')
    }

    setSearchParams(next)
  }

  return (
    <div className="space-y-6">
      <SectionTitle
        eyebrow="Каталог"
        title="Подберите товары под вашу задачу"
        subtitle="Фильтрация и сортировка выполняются в веб-клиенте поверх API"
      />

      <div className="grid gap-3 rounded-2xl border border-slate-200 bg-white p-4 md:grid-cols-[1fr_auto_auto] md:items-end">
        <Input
          label="Поиск"
          value={search}
          onChange={(event) => updateParam({ search: event.target.value || undefined })}
          placeholder="Например, наушники"
        />

        <label className="block space-y-1.5">
          <span className="text-sm font-medium text-slate-700">Категория</span>
          <select
            className="w-full rounded-xl border border-slate-300 px-3 py-2.5 text-sm"
            value={categoryIdRaw ?? ''}
            onChange={(event) => updateParam({ categoryId: event.target.value || undefined })}
          >
            <option value="">Все категории</option>
            {(categories ?? []).map((category) => (
              <option value={String(category.id)} key={category.id}>
                {category.name}
              </option>
            ))}
          </select>
        </label>

        <label className="block space-y-1.5">
          <span className="text-sm font-medium text-slate-700">Сортировка</span>
          <Listbox value={selectedSort} onChange={(option) => updateParam({ sort: option.value })}>
            <div className="relative">
              <ListboxButton className="flex w-64 items-center justify-between rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm">
                {selectedSort.label}
                <ChevronDownIcon className="h-4 w-4 text-slate-500" />
              </ListboxButton>
              <ListboxOptions className="absolute z-10 mt-1 max-h-64 w-full overflow-auto rounded-xl border border-slate-200 bg-white p-1 shadow-card">
                {sortOptions.map((option) => (
                  <ListboxOption
                    key={option.value}
                    value={option}
                    className="group flex cursor-pointer items-center justify-between rounded-lg px-3 py-2 text-sm data-[focus]:bg-slate-100"
                  >
                    <span>{option.label}</span>
                    <CheckIcon className="invisible h-4 w-4 text-mint group-data-[selected]:visible" />
                  </ListboxOption>
                ))}
              </ListboxOptions>
            </div>
          </Listbox>
        </label>
      </div>

      {isLoading ? (
        <Loader label="Загружаем каталог..." />
      ) : pagedProducts.length === 0 ? (
        <EmptyState title="Товары не найдены" description="Смените фильтры или попробуйте другой запрос." />
      ) : (
        <>
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
            {pagedProducts.map((product) => (
              <ProductCard key={product.id} product={product} onAddToCart={addProduct} />
            ))}
          </div>
          <Pagination
            page={currentPage}
            totalPages={totalPages}
            onChange={(nextPage) => updateParam({ page: String(nextPage) })}
          />
        </>
      )}
    </div>
  )
}
