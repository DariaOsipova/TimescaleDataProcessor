using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTOs;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly IValueRecordService _valueService;

        public ValuesController(IValueRecordService valueService)
        {
            _valueService = valueService;
        }

        // МЕТОД 1: Загрузка CSV файла
        [HttpPost("upload")] // Атрибут говорит:"Этот метод обрабатывает POST запросы по адресу /api/values/upload"
                             // async Task- Возвращает результат(JSON),IActionResult-Имя метода
        public async Task<IActionResult> UploadCsv(IFormFile file)
        { // параметр UploadCsv(IFormFile file)-загруженный файл
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "Файл не выбран." });

            try
            {
                var result = await _valueService.ProcessCsvAsync(file);
                return Ok(new { message = "Файл успешно обработан.", result });
            }
            catch (Domain.Exceptions.CsvValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Внутренняя ошибка сервера.", details = ex.Message });
            }
        }

        // МЕТОД: Получение последних 10 значений по имени файла
        [HttpGet("last10")]
        public async Task<IActionResult> GetLast10Values([FromQuery] string fileName)
        { // интерфейс представляет результат действия контроллера.Он говорит ASP.NET Core, что вернуть клиенту
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest(new { error = "Имя файла обязательно." });

            try
            {
                var values = await _valueService.GetLast10ValuesByFileNameAsync(fileName); // записи файла
                return Ok(values); // HTTP статус 200 (успешно
            }
            catch (Domain.Exceptions.CsvValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Внутренняя ошибка сервера.", details = ex.Message });
            }
        }
    }
}
