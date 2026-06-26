import { useMutation } from '@tanstack/react-query'
import { type FormEvent, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { register } from '../features/auth/api/authApi'
import { getApiErrorMessage } from '../shared/api/http'
import { Button } from '../shared/ui/Button'
import { Input } from '../shared/ui/Input'
import { toast } from '../shared/ui/Toast'

function isPhoneValid(phone: string) {
  const digits = phone.replace(/\D/g, '')
  return digits.length >= 10 && digits.length <= 15 && /^[+\d()\s-]+$/.test(phone)
}

export function RegisterPage() {
  const navigate = useNavigate()
  const [fullName, setFullName] = useState('')
  const [email, setEmail] = useState('')
  const [phone, setPhone] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [isConsentGiven, setIsConsentGiven] = useState(false)

  const mutation = useMutation({
    mutationFn: register,
    onSuccess: () => {
      toast.success('Регистрация успешна. Теперь войдите в аккаунт.')
      navigate('/login')
    },
    onError: (error) => {
      toast.error(getApiErrorMessage(error, 'Не удалось зарегистрироваться'))
    },
  })

  const onSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    if (!isPhoneValid(phone.trim())) {
      toast.error('Введите корректный номер телефона')
      return
    }

    if (password.length < 6) {
      toast.error('Пароль должен содержать минимум 6 символов')
      return
    }

    if (password !== confirmPassword) {
      toast.error('Пароли не совпадают')
      return
    }

    if (!isConsentGiven) {
      toast.error('Необходимо согласие на обработку персональных данных')
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
        <Input
          label="Телефон"
          type="tel"
          value={phone}
          onChange={(event) => setPhone(event.target.value)}
          placeholder="+7 (900) 123-45-67"
          minLength={10}
          required
        />
        <Input
          label="Пароль"
          type="password"
          value={password}
          onChange={(event) => setPassword(event.target.value)}
          minLength={6}
          required
        />
        <Input
          label="Повторите пароль"
          type="password"
          value={confirmPassword}
          onChange={(event) => setConfirmPassword(event.target.value)}
          minLength={6}
          required
        />
        <label className="flex items-start gap-3 rounded-2xl border border-slate-200 bg-slate-50 p-3 text-sm leading-5 text-slate-700">
          <input
            type="checkbox"
            className="mt-1 h-4 w-4 rounded border-slate-300 text-mint focus:ring-mint"
            checked={isConsentGiven}
            onChange={(event) => setIsConsentGiven(event.target.checked)}
            required
          />
          <span>
            Я согласен на{' '}
            <a
              href="/personal-data"
              target="_blank"
              rel="noopener noreferrer"
              className="font-semibold text-ink underline-offset-2 hover:underline"
            >
              обработку персональных данных
            </a>{' '}
            и принимаю условия{' '}
            <a
              href="/user-agreement"
              target="_blank"
              rel="noopener noreferrer"
              className="font-semibold text-ink underline-offset-2 hover:underline"
            >
              пользовательского соглашения
            </a>
            .
          </span>
        </label>
        <Button type="submit" variant="secondary" fullWidth disabled={mutation.isPending || !isConsentGiven}>
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
