
---

# ATM System Engine

Современная программная реализация системы банкомата, построенная на принципах **гексагональной архитектуры** (Ports and Adapters). Проект включает в себя отказоустойчивый RESTful API сервис и интерактивный консольный клиент.

## 🌟 Ключевые возможности

### Ролевая модель и сессии
*   **Двухуровневая авторизация**:
    *   **User**: Доступ по номеру счета и PIN-коду. Позволяет управлять личными финансами.
    *   **Admin**: Доступ по системному паролю. Позволяет создавать новые счета в системе.
*   **Session Management**: Безопасное управление сессиями с выдачей `Guid` токенов для последующих операций.

### Банковские операции
*   **Управление балансом**: Пополнение счета и снятие средств с валидацией остатка.
*   **История операций**: Каждое изменение состояния счета автоматически добавляется в историю операций.

---

## 🏗 Архитектура и технологии

Проект спроектирован с упором на расширяемость и независимость бизнес-логики от внешних инструментов.

### Core
*   **Луковая архитектура**: Четкое разделение на Domain, Application, Abstractions, Contracts, Infrastructure и Presentation.
*   **Dependency Injection**: Использование `Microsoft.Extensions.DependencyInjection` для слабого связывания компонентов. Модульная регистрация зависимостей через методы расширения.
*   **Async**: Полностью асинхронный стек вызовов.

### Backend
*   **Framework**: ASP.NET Core (Controllers).
*   **Database**: PostgreSQL. Работа с БД реализована через низкоуровневый драйвер **Npgsql** для максимального контроля и производительности.
*   **Migrations**: Версионирование схемы БД с помощью **FluentMigrator** (SQL-based миграции).
*   **API**: REST-архитектура, документация в формате **OpenAPI Swagger**.
*   **Configuration**: Типизированные настройки (`IOptions`) с валидацией параметров при старте приложения.

### Client
*   **UI/UX**: Интерактивный CLI на базе **Spectre.Console** (таблицы, формы ввода, форматированный вывод).
*   **Communication**: Декларативный HTTP-клиент на базе **Refit**.

---

## 🛠 Технологический стек

*   **Runtime**: .NET 10
*   **Database**: PostgreSQL + Docker
*   **API Documentation**: Swagger
*   **CLI Library**: Spectre.Console
*   **HTTP Client**: Refit
*   **Migrations**: FluentMigrator

---

## 🚀 Быстрый старт

### Требования
*   Docker и Docker Compose
*   .NET SDK (версия 8.0 или выше)

### Запуск инфраструктуры
1. Склонируйте репозиторий.
2. Поднимите бд:
   ```bash
   docker compose up --build 
   ```



### Запуск Сервиса

Добавьте системный пароль в user-secrets
```
dotnet user-secrets set "SystemPasswordSettings:Password" "Значение"
```

Запустите проект
```bash
cd src/service/Main
dotnet run
```
После запуска документация API будет доступна по адресу `http://localhost:5000/swagger` (или аналогичном).

### Запуск Клиента
Из корневой папки
```bash
cd src/console-client/Cli
dotnet run
```

---

## 📂 Структура проекта

```text
src/
├── service/                          # Backend-сервис
│   ├── Main                          # Startup проект (Host)
│   ├── Domain/                       # Доменные модели
│   │   └── BankApp.Domain
│   ├── Application/                  # Бизнес-логика и интерфейсы
│   │   ├── BankApp.Application
│   │   ├── BankApp.Application.Abstractions
│   │   └── BankApp.Contracts
│   ├── Infrastructure/               # Работа с БД и внешними сервисами
│   │   └── BankApp.Infrastructure.Persistence
│   └── Presentation/                 # Web API слой
│       └── BankApp.Presentation.Http
│
└── console-client/                   # Консольный клиент
    ├── Cli                           # Точка входа (Entry point)
    ├── Application/                  # Клиентская логика
    │   ├── BankApp.Cli.Application
    │   ├── BankApp.Cli.Application.Abstractions
    │   ├── BankApp.Cli.Application.Contracts
    │   └── BankApp.Cli.Application.Models
    ├── Infrastructure/               # HTTP-клиенты (Refit)
    │   └── BankApp.Cli.Infrastructure.BankApiService
    └── Presentation/                 # Слой интерфейса (Spectre.Console)
        └── BankApp.Cli.Presentation.Cli
```
---

Этот проект является доработанной реализацией лабораторной работы с курса Backend разработки на C#
