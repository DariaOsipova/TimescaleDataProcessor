using Xunit;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq; // Single()-проверка кол-ва
using System.Threading.Tasks; // использование коллекций(List)

namespace Tests.UnitTests.Infrastructure.Repositories
{
    public class ValueRecordRepositoryTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                              .UseInMemoryDatabase(Guid.NewGuid().ToString()) // каждый тест получает свою чистую уникальную бд
                              .Options;
            return new AppDbContext(options);
        }

        [Fact] // это тест
        public async Task AddRangeAsync_ShouldSaveRecords()
        {
            var context = GetDbContext(); // получаем объект для работы с БД
            var repo = new ValueRecordRepository(context); // создаем репозиторий для работы с БД
            var records =
                new List<ValueRecord> { new ValueRecord(DateTime.UtcNow, 1.5, 10.0, "test.csv") }; // 1.5-Время выполнения,10-значение

            // асинхронный значит поток не блокируется во время выполнения операции с БД
            await repo.AddRangeAsync(records);

            var saved = await context.ValueRecords.ToListAsync();
            Assert.Single(saved);
            Assert.Equal("test.csv", saved.First().FileName);
        }

        [Fact] // это тест
        public async Task GetLast10ByFileNameAsync_ShouldReturnLast10()
        {
            var context = GetDbContext(); // получаем объект для работы с БД
            var repo = new ValueRecordRepository(context); // создаем репозиторий для работы с БД
            var now = DateTime.UtcNow;

            for (int i = 0; i < 15; i++)
            {
                context.ValueRecords.Add(new ValueRecord(now.AddMinutes(i), i * 0.5, i * 10, "test.csv")); // i*0.5 для ExecutionTime,i * 10-для value
            }
            await context.SaveChangesAsync();

            var result = await repo.GetLast10ByFileNameAsync("test.csv");


            Assert.Equal(10, result.Count());
            Assert.Equal(now.AddMinutes(14), result.First().Date);
            Assert.Equal(now.AddMinutes(5), result.Last().Date);
        }

        [Fact] // это тест
        public async Task DeleteByFileNameAsync_ShouldDeleteAllRecords()
        {
            var context = GetDbContext(); // получаем объект для работы с БД
            var repo = new ValueRecordRepository(context); // создаем репозиторий для работы с БД

            for (int i = 0; i < 5; i++)
            {
                context.ValueRecords.Add(
                    new ValueRecord(DateTime.UtcNow.AddMinutes(i), i * 0.5, i * 10, "test.csv")); // i*0.5 для ExecutionTime,i * 10-для value
            }
            await context.SaveChangesAsync();

            await repo.DeleteByFileNameAsync("test.csv");

            var remaining = await context.ValueRecords.ToListAsync();
            Assert.Empty(remaining);
        }

        [Fact] // это тест
        public async Task DeleteByFileNameAsync_WithNoRecords_ShouldNotThrow()
        { // проверяем,что метод не выбрасывает исключение,даже если файла не существует
            var context = GetDbContext(); // получаем объект для работы с БД
            var repo = new ValueRecordRepository(context); // создаем репозиторий для работы с БД

            await repo.DeleteByFileNameAsync("nonexistent.csv");
            var records = await context.ValueRecords.ToListAsync();
            Assert.Empty(records);
        }

        [Fact] // это тест
        public async Task GetByFileNameAsync_ShouldReturnCorrectRecords()
        {
            var context = GetDbContext(); // получаем объект для работы с БД
            var repo = new ValueRecordRepository(context); // создаем репозиторий для работы с БД

            context.ValueRecords.Add(new ValueRecord(DateTime.UtcNow, 1.0, 10.0, "file1.csv")); // ExecutionTime,Value,FileName
            context.ValueRecords.Add(
                new ValueRecord(DateTime.UtcNow.AddMinutes(1), 2.0, 20.0, "file1.csv"));
            context.ValueRecords.Add(
                new ValueRecord(DateTime.UtcNow.AddMinutes(2), 3.0, 30.0, "file2.csv"));
            await context.SaveChangesAsync();

            var result = await repo.GetByFileNameAsync("file1.csv");

            Assert.Equal(2, result.Count()); // Проверяем равенство
            Assert.All(result, r => Assert.Equal("file1.csv", r.FileName));
        }
    }
}
