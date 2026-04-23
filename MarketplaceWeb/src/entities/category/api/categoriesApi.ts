import { api } from '../../../shared/api/http'
import type { Category } from '../../../shared/types/api'

export async function getCategories() {
  const { data } = await api.get<Category[]>('/categories')
  return data
}
