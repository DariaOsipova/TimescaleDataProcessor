using System.Collections.Generic; // для IEnumerable<ValueRecordDto>
using System.Threading.Tasks;
using Application.DTOs;
using Microsoft.AspNetCore.Http; // IFormFile-интерфейс представляет загруженный файл в ASP.NET Core

namespace Application.Interfaces
{
    public interface IValueRecordService
    {
        Task<UploadResultDto> ProcessCsvAsync(IFormFile file); //  для ответа на загрузку файла
        Task<IEnumerable<ValueRecordDto>> GetLast10ValuesByFileNameAsync(string fileName);
    }
}
