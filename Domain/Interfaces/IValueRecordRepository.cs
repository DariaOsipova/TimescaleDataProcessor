using System;
using System.Collections.Generic; // для <IEnumerable<ValueRecord>, коллекции, списки, словари
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IValueRecordRepository
    {
        Task AddRangeAsync(IEnumerable<ValueRecord> records); // множество записей за один раз
        Task<IEnumerable<ValueRecord>> GetLast10ByFileNameAsync(string fileName); // Возвращает последние 10 записей для указанного файла
        Task DeleteByFileNameAsync(string fileName);
        Task<IEnumerable<ValueRecord>> GetByFileNameAsync(string fileName);
    }
}
