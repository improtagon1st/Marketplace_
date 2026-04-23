import { api } from '../../../shared/api/http'
import type { AddToCartRequest, CartItem, CheckoutResponse } from '../../../shared/types/api'

export async function getCart() {
  const { data } = await api.get<CartItem[]>('/cart')
  return data
}

export async function addToCart(payload: AddToCartRequest) {
  const { data } = await api.post<string>('/cart', payload)
  return data
}

export async function updateCartItem(id: number, quantity: number) {
  const { data } = await api.put<string>(`/cart/${id}`, { quantity })
  return data
}

export async function removeCartItem(id: number) {
  const { data } = await api.delete<string>(`/cart/${id}`)
  return data
}

export async function clearCart() {
  const { data } = await api.delete<string>('/cart/clear')
  return data
}

export async function checkoutCart(pickupPointId: number) {
  const { data } = await api.post<CheckoutResponse>('/cart/checkout', { pickupPointId })
  return data
}
