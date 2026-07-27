using Xunit;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic; // списки, словари, множества
using System.Linq; // (.First(),filtered)
using System.Threading.Tasks;

namespace Tests.IntegrationTests.Repositories
{ // IDisposable-интерфейс, который говорит:"У этого класса есть ресурсы, которые нужно освободить после использования"
    public class ResultRecordRepositoryFilterTests : IDisposable
    {
        private readonly AppDbContext _context; // можно изменить только в конструкторе
        private readonly ResultRecordRepository _repository;

        public ResultRecordRepositoryFilterTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new ResultRecordRepository(_context);
        }

        public void Dispose() // метод очищает ресурсы после выполнения каждого теста
        {
            _context.Database.EnsureDeleted();
            _context.Dispose(); // Контекст закрывается, соединения освобождаются
        }

        [Fact] // это тест
        public async Task FilterAsync_ByFileName_ShouldReturnFilteredResults()
        {
            var results = new[]
            { // deltaTimeSeconds,minDate,avgExecutionTime,avgValue,maxValue,minValue
                new ResultRecord("file1.csv", 100.5, DateTime.UtcNow.AddDays(-5), 4.5, 150.5, 155.0, 200.3, 100.5),
                new ResultRecord("file2.csv", 200.5, DateTime.UtcNow.AddDays(-3), 6.5, 250.5, 260.0, 300.3, 200.5),
                new ResultRecord("file3.csv", 300.5, DateTime.UtcNow.AddDays(-1), 8.5, 350.5, 360.0, 400.3, 300.5)
            };
            await _context.ResultRecords.AddRangeAsync(results);
            await _context.SaveChangesAsync();

            var filtered = await _repository.FilterAsync(
                fileName: "file1.csv",
                minDate: null, // фильтр отключен
                maxDate: null,
                minAvgValue: null,
                maxAvgValue: null,
                minAvgExecutionTime: null,
                maxAvgExecutionTime: null
            );

            Assert.Single(filtered); // Assert--проверка результата
            Assert.Equal("file1.csv", filtered.First().FileName); // проверяем что значения одинаковы
        }

        [Fact]
        public async Task FilterAsync_ByDateRange_ShouldReturnFilteredResults() // диапозон дат
        {
            var baseDate = DateTime.UtcNow;
            var results = new[]
            { // deltaTimeSeconds,minDate,avgExecutionTime,avgValue,maxValue,minValue
                new ResultRecord("file1.csv", 100.5, baseDate.AddDays(-10), 4.5, 150.5, 155.0, 200.3, 100.5),
                new ResultRecord("file2.csv", 200.5, baseDate.AddDays(-5), 6.5, 250.5, 260.0, 300.3, 200.5),
                new ResultRecord("file3.csv", 300.5, baseDate.AddDays(-1), 8.5, 350.5, 360.0, 400.3, 300.5)
            };
            await _context.ResultRecords.AddRangeAsync(results);
            await _context.SaveChangesAsync();

            var filtered = await _repository.FilterAsync(
                fileName: null,
                minDate: baseDate.AddDays(-7), // все записи не ранне даты(сегодня-7)
                maxDate: baseDate,
                minAvgValue: null,
                maxAvgValue: null,
                minAvgExecutionTime: null,
                maxAvgExecutionTime: null
            );

            Assert.Equal(2, filtered.Count()); // проверяем что значения одинаковы
            Assert.Contains(filtered, r => r.FileName == "file2.csv"); // проверка,что коллекция filtered содержит запись с именем файла "file2.csv"
            Assert.Contains(filtered, r => r.FileName == "file3.csv");
        }

        [Fact] // это тест
        public async Task FilterAsync_ByAvgValueRange_ShouldReturnFilteredResults()
        {
            var results = new[]
            {
                // deltaTimeSeconds,minDate,avgExecutionTime,avgValue,maxValue,minValue
                new ResultRecord("file1.csv", 100.5, DateTime.UtcNow, 4.5, 150.5, 155.0, 200.3, 100.5),
                new ResultRecord("file2.csv", 200.5, DateTime.UtcNow, 6.5, 250.5, 260.0, 300.3, 200.5),
                new ResultRecord("file3.csv", 300.5, DateTime.UtcNow, 8.5, 350.5, 360.0, 400.3, 300.5)
            };
            await _context.ResultRecords.AddRangeAsync(results);
            await _context.SaveChangesAsync();

            var filtered = await _repository.FilterAsync(
                fileName: null,
                minDate: null,
                maxDate: null,
                minAvgValue: 200.0,
                maxAvgValue: 300.0,
                minAvgExecutionTime: null,
                maxAvgExecutionTime: null
            );

            Assert.Single(filtered); // проверка,что коллекция filtered содержит ровно 1 элемент
            Assert.Equal(250.5, filtered.First().AvgValue); // проверяем что значения одинаковы
        }

        [Fact]  // это тест
        public async Task FilterAsync_ByAvgExecutionTimeRange_ShouldReturnFilteredResults()
        {
            var results = new[]
            {
                new ResultRecord("file1.csv", 100.5, DateTime.UtcNow, 4.5, 150.5, 155.0, 200.3, 100.5),
                new ResultRecord("file2.csv", 200.5, DateTime.UtcNow, 6.5, 250.5, 260.0, 300.3, 200.5),
                new ResultRecord("file3.csv", 300.5, DateTime.UtcNow, 8.5, 350.5, 360.0, 400.3, 300.5)
            };
            await _context.ResultRecords.AddRangeAsync(results);
            await _context.SaveChangesAsync();

            var filtered = await _repository.FilterAsync(
                fileName: null,
                minDate: null,
                maxDate: null,
                minAvgValue: null,
                maxAvgValue: null,
                minAvgExecutionTime: 5.0,
                maxAvgExecutionTime: 7.0
            );

            Assert.Single(filtered); // проверка,что коллекция filtered содержит ровно 1 элемент
            Assert.Equal(6.5, filtered.First().AvgExecutionTime); // проверяем что значения одинаковы
        }

        [Fact] // это тест
        public async Task FilterAsync_WithAllFilters_ShouldReturnFilteredResults() // все фильтры
        {
            var baseDate = DateTime.UtcNow;
            var results = new[]
            {
                new ResultRecord("file1.csv", 100.5, baseDate.AddDays(-10), 4.5, 150.5, 155.0, 200.3, 100.5),
                new ResultRecord("file2.csv", 200.5, baseDate.AddDays(-5), 6.5, 250.5, 260.0, 300.3, 200.5),
                new ResultRecord("file3.csv", 300.5, baseDate.AddDays(-1), 8.5, 350.5, 360.0, 400.3, 300.5)
            };
            await _context.ResultRecords.AddRangeAsync(results);
            await _context.SaveChangesAsync();

            // Act
            var filtered = await _repository.FilterAsync(
                fileName: "file2.csv",
                minDate: baseDate.AddDays(-7),
                maxDate: baseDate,
                minAvgValue: 200.0,
                maxAvgValue: 300.0,
                minAvgExecutionTime: 5.0,
                maxAvgExecutionTime: 7.0
            );

            Assert.Single(filtered);
            Assert.Equal("file2.csv", filtered.First().FileName);
            Assert.Equal(6.5, filtered.First().AvgExecutionTime);
            Assert.Equal(250.5, filtered.First().AvgValue); // проверяем что значения одинаковы
        }
    }
}