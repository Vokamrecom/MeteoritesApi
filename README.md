# MeteoritesApi

ASP.NET Core 8 сервис для загрузки NASA meteorite dataset, его синхронизации в PostgreSQL и выдачи агрегированных данных через REST API.

## Стек

- .NET 8, ASP.NET Core Web API, EF Core + Npgsql
- Hangfire (фоновые задания + Dashboard)
- Serilog
- FluentValidation
- PostgreSQL

##  Быстрый старт

1. Установите .NET 8.0 SDK и PostgreSQL
2. Настройте строку подключения в `appsettings.json`
3. Запустите:
```bash
dotnet restore
dotnet run
```
База данных создается автоматически!

**Переменные/конфиги**
   - `appsettings.json` содержит:
     - `ConnectionStrings:Default` — строка подключения к PostgreSQL
     - `SyncJob:CronExpression` — расписание Hangfire job
     - `Hangfire` — очередь и количество воркеров
     - `NASAData:SourceUrl` — URL датасета (по умолчанию NASA JSON)


База данных создается автоматически!

##  Документация

Swagger UI: `https://localhost:7150/swagger`
