import { api } from '../../../shared/api/http'
import type { Product } from '../../../shared/types/api'

export async function getProducts(params?: { categoryId?: number; search?: string }) {
  const { data } = await api.get<Product[]>('/products', {
    params: {
      categoryId: params?.categoryId,
      search: params?.search,
    },
  })
  return data
}

export async function getProduct(id: number) {
  const { data } = await api.get<Product>(`/products/${id}`)
  return data
}
