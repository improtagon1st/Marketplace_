import { useMutation } from '@tanstack/react-query'
import { addToCart } from '../../../entities/cart/api/cartApi'
import { queryClient } from '../../../app/providers'
import { useAuthStore } from '../../auth/store/authStore'
import { useCartStore } from '../store/cartStore'
import { getApiErrorMessage } from '../../../shared/api/http'
import { toast } from '../../../shared/ui/Toast'
import type { Product } from '../../../shared/types/api'

export function useAddToCart() {
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated)
  const user = useAuthStore((state) => state.user)
  const addGuestItem = useCartStore((state) => state.addGuestItem)

  const mutation = useMutation({
    mutationFn: ({ productId, quantity }: { productId: number; quantity: number }) =>
      addToCart({ productId, quantity }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['cart'] })
      queryClient.invalidateQueries({ queryKey: ['cart', 'count'] })
    },
  })

  const addProduct = async (product: Product, quantity = 1) => {
    if (isAuthenticated && user?.role === 'Customer') {
      try {
        await mutation.mutateAsync({ productId: product.id, quantity })
        toast.success('Товар добавлен в корзину')
      } catch (error) {
        toast.error(getApiErrorMessage(error, 'Не удалось добавить товар в корзину'))
      }
      return
    }

    addGuestItem(product, quantity)
    toast.success('Товар добавлен в локальную корзину')
  }

  return {
    addProduct,
    isPending: mutation.isPending,
  }
}
