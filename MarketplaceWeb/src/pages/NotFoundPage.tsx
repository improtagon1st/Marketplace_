import { Link } from 'react-router-dom'
import { Button } from '../shared/ui/Button'

export function NotFoundPage() {
  return (
    <div className="mx-auto flex max-w-lg flex-col items-center rounded-3xl border border-slate-200 bg-white p-10 text-center">
      <p className="text-xs uppercase tracking-widest text-slate-500">404</p>
      <h1 className="mt-2 text-3xl font-extrabold text-ink">Страница не найдена</h1>
      <p className="mt-2 text-sm text-slate-600">Похоже, ссылка устарела или была введена с ошибкой.</p>
      <Link to="/" className="mt-5">
        <Button variant="primary">На главную</Button>
      </Link>
    </div>
  )
}
