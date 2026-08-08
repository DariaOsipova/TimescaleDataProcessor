using System; // базовые типы(DataTime)
using System.Collections.Generic; // IEnumerable<T>-интерфейс-коллекция объектов,их можно перебирать
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class ResultRecordRepository : IResultRecordRepository
    { // реализует интерфейс IResultRecordRepository
        private readonly AppDbContext _context;

        public ResultRecordRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResultRecord?> GetByFileNameAsync(string fileName)
        {
            return await _context.ResultRecords.FirstOrDefaultAsync(r => r.FileName == fileName); // найти первую запись
        }

        public async Task AddAsync(ResultRecord result)
        {
            await _context.ResultRecords.AddAsync(result);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ResultRecord result)
        {
            _context.ResultRecords.Update(result);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ResultRecord>> FilterAsync( // коллекция записей,которую можно перебирать
            string? fileName = null, DateTime? minDate = null, DateTime? maxDate = null,
            double? minAvgValue = null, double? maxAvgValue = null, double? minAvgExecutionTime = null,
            double? maxAvgExecutionTime = null)
        {
            var query = _context.ResultRecords.AsQueryable();

            if (!string.IsNullOrWhiteSpace(fileName))
                query = query.Where(r => r.FileName.Contains(fileName)); // фильтр по имени

            if (minDate.HasValue)
                query = query.Where(r => r.MinDate >= minDate.Value);

            if (maxDate.HasValue)
                query = query.Where(r => r.MinDate <= maxDate.Value);

            if (minAvgValue.HasValue)
                query = query.Where(r => r.AvgValue >= minAvgValue.Value);

            if (maxAvgValue.HasValue)
                query = query.Where(r => r.AvgValue <= maxAvgValue.Value);

            if (minAvgExecutionTime.HasValue)
                query = query.Where(r => r.AvgExecutionTime >= minAvgExecutionTime.Value);

            if (maxAvgExecutionTime.HasValue)
                query = query.Where(r => r.AvgExecutionTime <= maxAvgExecutionTime.Value);

            return await query.OrderByDescending(r => r.ProcessedAt).ToListAsync();
            //Descending(r => r.ProcessedAt)-сортировка по убыванию(от новых к старым) по полю ProcessedAt(дата/время обработки записи)
        }
    }
}