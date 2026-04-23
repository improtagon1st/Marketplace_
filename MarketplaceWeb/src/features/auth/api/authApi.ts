import type { LoginRequest, LoginResponse, RegisterRequest } from '../../../shared/types/api'
import { api } from '../../../shared/api/http'

export async function login(payload: LoginRequest) {
  const { data } = await api.post<LoginResponse>('/auth/login', payload)
  return data
}

export async function register(payload: RegisterRequest) {
  const { data } = await api.post<string>('/auth/register', payload)
  return data
}
