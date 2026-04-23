# Как запустить проект после скачивания

Эта инструкция для самого простого запуска на другом компьютере.

Главная мысль: локальную базу данных создавать не нужно. Проект уже умеет подключаться к удаленной базе Somee через `MarketplaceAPI/appsettings.json`.

## 1. Что установить один раз

Установи:

1. Visual Studio 2022 Community.
2. В установщике Visual Studio отметь:
   - `.NET desktop development`;
   - `ASP.NET and web development`.
3. Node.js LTS.
4. Git, если хочешь скачивать через `git clone`.

Проверка в терминале:

```powershell
dotnet --version
node -v
npm -v
```

Если команды показывают версии, все нормально.

## 2. Скачать проект

```powershell
git clone https://github.com/improtagon1st/Marketplace_project.git
cd Marketplace_project
```

Если скачал ZIP-архивом с GitHub, просто распакуй архив и открой терминал внутри папки `Marketplace_project`.

## 3. Запустить API

Открой первый терминал:

```powershell
cd MarketplaceAPI
dotnet restore
dotnet dev-certs https --trust
dotnet run --launch-profile https
```

Важно: этот терминал не закрывать.

API должен открыться тут:

```text
https://localhost:7093/swagger
```

Если Windows спросит, доверять ли сертификату, нажми "Да".

## 4. Запустить сайт

Открой второй терминал в папке проекта `Marketplace_project`:

```powershell
cd MarketplaceWeb
npm install
npm run dev
```

Сайт обычно будет тут:

```text
http://localhost:5173
```

Если Vite покажет другой адрес, открывай тот адрес, который написан в терминале.

## 5. Запустить WPF-приложение

Открой третий терминал в папке проекта `Marketplace_project`:

```powershell
dotnet run --project MarketplaceWPF
```

Можно и через Visual Studio:

1. Открой `Marketplace.sln`.
2. Запусти сначала `MarketplaceAPI`.
3. Потом запусти `MarketplaceWPF`.

## 6. В каком порядке запускать

Всегда так:

1. Сначала `MarketplaceAPI`.
2. Потом сайт `MarketplaceWeb`.
3. Потом WPF `MarketplaceWPF`.

Если API не запущен, сайт и WPF не смогут получить товары, корзину и заказы.

## 7. Тестовые аккаунты

Покупатель для сайта:

```text
user@mail.ru
123456
```

Администратор для WPF:

```text
admin@mail.ru
123456
```

Работник ПВЗ для WPF:

```text
worker1@mail.ru
123456
```

## 8. Если что-то не работает

Если сайт пишет ошибку сети:

1. Проверь, что API запущен.
2. Открой `https://localhost:7093/swagger`.
3. Если браузер ругается на сертификат, выполни:

```powershell
dotnet dev-certs https --trust
```

Если `npm install` не работает:

1. Проверь, что установлен Node.js LTS.
2. Закрой терминал.
3. Открой новый терминал и попробуй снова.

Если база данных не отвечает:

1. Проверь интернет.
2. Удаленная база находится на Somee, поэтому без интернета проект не сможет получить данные.

## 9. Нужно ли ставить MS SQL Server

Для обычного запуска не нужно.

MS SQL Server нужен только если хочешь работать со своей локальной базой. Для простого просмотра проекта используй удаленную базу, которая уже подключена в API.
