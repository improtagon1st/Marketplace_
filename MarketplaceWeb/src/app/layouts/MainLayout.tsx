import { Dialog, DialogPanel } from '@headlessui/react'
import { Bars3Icon, ShoppingBagIcon, XMarkIcon } from '@heroicons/react/24/outline'
import { useQuery } from '@tanstack/react-query'
import { useState } from 'react'
import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom'
import { getCart } from '../../entities/cart/api/cartApi'
import { useAuthStore } from '../../features/auth/store/authStore'
import { getGuestCartCount, useCartStore } from '../../features/cart/store/cartStore'
import { Button } from '../../shared/ui/Button'

const links = [
  { to: '/', label: 'Главная' },
  { to: '/catalog', label: 'Каталог' },
  { to: '/cart', label: 'Корзина' },
]

export function MainLayout() {
  const [mobileOpen, setMobileOpen] = useState(false)
  const navigate = useNavigate()

  const isAuthenticated = useAuthStore((state) => state.isAuthenticated)
  const user = useAuthStore((state) => state.user)
  const logout = useAuthStore((state) => state.logout)
  const guestItems = useCartStore((state) => state.guestItems)
  const isCustomer = isAuthenticated && user?.role === 'Customer'

  const navLinks = isCustomer ? [...links, { to: '/orders', label: 'Мои заказы' }] : links

  const { data: serverCart } = useQuery({
    queryKey: ['cart', 'count'],
    queryFn: getCart,
    enabled: isCustomer,
  })

  const cartCount = isAuthenticated
    ? (serverCart ?? []).reduce((sum, item) => sum + item.quantity, 0)
    : getGuestCartCount(guestItems)

  const handleLogout = () => {
    logout()
    navigate('/')
  }

  return (
    <div className="flex min-h-screen flex-col">
      <header className="sticky top-0 z-30 border-b border-slate-200/70 bg-white/90 backdrop-blur">
        <div className="mx-auto flex w-full max-w-7xl items-center justify-between px-4 py-3 md:px-6">
          <Link to="/" className="flex items-center gap-2">
            <span className="rounded-lg bg-ink px-2 py-1 text-xs font-bold uppercase text-white">MP</span>
            <span className="text-lg font-extrabold text-ink">Marketplace</span>
          </Link>

          <nav className="hidden items-center gap-6 md:flex">
            {navLinks.map((link) => (
              <NavLink
                key={link.to}
                to={link.to}
                className={({ isActive }) =>
                  `text-sm font-semibold transition ${isActive ? 'text-ink' : 'text-slate-600 hover:text-ink'}`
                }
              >
                {link.label}
              </NavLink>
            ))}
            <Link to="/cart" className="text-sm font-semibold text-slate-700">
              Корзина: {cartCount}
            </Link>
          </nav>

          <div className="hidden items-center gap-2 md:flex">
            {isAuthenticated && user ? (
              <>
                <Link to="/profile" className="text-sm font-medium text-slate-700 hover:text-ink">
                  {user.fullName}
                </Link>
                <Button variant="ghost" onClick={handleLogout}>
                  Выйти
                </Button>
              </>
            ) : (
              <>
                <Link to="/login" className="text-sm font-semibold text-slate-700 hover:text-ink">
                  Вход
                </Link>
                <Button variant="primary" onClick={() => navigate('/register')}>
                  Регистрация
                </Button>
              </>
            )}
          </div>

          <button
            type="button"
            className="md:hidden"
            onClick={() => setMobileOpen(true)}
            aria-label="Открыть меню"
          >
            <Bars3Icon className="h-6 w-6 text-slate-700" />
          </button>
        </div>
      </header>

      <Dialog open={mobileOpen} onClose={setMobileOpen} className="relative z-50 md:hidden">
        <div className="fixed inset-0 bg-black/30" aria-hidden="true" />
        <div className="fixed inset-y-0 right-0 flex w-full max-w-xs">
          <DialogPanel className="flex w-full flex-col bg-white p-5 shadow-2xl">
            <div className="mb-4 flex items-center justify-between">
              <span className="text-base font-bold">Меню</span>
              <button type="button" onClick={() => setMobileOpen(false)} aria-label="Закрыть меню">
                <XMarkIcon className="h-6 w-6 text-slate-700" />
              </button>
            </div>

            <div className="space-y-3">
              {navLinks.map((link) => (
                <Link
                  key={link.to}
                  to={link.to}
                  className="block rounded-lg bg-slate-100 px-3 py-2 text-sm font-medium"
                  onClick={() => setMobileOpen(false)}
                >
                  {link.label}
                </Link>
              ))}
              <p className="flex items-center gap-1 text-sm font-semibold text-slate-700">
                <ShoppingBagIcon className="h-4 w-4" />
                Товаров в корзине: {cartCount}
              </p>
            </div>

            <div className="mt-auto pt-5">
              {isAuthenticated && user ? (
                <>
                  <Link to="/profile" onClick={() => setMobileOpen(false)} className="mb-2 block text-sm">
                    {user.fullName}
                  </Link>
                  <Button
                    variant="ghost"
                    fullWidth
                    onClick={() => {
                      setMobileOpen(false)
                      handleLogout()
                    }}
                  >
                    Выйти
                  </Button>
                </>
              ) : (
                <div className="space-y-2">
                  <Button
                    fullWidth
                    variant="primary"
                    onClick={() => {
                      setMobileOpen(false)
                      navigate('/login')
                    }}
                  >
                    Вход
                  </Button>
                  <Button
                    fullWidth
                    variant="secondary"
                    onClick={() => {
                      setMobileOpen(false)
                      navigate('/register')
                    }}
                  >
                    Регистрация
                  </Button>
                </div>
              )}
            </div>
          </DialogPanel>
        </div>
      </Dialog>

      <main className="mx-auto w-full max-w-7xl flex-1 px-4 py-8 md:px-6">
        <Outlet />
      </main>

      <footer className="border-t border-slate-200 bg-white/70">
        <div className="mx-auto flex w-full max-w-7xl flex-col gap-1 px-4 py-5 text-sm text-slate-600 md:px-6">
          <p className="font-semibold text-slate-700">Marketplace</p>
          <p>Покупки онлайн, выдача в ПВЗ, управление заказами в личном кабинете.</p>
        </div>
      </footer>
    </div>
  )
}
