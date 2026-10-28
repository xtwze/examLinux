# ExamApi — подготовка к зачёту по Linux

Простой ASP.NET Core 8 API без базы данных: модели `Product` и `Category`,
два CRUD-контроллера и `GET /api/health`. Данные хранятся в памяти и
сбрасываются после перезапуска.

Основной сценарий рассчитан на **Ubuntu 22.04 или 24.04 в VirtualBox**, обычного
пользователя с `sudo` и доступ к интернету. Docker для выполнения задания
не нужен.

## С чего начать

Открой **[пошаговую шпаргалку](exam-prep/00-START.txt)** на Windows и вводи
команды блоками в терминал Linux. Она включает установку SDK, клонирование,
публикацию, systemd, Nginx, проверку из Windows и показ `history`.

| Файл | Для чего |
|---|---|
| [00-START.txt](exam-prep/00-START.txt) | Полный сценарий выполнения |
| [CONFIG-EXPLAINED.txt](exam-prep/CONFIG-EXPLAINED.txt) | Каждая строка Nginx, systemd и appsettings простыми словами |
| [HELP.txt](exam-prep/HELP.txt) | Диагностика ошибок сборки, сервиса, Nginx и сети |
| [WINDOWS.ps1](exam-prep/WINDOWS.ps1) | Демонстрация Health Check и CRUD обеих моделей |
| [AUDIT.txt](exam-prep/AUDIT.txt) | Сопоставление исходного репозитория с каждым требованием задания |
| [VERIFICATION.txt](exam-prep/VERIFICATION.txt) | Лог проверки исходного API в Linux |
| [AI.txt](exam-prep/AI.txt) | Сетевой и локальный ИИ для подсказок |
| [AI-CONTEXT.txt](exam-prep/AI-CONTEXT.txt) | Готовый контекст для чата |
| [ASK-LOCAL.ps1](exam-prep/ASK-LOCAL.ps1) | Необязательный клиент локальной Ollama на Windows |

## Основные команды

Установка и клонирование в фиксированную папку:

```bash
sudo apt update
sudo apt install -y git curl nginx dotnet-sdk-8.0
mkdir -p "$HOME/app"
git clone https://github.com/xtwze/examLinux.git "$HOME/app/examLinux"
cd "$HOME/app/examLinux"
```

Если репозиторий уже клонирован в `~/app/examLinux`, повторять `git clone`
не нужно. Все дальнейшие команды используют этот точный путь.

Сборка выполняется обычным пользователем, копирование — через `sudo`:

```bash
dotnet publish ExamApi.csproj -c Release -o "$HOME/exam-publish"
sudo install -d -m 755 /var/www/app
sudo cp -a "$HOME/exam-publish/." /var/www/app/
sudo chown -R root:root /var/www/app
sudo find /var/www/app -type d -exec chmod 755 {} +
sudo find /var/www/app -type f -exec chmod 644 {} +
```

Далее выполни шаги systemd и Nginx из шпаргалки. Готовые конфиги находятся
в корне: `examapi.service` и `examapi.nginx.conf`.
Шаг 4 подставляет имя твоего обычного пользователя Ubuntu в `User=`:
от него systemd запускает API. Отдельную учётную запись создавать не нужно.
В шаблоне unit стоит `User=YOUR_USER`; установи его через команду `sed`
из шпаргалки, чтобы заменить шаблон своим именем. `root` владеет публикацией,
а обычный пользователь читает файлы 644 и проходит в каталоги 755.
API слушает только `127.0.0.1:5000`; снаружи доступен Nginx на 80.

Схема запроса при NAT VirtualBox:

```text
Windows 127.0.0.1:8080 → Linux Nginx :80 → ExamApi :5000
```

Создай правило проброса TCP с IP хоста `127.0.0.1`, портом хоста `8080`
и портом гостя `80`. IP гостя при стандартном DHCP оставь пустым.
Для SSH добавь второе правило TCP `127.0.0.1:2222 → гостевой :22`,
установи `openssh-server` и включи `ssh` по шагу 2 шпаргалки.
В Windows: `ssh -p 2222 student@127.0.0.1`, где `student` — имя из `whoami`
в Ubuntu. Для SSH и запуска API используется один и тот же обычный пользователь Ubuntu.
Диагностика HTTP и SSH через NAT — [HELP.txt](exam-prep/HELP.txt), пункты 7 и 12.

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

Результаты текущего пакета и границы проверки записаны в
[VERIFICATION.txt](exam-prep/VERIFICATION.txt). Исторический аудит
[AUDIT.txt](exam-prep/AUDIT.txt) описывает прежние конфиги и не является
подтверждением проверки текущей версии. Проверку SSH, NAT/Bridge и
автозапуска после reboot выполняй в своей VM по шпаргалке.

.NET 8 используется, потому что он доступен в Ubuntu 24.04, на которой
прошла практическая репетиция. В задании .NET 6 указан как желательный,
но разрешён другой фреймворк.
ИИ не требуется заданием; на зачёте используй его только при разрешении
преподавателя. Основная шпаргалка работает без ИИ.
