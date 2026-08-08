using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTOs;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly IResultRecordService _resultService;

        public ResultsController(IResultRecordService resultService)
        {
            _resultService = resultService;
        }

        [HttpGet] // атрибут говорит ASP.NET Core: "Этот метод обрабатывает GET запросы"
                  // принимает параметры фильтрации из URL,передает их в сервис,возвращает отфильтрованные записи
        public async Task<IActionResult> GetResults([FromQuery] FilterRequestDto filter)
        {
            var results = await _resultService.FilterAsync(filter);
            return Ok(results);
        }
    }
}
