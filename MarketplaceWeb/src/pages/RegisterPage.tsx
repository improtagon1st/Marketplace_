import { useMutation } from '@tanstack/react-query'
import { type FormEvent, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { register } from '../features/auth/api/authApi'
import { getApiErrorMessage } from '../shared/api/http'
import { Button } from '../shared/ui/Button'
import { Input } from '../shared/ui/Input'
import { toast } from '../shared/ui/Toast'

export function RegisterPage() {
  const navigate = useNavigate()
  const [fullName, setFullName] = useState('')
  const [email, setEmail] = useState('')
  const [phone, setPhone] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')

  const mutation = useMutation({
    mutationFn: register,
    onSuccess: () => {
      toast.success('Регистрация успешна. Выполните вход.')
      navigate('/login')
    },
    onError: (error) => {
      toast.error(getApiErrorMessage(error, 'Не удалось зарегистрироваться'))
    },
  })

  const onSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    if (password !== confirmPassword) {
      toast.error('Пароли не совпадают')
      return
    }

    mutation.mutate({
      fullName: fullName.trim(),
      email: email.trim(),
      phone: phone.trim(),
      password,
    })
  }

  return (
    <div className="mx-auto w-full max-w-md rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
      <h1 className="text-2xl font-extrabold text-ink">Регистрация</h1>
      <p className="mt-1 text-sm text-slate-600">Создайте аккаунт покупателя для оформления заказов.</p>

      <form className="mt-6 space-y-4" onSubmit={onSubmit}>
        <Input label="ФИО" value={fullName} onChange={(event) => setFullName(event.target.value)} required />
        <Input label="Email" type="email" value={email} onChange={(event) => setEmail(event.target.value)} required />
        <Input label="Телефон" value={phone} onChange={(event) => setPhone(event.target.value)} required />
        <Input label="Пароль" type="password" value={password} onChange={(event) => setPassword(event.target.value)} required />
        <Input
          label="Повторите пароль"
          type="password"
          value={confirmPassword}
          onChange={(event) => setConfirmPassword(event.target.value)}
          required
        />
        <Button type="submit" variant="secondary" fullWidth disabled={mutation.isPending}>
          {mutation.isPending ? 'Регистрируем...' : 'Создать аккаунт'}
        </Button>
      </form>

      <p className="mt-4 text-sm text-slate-600">
        Уже есть аккаунт?{' '}
        <Link to="/login" className="font-semibold text-ink hover:text-slate-700">
          Войти
        </Link>
      </p>
    </div>
  )
}
