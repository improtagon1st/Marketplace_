import { api } from '../../../shared/api/http'
import type { Order } from '../../../shared/types/api'

export async function getOrders() {
  const { data } = await api.get<Order[]>('/orders')
  return data
}

export async function getOrderById(id: number) {
  const { data } = await api.get<Order>(`/orders/${id}`)
  return data
}

export async function getOrderQrBlob(id: number) {
  const { data } = await api.get<Blob>(`/orders/qr/${id}`, {
    responseType: 'blob',
  })
  return data
}
