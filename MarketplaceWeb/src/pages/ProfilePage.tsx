import { useAuthStore } from '../features/auth/store/authStore'

export function ProfilePage() {
  const user = useAuthStore((state) => state.user)

  if (!user) {
    return <p className="text-sm text-slate-700">Пользователь не найден.</p>
  }

  return (
    <div className="mx-auto max-w-xl rounded-3xl border border-slate-200 bg-white p-6">
      <h1 className="text-2xl font-extrabold text-ink">Личный кабинет</h1>
      <p className="mt-1 text-sm text-slate-600">Информация из текущей сессии</p>

      <dl className="mt-6 space-y-3 text-sm">
        <div className="flex items-center justify-between border-b border-slate-100 pb-2">
          <dt className="text-slate-600">ФИО</dt>
          <dd className="font-semibold text-ink">{user.fullName}</dd>
        </div>
        <div className="flex items-center justify-between border-b border-slate-100 pb-2">
          <dt className="text-slate-600">Email</dt>
          <dd className="font-semibold text-ink">{user.email}</dd>
        </div>
        <div className="flex items-center justify-between border-b border-slate-100 pb-2">
          <dt className="text-slate-600">Роль</dt>
          <dd className="font-semibold text-ink">{user.role}</dd>
        </div>
        <div className="flex items-center justify-between">
          <dt className="text-slate-600">ID пользователя</dt>
          <dd className="font-semibold text-ink">{user.userId}</dd>
        </div>
      </dl>
    </div>
  )
}
