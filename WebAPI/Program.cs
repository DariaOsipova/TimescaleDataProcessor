using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Application.Interfaces;
using Application.Services;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{ // CORS-политика безопасности, которая разрешает/запрещает запросы с других сайтов
    options.AddPolicy("AllowAll", // AddPolicy("AllowAll")-Создаем политику с именем "AllowAll"
     policy => { policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }); // Разрешаем запросы с любых сайтов
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Настраивает генерацию Swagger документации,EnableAnnotations() включает поддержку аннотаций
builder.Services.AddSwaggerGen(c => { c.EnableAnnotations(); });

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection") ?? //?? -Если GetConnectionString вернул null, используй значение по умолчанию
    "Host=localhost;Port=5432;Database=TimescaleDb;Username=postgres;Password=123456;";

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddInfrastructure();

builder.Services.AddScoped<IValueRecordService, ValueRecordService>();
builder.Services.AddScoped<IResultRecordService, ResultRecordService>();

builder.Services.AddAutoMapper(typeof(Application.Mappings.MappingProfile));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // инструмент для подготовки документации и тестирования API
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Автоматически перенаправляет HTTP запросы на HTTP
app.UseAuthorization(); // Включает проверку авторизации (роли, права доступа)

app.UseCors("AllowAll"); // Применяет политику CORS с именем "AllowAll", которую мы настраивали ранее

app.UseMiddleware<ExceptionHandlingMiddleware>(); // Добавляет middleware для обработки исключений(ловит все исключения, Возвращает понятный ответ клиенту, Логирует ошибки)

app.MapControllers(); // включить расписание для  API
Console.WriteLine("        API ЗАПУЩЕН!");
Console.WriteLine("SWAGGER: /swagger");

app.Run();