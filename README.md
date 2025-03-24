# simple_site

simple_site — это одностраничное веб-приложение на базе ASP.NET Core. Приложение собирает координаты движения мыши в формате `[X, Y, T]` (где `X` и `Y` — координаты, `T` — время события) с помощью JavaScript, отправляет их на сервер по нажатию кнопки "Отправить данные" и сохраняет в базу данных в формате JSON.

---

## Структура проекта

Проект организован по принципами чистой архитектуры и разделен на следующие слои:

- **Domain**: (`MouseData`, `MouseEvent`) — объекты.
- **Application**: (`MouseService`) — бизнес-логика приложения.
- **Infrastructure**: Адаптер (`MouseRepository`) — реализация взаимодействия с бд.
- **Data**: Контекст базы данных (`AppDbContext`) — работа с Entity Framework Core.
- **Controllers**: (`HomeController`) — обработка HTTP-запросов.
- **wwwroot/js**: Клиентская логика (`site.js`) — сбор и отправка данных о движении мыши.
- **Pages**: (`Index.cshtml`) — пользовательский интерфейс.
- **Root**: Конфигурация (`Program.cs`, `appsettings.json`) — настройка приложения и подключение к базе данных.

---

## Требования

Для работы с проектом вам понадобится:
- Docker и Docker Compose для запуска приложения.
- .NET 9 SDK

---

## Установка

1. **Клонируйте репозиторий:**
   - git clone https://github.com/Artimary/simple_site.git
   - cd simple_site

2. **Запустите проект с помощью Docker**
Выполните следующую команду в корне проекта: 
- docker-compose build --no-cache
- docker-compose up

---

## Использование

**Доступ к приложению:**
После успешного запуска откройте браузер и перейдите по адресу: http://localhost:5000

**Остановка приложения:**
Чтобы остановить приложение, выполните: 
- docker-compose down -v

**Логи:**
Логи контейнеров можно посмотреть с помощью команды:
- docker-compose logs

**База данных**
При запуске бд, чтобы войти используйте следующую команду:
- docker exec -it simple_site-sqlserver-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P StrongP@ssw0rd -C
Для просмотра созданной бд:
- USE AppDB
- SELECT * FROM MouseData 
- GO

---

## Юнит-тесты
Проект включает юнит-тесты для проверки бизнес-логики и взаимодействия с репозиториями. Тесты написаны с использованием фреймворка xUnit и находятся в папке test.

**Структура:**
**HomeControllerTests.cs** — тесты для HomeController, проверяют вызов сервиса и логирование.
**MouseRepositoryTests.cs** — тесты для MouseRepository, проверяют добавление данных в контекст базы данных.
**MouseServiceTests.cs** — тесты для MouseService, проверяют вызов репозитория и логирование.

**Локальное выполнение тестов**
- cd test
- dotnet restore
- dotnet test