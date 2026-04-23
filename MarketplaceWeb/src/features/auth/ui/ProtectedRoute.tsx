import { Navigate, useLocation } from 'react-router-dom'
import { type PropsWithChildren } from 'react'
import { useAuthStore } from '../store/authStore'

export function ProtectedRoute({ children }: PropsWithChildren) {
  const location = useLocation()
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated)
  const user = useAuthStore((state) => state.user)

  if (!isAuthenticated || !user || user.role !== 'Customer') {
    const redirect = `${location.pathname}${location.search}`
    return <Navigate to={`/login?redirect=${encodeURIComponent(redirect)}`} replace />
  }

  return children
}
