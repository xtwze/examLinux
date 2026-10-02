# ExamApi — подготовка к зачёту по Linux

Простой ASP.NET Core 6 API без базы данных: модели `Product` и `Category`,
два CRUD-контроллера и `GET /api/health`. Данные хранятся в памяти и
сбрасываются после перезапуска.

Основной сценарий рассчитан на **Ubuntu 22.04 в VirtualBox**, обычного
пользователя с `sudo` и доступ к интернету. Docker для выполнения задания
не нужен.

## С чего начать

Открой **[пошаговую шпаргалку](exam-prep/00-START.txt)** на Windows и вводи
команды блоками в терминал Linux. Она включает установку SDK, клонирование,
публикацию, systemd, Nginx, проверку из Windows и показ `history`.

| Файл | Для чего |
|---|---|
| [00-START.txt](exam-prep/00-START.txt) | Полный сценарий выполнения |
| [HELP.txt](exam-prep/HELP.txt) | Диагностика ошибок сборки, сервиса, Nginx и сети |
| [WINDOWS.ps1](exam-prep/WINDOWS.ps1) | Демонстрация Health Check и CRUD обеих моделей |
| [AUDIT.txt](exam-prep/AUDIT.txt) | Сопоставление исходного репозитория с каждым требованием задания |
| [VERIFICATION.txt](exam-prep/VERIFICATION.txt) | Лог проверки исходного API в Linux |
| [AI.txt](exam-prep/AI.txt) | Сетевой и локальный ИИ для подсказок |
| [AI-CONTEXT.txt](exam-prep/AI-CONTEXT.txt) | Готовый контекст для чата |
| [ASK-LOCAL.ps1](exam-prep/ASK-LOCAL.ps1) | Необязательный клиент локальной Ollama на Windows |

## Основные команды

Установка на чистой Ubuntu 22.04 и клонирование в пустую папку:

```bash
sudo apt update
sudo apt install -y git curl nginx dotnet-sdk-6.0
mkdir -p "$HOME/app"
cd "$HOME/app"
git clone https://github.com/xtwze/examLinux.git .
```

Если репозиторий уже клонирован в `~/app`, повторять `git clone` не нужно.

Сборка выполняется обычным пользователем, копирование — через `sudo`:

```bash
dotnet publish ExamApi.csproj -c Release -o "$HOME/exam-publish"
sudo install -d -m 755 /var/www/app
sudo cp -a "$HOME/exam-publish/." /var/www/app/
sudo chown -R root:root /var/www/app
sudo chmod -R a+rX /var/www/app
```

Далее выполни шаги systemd и Nginx из шпаргалки. Готовые конфиги находятся
в корне: `examapi.service` и `examapi.nginx.conf`.

Схема запроса при NAT VirtualBox:

```text
Windows 127.0.0.1:8080 → Linux Nginx :80 → ExamApi :5000
```

Создай правило проброса TCP с IP хоста `127.0.0.1`, портом хоста `8080`
и портом гостя `80`. IP гостя при стандартном DHCP оставь пустым.
После развёртывания проверь в PowerShell:

```powershell
$base = 'http://127.0.0.1:8080'
Invoke-RestMethod "$base/api/health"
Invoke-RestMethod "$base/api/products"
Invoke-RestMethod "$base/api/categories"
```

## Маршруты

| Метод | Путь | Действие |
|---|---|---|
| GET | `/api/health` | Проверка ответа сервиса |
| GET | `/api/products`, `/api/categories` | Список |
| GET | `/api/products/{id}`, `/api/categories/{id}` | Одна запись |
| POST | `/api/products`, `/api/categories` | Создание |
| PUT | `/api/products/{id}`, `/api/categories/{id}` | Изменение |
| DELETE | `/api/products/{id}`, `/api/categories/{id}` | Удаление |

После POST используй ID из ответа, а не фиксированный `4`.
Успешные PUT/DELETE возвращают `204` без тела.

Корень `/` не имеет маршрута. Swagger включён только в Development;
для отдельной локальной тренировки можно запустить:

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run --urls http://127.0.0.1:5001
```

Swagger будет доступен внутри VM на `http://127.0.0.1:5001/swagger`.
Для защиты через systemd используется Production и обычные API-запросы.

## Что проверено

Для исходного коммита `fe1426f` прошли чистая Linux-сборка, запуск от
`www-data`, проверка конфигов systemd/Nginx и 34 HTTP-проверки напрямую
на 5000 и через Nginx на 80. Код API с тех пор в этой доработке не менялся.

Проверка проводилась в Ubuntu 22.04 ARM64-контейнере. Реальные автозагрузка
systemd после перезагрузки, сеть VirtualBox, Windows PowerShell и Ollama
должны быть проверены на целевом компьютере перед зачётом.

.NET 6 оставлен ради задания; [его поддержка завершилась 12.11.2024](https://devblogs.microsoft.com/dotnet/dotnet-6-end-of-support/).
ИИ не требуется заданием; на зачёте используй его только при разрешении
преподавателя. Основная шпаргалка работает без ИИ.
