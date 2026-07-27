TimescaleDataProcessor
WebAPI на C# .NET 8 для обработки timescale данных из CSV-файлов.
Описание
API для загрузки CSV-файлов с валидацией, агрегацией статистики и фильтрацией результатов.
Технологии
.NET 8-платформа
ASP.NET Core WebAPI-REST API
Entity Framework Core-ORM
PostgreSQL-база данных
Swagger / OpenAPI-документация
xUnit-тестирование
Moq-моки
AutoMapper-маппинг DTO
Swagger
http://localhost:5211/swagger
dotnet build
dotnet test
Проверь покрытие:
coverlet Tests/bin/Debug/net8.0/Tests.dll --target "dotnet" --targetargs "test --no-build" --format cobertura --output ./coverage.xml