using System;
using System.Collections.Generic; // для <IEnumerable<ResultRecord>, коллекции, списки, словари
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IResultRecordRepository
    {
        Task<ResultRecord?> GetByFileNameAsync(string fileName); // параллельное выполнение задач
        Task AddAsync(ResultRecord result);
        Task UpdateAsync(ResultRecord result);
        // метод фильтрации ищет записи ResultRecord по критериям(все критерии должны совпасть)
        Task<IEnumerable<ResultRecord>> FilterAsync(string? fileName = null, DateTime? minDate = null,
                                                DateTime? maxDate = null,
                                                double? minAvgValue = null,
                                                double? maxAvgValue = null,
                                                double? minAvgExecutionTime = null,
                                                double? maxAvgExecutionTime = null);
    }
}