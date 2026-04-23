export interface AuthUser {
  userId: string
  email: string
  fullName: string
  role: string
  pickupPointId?: number | null
}

export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  token: string
  userId: string
  email: string
  fullName: string
  role: string
  pickupPointId?: number | null
}

export interface RegisterRequest {
  email: string
  password: string
  fullName: string
  phone: string
}

export interface Category {
  id: number
  name: string
  description?: string | null
}

export interface Product {
  id: number
  name: string
  description?: string | null
  price: number
  stock: number
  categoryId: number
  categoryName: string
  imageUrl?: string | null
}

export interface CartItem {
  id: number
  productId: number
  productName: string
  productImage?: string | null
  productPrice: number
  quantity: number
  totalPrice: number
  availableStock: number
}

export interface GuestCartItem {
  productId: number
  productName: string
  productImage?: string | null
  productPrice: number
  quantity: number
  availableStock: number
}

export interface AddToCartRequest {
  productId: number
  quantity: number
}

export interface PickupPoint {
  id: number
  name: string
  address: string
  phone: string
  workingHours: string
}

export interface CheckoutResponse {
  orderId?: number
  OrderId?: number
  qrCode?: string
  QRCode?: string
  message?: string
  Message?: string
}

export interface OrderItem {
  productName: string
  quantity: number
  priceAtOrder: number
}

export interface Order {
  id: number
  customerName: string
  customerPhone: string
  pickupPointId: number
  pickupPointName: string
  pickupPointAddress: string
  totalPrice: number
  status: 'Created' | 'Delivered' | 'PickedUp' | string
  qrCode: string
  createdAt: string
  deliveredAt?: string | null
  pickedUpAt?: string | null
  items: OrderItem[]
}
