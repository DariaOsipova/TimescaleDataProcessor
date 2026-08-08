using System;
using System.Collections.Generic; // 
using System.Linq; // 
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class ValueRecordRepository : IValueRecordRepository
    { // реализует интерфейс IValueRecordRepository
        private readonly AppDbContext _context;

        public ValueRecordRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<ValueRecord> records)
        { // множество записей в бд за одну операцию
          //IEnumerable-тип параметра(Принимает любую коллекцию,которую можно перебирать),
            await _context.ValueRecords.AddRangeAsync(records);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ValueRecord>> GetLast10ByFileNameAsync(string fileName)
        { // коллекция записей
            return await _context.ValueRecords.Where(v => v.FileName == fileName)
                .OrderByDescending(v => v.Date)
                .Take(10)
                .ToListAsync();
        } // последние 10 записей для указанного файла,отсортированные по дате(сначала новые)

        public async Task DeleteByFileNameAsync(string fileName)
        {
            var records = await _context.ValueRecords.Where(v => v.FileName == fileName).ToListAsync();

            if (records.Any())
            { // если есть записи-удаляем их все
                _context.ValueRecords.RemoveRange(records);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ValueRecord>> GetByFileNameAsync(string fileName)
        {
            return await _context.ValueRecords.Where(v => v.FileName == fileName)
                .OrderBy(v => v.Date)
                .ToListAsync();
        }
    }
}
