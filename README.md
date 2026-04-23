# Marketplace (дипломный проект)

Репозиторий содержит 3 части:

- `MarketplaceAPI` - ASP.NET Core Web API (`net8.0`) с JWT, товарами, корзиной, заказами и ПВЗ.
- `MarketplaceWPF` - WPF-клиент для ролей `Admin` и `PickupPointWorker`.
- `MarketplaceWeb` - React + Vite + TypeScript сайт для роли `Customer` (покупатели).

## Быстрый запуск для защиты

Самая простая инструкция для запуска на другом компьютере лежит в [FRIEND_START_GUIDE.md](FRIEND_START_GUIDE.md).

### 1. Запустить API (терминал #1)

```powershell
cd MarketplaceAPI
dotnet restore
dotnet run --launch-profile https
```

API должен быть доступен по `https://localhost:7093`.

Если браузер/клиент не доверяет dev-сертификату HTTPS:

```powershell
dotnet dev-certs https --trust
```

### 2. Запустить сайт (терминал #2)

```powershell
cd MarketplaceWeb
npm install
npm run dev
```

Сайт будет доступен по URL из вывода Vite (обычно `http://localhost:5173`).

## Конфигурация Web-клиента

В `MarketplaceWeb/.env`:

```env
VITE_API_BASE_URL=https://localhost:7093/api
```

## Роли и доступ

- `Customer` - работает через сайт `MarketplaceWeb`.
- `Admin` и `PickupPointWorker` - работают через `MarketplaceWPF`.

Сайт намеренно блокирует вход для `Admin/Worker` и предлагает использовать WPF.

## Что реализовано в MarketplaceWeb

- Главная витрина
- Каталог товаров (поиск, фильтр по категориям, сортировка, пагинация)
- Карточка товара
- Регистрация и вход
- Гостевая корзина в `localStorage`
- Синхронизация гостевой корзины после входа
- Корзина авторизованного пользователя через API
- Checkout с выбором ПВЗ
- Оплата при получении
- Список заказов
- Детали заказа + QR-код (`GET /api/orders/qr/{id}` как blob)
- Профиль пользователя

## Тестовые аккаунты

- Admin: `admin@mail.ru` / `123456`
- Worker: `worker1@mail.ru` / `123456`
- Customer: `user@mail.ru` / `123456`

## Известные ограничения первой версии

- Нет онлайн-оплаты
- Нет избранного, отзывов, промокодов
- Нет автотестов (проверка вручную)
