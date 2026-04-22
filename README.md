# ToDo API

REST API для управления задачами с JWT-аутентификацией. 
Проект демонстрирует навыки работы с ASP.NET Core, Clean Architecture, Entity Framework Core и интеграционным тестированием.

## Технологии

- **.NET 9** / ASP.NET Core Web API
- **Entity Framework Core** + SQLite
- **JWT Bearer** аутентификация
- **FluentValidation**
- **Serilog** (логирование в файл)
- **API Versioning**
- **xUnit** + **WebApplicationFactory** (интеграционные тесты)
- **BCrypt.Net** (хэширование паролей)

## Архитектура

Проект разделен на слои в соответствии с Clean Architecture:

- **ToDoApp.API** — контроллеры, middleware, конфигурация
- **ToDoApp.Core** — сущности, интерфейсы, DTO
- **ToDoApp.Infrastructure** — реализация репозиториев, сервисов, DbContext
- **ToDoApp.IntegrationTests** — интеграционные тесты

## Аутентификация

- Регистрация: `POST /api/v1/auth/register`
- Логин: `POST /api/v1/auth/login` (возвращает JWT токен)

Пароли хэшируются с помощью BCrypt.

## Задачи (CRUD)

Все эндпоинты защищены атрибутом `[Authorize]`. 
Требуется передача токена в заголовке:
`Authorization: Bearer {token}`

| Метод | Эндпоинт | Описание |
|-------|----------|----------|
| GET | `/api/v1/tasks` | Получить все задачи текущего пользователя |
| GET | `/api/v1/tasks/{id}` | Получить задачу по ID |
| POST | `/api/v1/tasks` | Создать новую задачу |
| PUT | `/api/v1/tasks/{id}` | Обновить задачу |
| DELETE | `/api/v1/tasks/{id}` | Удалить задачу |

## Тестирование

Проект содержит **9 интеграционных тестов**, покрывающих:

- Регистрацию и логин (позитивные и негативные сценарии)
- Доступ к задачам с токеном и без
- CRUD операции с задачами
- Валидацию (FluentValidation)
- Изоляцию данных между пользователями

Тесты используют **SQLite In-Memory** и **WebApplicationFactory**.

## Запуск проекта

### Требования
- .NET 9 SDK

### Запуск API

cd ToDoApp.API
dotnet run

Swagger UI: /swagger

## Запуск тестов

cd ToDoApp.IntegrationTests
dotnet test

## Примеры запросов
Регистрация

POST /api/v1/auth/register
Content-Type: application/json

{
  "userName": "testuser",
  "email": "test@example.com",
  "password": "123456"
}

Логин

POST /api/v1/auth/login
Content-Type: application/json

{
  "userName": "testuser",
  "password": "123456"
}

Создание задачи

POST /api/v1/tasks
Authorization: Bearer {token}
Content-Type: application/json

{
  "taskName": "Завершить README",
  "isCompleted": false
}

## Автор

Екатерина Заславская — [GitHub](https://github.com/EllyLilly)