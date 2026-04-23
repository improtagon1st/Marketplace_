import axios, { AxiosError } from 'axios'
import { AUTH_STORAGE_KEY, AUTH_TOKEN_KEY } from '../constants/storage'

const baseURL = import.meta.env.VITE_API_BASE_URL ?? 'https://localhost:7093/api'

export const api = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
})

api.interceptors.request.use((config) => {
  const token = localStorage.getItem(AUTH_TOKEN_KEY)
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  (response) => response,
  (error: AxiosError<{ message?: string }>) => {
    if (error.response?.status === 401) {
      localStorage.removeItem(AUTH_TOKEN_KEY)
      localStorage.removeItem(AUTH_STORAGE_KEY)
      const isLoginPage = window.location.pathname === '/login'

      if (!isLoginPage) {
        const redirect = `${window.location.pathname}${window.location.search}`
        window.location.href = `/login?redirect=${encodeURIComponent(redirect)}`
      }
    }

    return Promise.reject(error)
  },
)

export function getApiErrorMessage(error: unknown, fallback = 'Произошла ошибка') {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data
    if (typeof data === 'string') {
      return data
    }
    if (data?.message) {
      return data.message
    }
  }

  if (error instanceof Error) {
    return error.message
  }

  return fallback
}
