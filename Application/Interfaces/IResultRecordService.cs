using System.Collections.Generic; // Для IEnumerable<ResultRecordDto>
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IResultRecordService
    { // определяет, какие операции доступны для работы с записями результатов
        Task<IEnumerable<ResultRecordDto>> FilterAsync(FilterRequestDto filter);
        // возвращаемый тип асинхронного метода, который возвращает коллекцию DTO (ResultRecordDto).
    }
}