# ExamApi — ASP.NET Core 6 Web API

Простой REST API без подключения к БД (экзаменационное задание).

---

## Структура проекта

```
ExamApi/
├── Controllers/
│   ├── ProductsController.cs    # CRUD для Products
│   ├── CategoriesController.cs  # CRUD для Categories
│   └── HealthController.cs      # Health Check
├── Models/
│   ├── Product.cs
│   └── Category.cs
├── Program.cs
├── appsettings.json             # слушает :5000
├── examapi.service              # systemd unit файл
├── examapi.nginx.conf           # конфиг Nginx reverse proxy
└── README.md                    # эта шпаргалка
```

---

## Эндпоинты

| Метод  | URL                   | Описание           |
|--------|-----------------------|--------------------|
| GET    | /api/products         | Все продукты       |
| GET    | /api/products/{id}    | Продукт по ID      |
| POST   | /api/products         | Создать продукт    |
| PUT    | /api/products/{id}    | Обновить продукт   |
| DELETE | /api/products/{id}    | Удалить продукт    |
| GET    | /api/categories       | Все категории      |
| GET    | /api/categories/{id}  | Категория по ID    |
| POST   | /api/categories       | Создать категорию  |
| PUT    | /api/categories/{id}  | Обновить категорию |
| DELETE | /api/categories/{id}  | Удалить категорию  |
| GET    | /api/health           | Health Check       |

---

## ЧАСТЬ 1 — Локальный запуск на Windows (для проверки)

### Шаг 1. Установить .NET 6 SDK

Скачать и установить: https://dotnet.microsoft.com/en-us/download/dotnet/6.0  
Выбрать: **.NET 6.0 → SDK → Windows x64 Installer**

Проверить установку (открыть PowerShell):
```powershell
dotnet --version
# должно вывести: 6.0.xxx
```

### Шаг 2. Клонировать репозиторий

```powershell
git clone https://github.com/<ваш-логин>/ExamApi.git
cd ExamApi
```

### Шаг 3. Запустить

```powershell
dotnet run
```

Приложение запустится на `http://localhost:5000`

### Шаг 4. Проверить в браузере или PowerShell

```powershell
# Health check
Invoke-RestMethod http://localhost:5000/api/health

# Все продукты
Invoke-RestMethod http://localhost:5000/api/products

# Все категории
Invoke-RestMethod http://localhost:5000/api/categories

# Создать продукт
$body = @{ name="Monitor"; price=299.99; stock=5 } | ConvertTo-Json
Invoke-RestMethod http://localhost:5000/api/products -Method Post -Body $body -ContentType "application/json"
```

Swagger UI (интерактивная документация): `http://localhost:5000/swagger`

---

## ЧАСТЬ 2 — Загрузить проект на GitHub

### Шаг 1. Создать репозиторий на GitHub

1. Зайти на https://github.com → **New repository**
2. Имя: `ExamApi`, тип: Public, без README (он уже есть)
3. Нажать **Create repository**

### Шаг 2. Инициализировать и запушить (из папки проекта на Windows)

```powershell
git init
git add .
git commit -m "Initial commit"
git branch -M main
git remote add origin https://github.com/<ваш-логин>/ExamApi.git
git push -u origin main
```

---

## ЧАСТЬ 3 — Деплой на Linux VM

> Все команды выполняются на VM по SSH.  
> Вместо `<ваш-логин>` подставить свой GitHub username.

### Шаг 1. Установить .NET 6

```bash
# Добавить репозиторий Microsoft
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

# Обновить пакеты и установить SDK
sudo apt update
sudo apt install -y dotnet-sdk-6.0

# Проверить
dotnet --version
```

### Шаг 2. Подготовить директорию и клонировать репозиторий

```bash
# Создать директорию для репозитория
mkdir -p /home/$USER/app
cd /home/$USER/app

# Клонировать репозиторий
git clone https://github.com/<ваш-логин>/ExamApi.git .

# Убедиться что файлы на месте
ls -la
```

### Шаг 3. Собрать проект и опубликовать в /var/www/app

```bash
# Создать папку для деплоя с правами 755
sudo mkdir -p /var/www/app
sudo chmod 755 /var/www/app

# Собрать и опубликовать
cd /home/$USER/app
dotnet publish -c Release -o /var/www/app

# Проверить что файлы появились
ls -la /var/www/app
```

### Шаг 4. Создать systemd демон

```bash
# Скопировать unit файл
sudo cp /home/$USER/app/examapi.service /etc/systemd/system/examapi.service

# Перечитать конфиги systemd
sudo systemctl daemon-reload

# Включить автозапуск при старте VM
sudo systemctl enable examapi

# Запустить сервис
sudo systemctl start examapi

# Проверить статус (должно быть: Active: active (running))
sudo systemctl status examapi
```

Если что-то пошло не так — смотреть логи:
```bash
sudo journalctl -u examapi -n 50 --no-pager
```

### Шаг 5. Установить Nginx и настроить reverse proxy

```bash
# Установить Nginx
sudo apt install -y nginx

# Скопировать конфиг
sudo cp /home/$USER/app/examapi.nginx.conf /etc/nginx/sites-available/examapi

# Включить сайт
sudo ln -s /etc/nginx/sites-available/examapi /etc/nginx/sites-enabled/

# Удалить дефолтный сайт чтобы не конфликтовал
sudo rm /etc/nginx/sites-enabled/default

# Проверить конфиг на ошибки
sudo nginx -t

# Перезапустить и включить автозапуск
sudo systemctl restart nginx
sudo systemctl enable nginx

# Проверить статус
sudo systemctl status nginx
```

---

## ЧАСТЬ 4 — Проверка с Windows (финальная демонстрация)

Узнать IP адрес VM:
```bash
ip a
# смотреть inet у eth0 или ens3
```

Запросы с Windows (подставить реальный IP):
```powershell
# Health check
Invoke-RestMethod http://<IP-VM>/api/health

# Получить все продукты
Invoke-RestMethod http://<IP-VM>/api/products

# Получить продукт по ID
Invoke-RestMethod http://<IP-VM>/api/products/1

# Создать продукт
$body = @{ name="Monitor"; price=299.99; stock=5 } | ConvertTo-Json
Invoke-RestMethod http://<IP-VM>/api/products -Method Post -Body $body -ContentType "application/json"

# Обновить продукт
$body = @{ name="Monitor 4K"; price=499.99; stock=3 } | ConvertTo-Json
Invoke-RestMethod http://<IP-VM>/api/products/4 -Method Put -Body $body -ContentType "application/json"

# Удалить продукт
Invoke-RestMethod http://<IP-VM>/api/products/4 -Method Delete

# Получить все категории
Invoke-RestMethod http://<IP-VM>/api/categories
```

---

## ЧАСТЬ 5 — История команд (показать перед сдачей)

```bash
history
```

---

## Быстрая диагностика

| Проблема | Команда |
|---|---|
| Сервис не запускается | `sudo journalctl -u examapi -n 50` |
| Nginx не стартует | `sudo nginx -t` |
| Порт уже занят | `sudo ss -tlnp \| grep 5000` |
| Проверить что слушает :5000 | `curl http://localhost:5000/api/health` |
| Проверить что слушает :80 | `curl http://localhost/api/health` |
