using Xunit;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using System;
using System.Linq; // Single()-проверка кол-ва,count
using System.Threading.Tasks;

namespace Tests.UnitTests.Infrastructure.Repositories
{
    public class ResultRecordRepositoryTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                              .UseInMemoryDatabase(Guid.NewGuid().ToString()) // каждый тест получает свою чистую уникальную бд
                              .Options;
            return new AppDbContext(options);
        }

        [Fact] // это тест
        public async Task AddAsync_ShouldSaveResult()
        {
            var context = GetDbContext(); // получаем объект для работы с БД
            var repo = new ResultRecordRepository(context); // создаем репозиторий для работы с БД
                                                            //FileName,DeltaTimeSeconds-Разница во времени в секундах,MinDate,AvgExecutionTime-среднее время выполнения,
                                                            //DateTime.UtcNow-Самая ранняя дата в данных,MedianValue-срединное значение
            var record = new ResultRecord("test.csv", 120, DateTime.UtcNow, 2.0, 20.0, 20.0, 30.0, 10.0);

            // асинхронный значит поток не блокируется во время выполнения операции с БД
            await repo.AddAsync(record);

            var saved = await context.ResultRecords.FirstOrDefaultAsync();
            Assert.NotNull(saved);
            Assert.Equal("test.csv", saved.FileName);
        }

        [Fact] // это тест
        public async Task GetByFileNameAsync_ShouldReturnCorrectRecord()
        {
            var context = GetDbContext();// получаем объект для работы с БД
            var repo = new ResultRecordRepository(context); // создаем репозиторий для работы с БД
            var record = new ResultRecord("test.csv", 120, DateTime.UtcNow, 2.0, 20.0, 20.0, 30.0, 10.0);
            await context.ResultRecords.AddAsync(record);
            await context.SaveChangesAsync();

            var result = await repo.GetByFileNameAsync("test.csv");

            Assert.NotNull(result);
            Assert.Equal("test.csv", result.FileName);
        }

        [Fact] // это тест
        public async Task GetByFileNameAsync_WithNonExistentFile_ShouldReturnNull()
        {
            var context = GetDbContext();
            var repo = new ResultRecordRepository(context); // создаем репозиторий для работы с БД
            var result = await repo.GetByFileNameAsync("nonexistent.csv");

            Assert.Null(result); // если файл не найден
        }

        [Fact] // это тест
        public async Task UpdateAsync_ShouldUpdateRecord()
        { // сохраняем запись в бд перед обновлением
            var context = GetDbContext();
            var repo = new ResultRecordRepository(context); // создаем репозиторий для работы с БД
            var record = new ResultRecord("test.csv", 120, DateTime.UtcNow, 2.0, 20.0, 20.0, 30.0, 10.0);
            await context.ResultRecords.AddAsync(record);
            await context.SaveChangesAsync();

            record.Update("updated.csv", 180, DateTime.UtcNow.AddDays(-1), 3.0, 30.0, 30.0, 40.0, 20.0);
            await repo.UpdateAsync(record);

            var updated = await context.ResultRecords.FirstOrDefaultAsync();
            Assert.NotNull(updated);
            Assert.Equal("updated.csv", updated.FileName);
            Assert.Equal(180, updated.DeltaTimeSeconds);
        }

        [Fact] // это тест
        public async Task FilterAsync_WithFileNameFilter_ShouldReturnFilteredResults()
        {
            var context = GetDbContext();
            var repo = new ResultRecordRepository(context);

            context.ResultRecords.Add(
                new ResultRecord("file1.csv", 120, DateTime.UtcNow, 2.0, 20.0, 20.0, 30.0, 10.0));
            context.ResultRecords.Add(new ResultRecord("file2.csv", 180, DateTime.UtcNow.AddDays(-1), 3.0,
                                                       30.0, 30.0, 40.0, 20.0));
            await context.SaveChangesAsync();

            var results = await repo.FilterAsync(fileName: "file1");

            Assert.Single(results);
            Assert.Equal("file1.csv", results.First().FileName);
        }

        [Fact] // это тест
        public async Task FilterAsync_WithDateRange_ShouldReturnFilteredResults()
        {
            var context = GetDbContext();
            var repo = new ResultRecordRepository(context);
            var date1 = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var date2 = new DateTime(2024, 1, 2, 10, 0, 0, DateTimeKind.Utc);
            var date3 = new DateTime(2024, 1, 3, 10, 0, 0, DateTimeKind.Utc);

            context.ResultRecords.Add(
                new ResultRecord("file1.csv", 120, date1, 2.0, 20.0, 20.0, 30.0, 10.0));
            context.ResultRecords.Add(
                new ResultRecord("file2.csv", 180, date2, 3.0, 30.0, 30.0, 40.0, 20.0));
            context.ResultRecords.Add(
                new ResultRecord("file3.csv", 200, date3, 4.0, 40.0, 40.0, 50.0, 30.0));
            await context.SaveChangesAsync();

            var results = await repo.FilterAsync(minDate: date1, maxDate: date2);

            Assert.Equal(2, results.Count()); // Проверяем равенство
        }

        [Fact] // это тест
        public async Task FilterAsync_WithAvgValueRange_ShouldReturnFilteredResults()
        {
            var context = GetDbContext();
            var repo = new ResultRecordRepository(context);

            context.ResultRecords.Add(
                new ResultRecord("file1.csv", 120, DateTime.UtcNow, 2.0, 15.0, 15.0, 30.0, 10.0));
            context.ResultRecords.Add(
                new ResultRecord("file2.csv", 180, DateTime.UtcNow, 3.0, 25.0, 25.0, 40.0, 20.0));
            context.ResultRecords.Add(
                new ResultRecord("file3.csv", 200, DateTime.UtcNow, 4.0, 35.0, 35.0, 50.0, 30.0));
            await context.SaveChangesAsync();

            var results = await repo.FilterAsync(minAvgValue: 20, maxAvgValue: 30);

            Assert.Single(results);
            Assert.Equal(25.0, results.First().AvgValue);
        }
    }
}