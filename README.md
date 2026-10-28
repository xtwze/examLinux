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
| [AUDIT.txt](exam-prep/AUDIT.txt) | Соответствие текущей схемы требованиям задания |
| [VERIFICATION.txt](exam-prep/VERIFICATION.txt) | Лог проверки исходного API в Linux |
| [AI.txt](exam-prep/AI.txt) | Сетевой и локальный ИИ для подсказок |
| [AI-CONTEXT.txt](exam-prep/AI-CONTEXT.txt) | Готовый контекст для чата |
| [ASK-LOCAL.ps1](exam-prep/ASK-LOCAL.ps1) | Необязательный клиент локальной Ollama на Windows |

## Основные команды

Все команды с короткими комментариями находятся в [00-START.txt](exam-prep/00-START.txt).
Используй её как единственный маршрут установки, без переключения между файлами.

Задание прямо требует **`/var/www/app` с правами `755`** —
[точный текст](exam-prep/REQUIREMENTS.txt). Поэтому этот путь сохранён.
Каталог один раз создаётся через `sudo install -d -m 755` с текущим
обычным пользователем как владельцем. Затем `dotnet publish` пишет прямо
в `/var/www/app` **без sudo**, промежуточных папок и копирования.
Тот же пользователь запускает API через systemd; `User=YOUR_USER`
в шаблоне заменяется автоматически при установке. Учётные записи не создаются.

Если уже выполнял прежнюю инструкцию, **переустанавливать Ubuntu не нужно**:
[HELP §1](exam-prep/HELP.txt) исправляет старого владельца root:root,
[HELP §8](exam-prep/HELP.txt) описывает обновление существующей установки.

API слушает только `127.0.0.1:5000`; снаружи доступен Nginx на 80.

Схема запроса при NAT VirtualBox:

```text
Windows 127.0.0.1:8080 → Linux Nginx :80 → ExamApi :5000
```

Проброс обоих портов, SSH и проверка Windows полностью включены в START.

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
ASPNETCORE_ENVIRONMENT=Development dotnet run --urls http://127.0.0.1:5001 # Запустить тренировочный Swagger на отдельном порту.
```

Swagger будет доступен внутри VM на `http://127.0.0.1:5001/swagger`.
Для защиты через systemd используется Production и обычные API-запросы.

## Что проверено

Результаты текущего пакета и границы проверки записаны в
[VERIFICATION.txt](exam-prep/VERIFICATION.txt). [AUDIT.txt](exam-prep/AUDIT.txt) сопоставляет текущую схему с заданием. Проверку SSH, NAT/Bridge и
автозапуска после reboot выполняй в своей VM по шпаргалке.

.NET 8 используется, потому что он доступен в Ubuntu 24.04, на которой
прошла практическая репетиция. В задании .NET 6 указан как желательный,
но разрешён другой фреймворк.
ИИ не требуется заданием; на зачёте используй его только при разрешении
преподавателя. Основная шпаргалка работает без ИИ.
