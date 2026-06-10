import { create } from 'zustand'
import { persist, createJSONStorage } from 'zustand/middleware'
import { addToCart } from '../../../entities/cart/api/cartApi'
import { GUEST_CART_STORAGE_KEY } from '../../../shared/constants/storage'
import type { GuestCartItem, Product } from '../../../shared/types/api'

interface CartState {
  guestItems: GuestCartItem[]
  addGuestItem: (product: Product, quantity?: number) => void
  updateGuestQuantity: (productId: number, quantity: number) => void
  removeGuestItem: (productId: number) => void
  clearGuestCart: () => void
  mergeGuestCartToServer: () => Promise<string[]>
}

export const useCartStore = create<CartState>()(
  persist(
    (set, get) => ({
      guestItems: [],
      addGuestItem: (product, quantity = 1) => {
        set((state) => {
          const existing = state.guestItems.find((item) => item.productId === product.id)
          if (existing) {
            return {
              guestItems: state.guestItems.map((item) =>
                item.productId === product.id
                  ? {
                      ...item,
                      quantity: Math.min(item.quantity + quantity, Math.max(product.stock, 1)),
                      availableStock: product.stock,
                      productPrice: product.price,
                    }
                  : item,
              ),
            }
          }

          const nextItem: GuestCartItem = {
            productId: product.id,
            productName: product.name,
            productImage: product.imageUrl,
            productPrice: product.price,
            quantity: Math.min(quantity, Math.max(product.stock, 1)),
            availableStock: product.stock,
          }

          return {
            guestItems: [...state.guestItems, nextItem],
          }
        })
      },
      updateGuestQuantity: (productId, quantity) => {
        set((state) => ({
          guestItems: state.guestItems
            .map((item) =>
              item.productId === productId
                ? {
                    ...item,
                    quantity: Math.max(1, Math.min(quantity, Math.max(item.availableStock, 1))),
                  }
                : item,
            )
            .filter((item) => item.quantity > 0),
        }))
      },
      removeGuestItem: (productId) => {
        set((state) => ({
          guestItems: state.guestItems.filter((item) => item.productId !== productId),
        }))
      },
      clearGuestCart: () => {
        set({ guestItems: [] })
      },
      mergeGuestCartToServer: async () => {
        const items = get().guestItems
        if (!items.length) return []

        const errors: string[] = []
        for (const item of items) {
          try {
            await addToCart({ productId: item.productId, quantity: item.quantity })
          } catch {
            errors.push(`Товар "${item.productName}" не удалось добавить в корзину аккаунта`)
          }
        }

        if (errors.length === 0) {
          set({ guestItems: [] })
        }

        return errors
      },
    }),
    {
      name: GUEST_CART_STORAGE_KEY,
      storage: createJSONStorage(() => localStorage),
      partialize: (state) => ({ guestItems: state.guestItems }),
    },
  ),
)

export function getGuestCartCount(items: GuestCartItem[]) {
  return items.reduce((sum, item) => sum + item.quantity, 0)
}

export function getGuestCartTotal(items: GuestCartItem[]) {
  return items.reduce((sum, item) => sum + item.productPrice * item.quantity, 0)
}
