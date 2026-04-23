import { useMutation } from '@tanstack/react-query'
import { type FormEvent, useMemo, useState } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router-dom'
import { queryClient } from '../app/providers'
import { login } from '../features/auth/api/authApi'
import { useAuthStore } from '../features/auth/store/authStore'
import { useCartStore } from '../features/cart/store/cartStore'
import { getApiErrorMessage } from '../shared/api/http'
import { Button } from '../shared/ui/Button'
import { Input } from '../shared/ui/Input'
import { toast } from '../shared/ui/Toast'

export function LoginPage() {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')

  const setSession = useAuthStore((state) => state.setSession)
  const logout = useAuthStore((state) => state.logout)
  const mergeGuestCartToServer = useCartStore((state) => state.mergeGuestCartToServer)

  const redirect = useMemo(() => searchParams.get('redirect') || '/catalog', [searchParams])

  const mutation = useMutation({
    mutationFn: login,
    onSuccess: async (response) => {
      setSession(response)

      if (response.role !== 'Customer') {
        logout()
        toast.error('Для ролей Admin и PickupPointWorker используйте WPF-клиент.')
        return
      }

      const mergeErrors = await mergeGuestCartToServer()
      if (mergeErrors.length > 0) {
        toast.error(mergeErrors.join('; '))
      }

      queryClient.invalidateQueries({ queryKey: ['cart'] })
      queryClient.invalidateQueries({ queryKey: ['cart', 'count'] })
      toast.success('Вход выполнен успешно')
      navigate(redirect, { replace: true })
    },
    onError: (error) => {
      toast.error(getApiErrorMessage(error, 'Не удалось выполнить вход'))
    },
  })

  const onSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    mutation.mutate({ email: email.trim(), password })
  }

  return (
    <div className="mx-auto w-full max-w-md rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
      <h1 className="text-2xl font-extrabold text-ink">Вход</h1>
      <p className="mt-1 text-sm text-slate-600">Авторизуйтесь, чтобы оформлять и отслеживать заказы.</p>

      <form className="mt-6 space-y-4" onSubmit={onSubmit}>
        <Input label="Email" type="email" value={email} onChange={(event) => setEmail(event.target.value)} required />
        <Input label="Пароль" type="password" value={password} onChange={(event) => setPassword(event.target.value)} required />
        <Button type="submit" variant="primary" fullWidth disabled={mutation.isPending}>
          {mutation.isPending ? 'Входим...' : 'Войти'}
        </Button>
      </form>

      <p className="mt-4 text-sm text-slate-600">
        Нет аккаунта?{' '}
        <Link to="/register" className="font-semibold text-ink hover:text-slate-700">
          Зарегистрироваться
        </Link>
      </p>
    </div>
  )
}
