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
dotnet build -c Debug /p:DebugSymbols=true /p:DebugType=full
 Собирает данные о покрытии кода тестами
dotnet-coverage collect "dotnet test Tests/Tests.csproj" -f cobertura -o coverage.xml
Устанавливает глобальный инструмент для генерации отчетов
dotnet tool install --global dotnet-reportgenerator-globaltool
 Генерирует красивый HTML отчет из XML файла покрытия
reportgenerator -reports:coverage.xml -targetdir:CoverageReport -reporttypes:Html
Открывает HTML отчет в браузере
Start-Process .\CoverageReport\index.html