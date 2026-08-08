using System.Net; // HttpStatusCode-перечисление HTTP статусов
using System.Text.Json;
using Domain.Exceptions;

namespace WebAPI.Middleware
{ // прмоежуточный слой
    public class ExceptionHandlingMiddleware
    { // логгер для записи всех ошибок, которые возникают в любом месте приложения
        private readonly RequestDelegate _next; // delegate-объект, который хранит ссылку на метод
        private readonly ILogger<ExceptionHandlingMiddleware> _logger; // запись

        public ExceptionHandlingMiddleware(RequestDelegate next,
                                           ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        { // не блокирует поток
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex); // обрабатываем ошибку
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, exception.Message);

            var response = // ответ
                new
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Произошла внутренняя ошибка сервера.",
                    Details = exception.Message
                };

            switch (exception)
            {
                case CsvValidationException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "Ошибка валидации CSV.",
                        Details = exception.Message
                    }; // ответ на ошибку
                    break;

                case FileProcessingException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "Ошибка обработки файла.",
                        Details = exception.Message
                    };
                    break;

                case ArgumentException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "Некорректные данные.",
                        Details = exception.Message
                    };
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response); // превращаем ответ в json строку
            context.Response.ContentType = "application/json"; // Устанавливает заголовок Content-Type: application/json
            await context.Response.WriteAsync(jsonResponse); // отправляем ответ
        }
    }
}