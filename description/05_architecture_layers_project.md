# Архитектура и слои системы

> Проект представляет собой веб-приложение для учёта компьютеров и установленного программного обеспечения. Используется ASP.NET Core MVC, Entity Framework Core и SQLite.

| Frontend (Client) | Backend (Server) | Связь |
|-------------------|------------------|-------|
| Razor Views, Bootstrap, HTML/CSS/JavaScript | ASP.NET Core MVC + EF Core | HTTP-запросы MVC, Model Binding |

## Слои системы

| Название уровня | Ответственность | Компоненты |
|-----------------|----------------|------------|
| Presentation Layer | Отображение интерфейса и обработка действий пользователя | Razor Views, Bootstrap, Forms |
| MVC Controller Layer | Обработка запросов и координация логики | ComputersController, SoftwaresController, AccountController |
| Business Layer | Реализация сценариев использования | Управление компьютерами, ПО, ролями и авторизацией |
| Data Access Layer | Работа с БД | AppDbContext, Entity Framework Core |
| Storage Layer | Физическое хранение данных | SQLite Database |

## Модули и ответственность

| Модуль | Слой | Ответственность |
|---------|------|----------------|
| Account Module | Presentation + Controller | Авторизация пользователей и управление доступом |
| Computer Module | Controller + Business | CRUD операции над компьютерами |
| Software Module | Controller + Business | Ведение справочника программного обеспечения |
| Installation Module | Business | Учёт установленного ПО на компьютерах |
| Identity Module | Infrastructure | Управление ролями Admin/User |
| Database Module | Data Layer | Хранение и извлечение данных через EF Core |

# Правило зависимостей

1. Views зависят только от Controllers.
2. Controllers работают через модели и DbContext.
3. Business-логика использует Domain Models.
4. Domain Models не зависят от инфраструктуры.
5. EF Core обеспечивает доступ к данным SQLite.
