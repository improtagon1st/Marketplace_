import { create } from 'zustand'
import { persist, createJSONStorage } from 'zustand/middleware'
import type { AuthUser, LoginResponse } from '../../../shared/types/api'
import { AUTH_STORAGE_KEY, AUTH_TOKEN_KEY } from '../../../shared/constants/storage'

interface AuthState {
  token: string | null
  user: AuthUser | null
  isAuthenticated: boolean
  setSession: (response: LoginResponse) => void
  logout: () => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      token: localStorage.getItem(AUTH_TOKEN_KEY),
      user: null,
      isAuthenticated: !!localStorage.getItem(AUTH_TOKEN_KEY),
      setSession: (response) => {
        localStorage.setItem(AUTH_TOKEN_KEY, response.token)
        set({
          token: response.token,
          isAuthenticated: true,
          user: {
            userId: response.userId,
            email: response.email,
            fullName: response.fullName,
            role: response.role,
            pickupPointId: response.pickupPointId,
          },
        })
      },
      logout: () => {
        localStorage.removeItem(AUTH_TOKEN_KEY)
        set({ token: null, user: null, isAuthenticated: false })
      },
    }),
    {
      name: AUTH_STORAGE_KEY,
      storage: createJSONStorage(() => localStorage),
      partialize: (state) => ({
        token: state.token,
        user: state.user,
        isAuthenticated: state.isAuthenticated,
      }),
    },
  ),
)
