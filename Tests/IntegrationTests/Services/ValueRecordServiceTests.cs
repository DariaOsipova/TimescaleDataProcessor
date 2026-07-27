using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Application.Services;
using Application.DTOs;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.IO; // memoryStream
using System.Linq; // Чтобы фильтровать, сортировать и искать данные в БД
using System.Text;
using System.Threading.Tasks;

namespace Tests.IntegrationTests.Services
{
    public class ValueRecordServiceTests : IDisposable // IDisposable для очистки ресурсов после тестов
    {
        private readonly AppDbContext _context;
        private readonly ValueRecordService _service;
        private readonly IMapper _mapper;

        public ValueRecordServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Уникальное имя для каждой БД(тесты не мешают друг другу)
                .Options; // Получить готовые настройки

            _context = new AppDbContext(options);

            var config = new MapperConfiguration(cfg => // правила преобразования между типами,cfg-Параметр-объект для настройки маппинга-
                                                        // (IMapperConfigurationExpression),Лямбда-оператор-передать cfg в выражение
            {
                cfg.CreateMap<ValueRecord, ValueRecordDto>();
                cfg.CreateMap<ValueRecordDto, ValueRecord>();
            });
            _mapper = config.CreateMapper();

            var valueRepo = new ValueRecordRepository(_context);
            var resultRepo = new ResultRecordRepository(_context);
            _service = new ValueRecordService(valueRepo, resultRepo, _mapper); // resultRepo-репозиторий ResultRecord,Создает реальный сервис и внедряет в него зависимости
        }

        public void Dispose() // реализует интерфейс IDisposable(для освобождения неуправляемых ресурсов (подключения к БД, файлы, сетевые соединения)
        {
            _context.Database.EnsureDeleted(); // Удаляет БД,если она существует
            _context.Dispose(); // Закрывает соединение,освобождает память
        }

        private IFormFile CreateFormFile(string content, string fileName)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var fileMock = new Mock<IFormFile>();

            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.ContentType).Returns("text/csv"); // настраивает мок так, чтобы при обращении к свойству ContentType возвращался тип "text/csv"
            fileMock.Setup(f => f.ContentDisposition) // при обращении к свойству ContentDisposition возвращалась строка с информацией о файле для HTTP-запроса
                   .Returns($"form-data; name=\"file\"; filename=\"{fileName}\"");

            return fileMock.Object;
        }

        [Fact] // метод является тестом
        public async Task ProcessCsvAsync_ShouldSaveData_WhenValid()
        {
            var content = @"Date;ExecutionTime;Value
2024-01-01T10:00:00.000Z;5.5;100.5
2024-01-01T10:01:00.000Z;3.2;200.3
2024-01-01T10:02:00.000Z;4.8;150.7";

            var file = CreateFormFile(content, "test.csv");
            var result = await _service.ProcessCsvAsync(file); // Не блокирует поток

            // Assert-проверки
            Assert.NotNull(result);
            Assert.Equal("test.csv", result.FileName); // значения равны
            Assert.Equal(3, result.RecordsCount); // значения равны
            Assert.True(result.IsNewFile);

            var savedValues = await _context.ValueRecords
                .Where(v => v.FileName == "test.csv") //  Фильтрует записи- оставляет только те, у которых FileName = "test.csv"
                .ToListAsync();
            Assert.Equal(3, savedValues.Count);

            var savedResult = await _context.ResultRecords
                .FirstOrDefaultAsync(r => r.FileName == "test.csv");
            Assert.NotNull(savedResult);
        }

        [Fact] // метод является тестом
        public async Task ProcessCsvAsync_ShouldOverwrite_WhenFileExists()
        {
            var content1 = @"Date;ExecutionTime;Value
2024-01-01T10:00:00.000Z;5.5;100.5";

            var content2 = @"Date;ExecutionTime;Value
2024-01-01T10:00:00.000Z;5.5;200.5
2024-01-01T10:01:00.000Z;3.2;300.3";

            var file1 = CreateFormFile(content1, "test.csv");
            var file2 = CreateFormFile(content2, "test.csv");

            await _service.ProcessCsvAsync(file1);
            var result = await _service.ProcessCsvAsync(file2);

            Assert.False(result.IsNewFile); // Assert.False-Значение должно быть false-файл уже существовал,перезапись,result.IsNewFile-Флаг "новый файл" у результата,
            var savedValues = await _context.ValueRecords
                .Where(v => v.FileName == "test.csv")
                .ToListAsync();
            Assert.Equal(2, savedValues.Count); // значения равны
        }

        [Fact] // метод является тестом
        public async Task ProcessCsvAsync_ShouldThrowException_WhenDateInvalid()
        {
            var content = @"Date;ExecutionTime;Value
2099-01-01T10:00:00.000Z;5.5;100.5";

            var file = CreateFormFile(content, "invalid.csv");
            await Assert.ThrowsAsync<Domain.Exceptions.CsvValidationException>(() =>
                _service.ProcessCsvAsync(file));
        }

        [Fact] // метод является тестом
        public async Task ProcessCsvAsync_ShouldThrowException_WhenExecutionTimeNegative()
        {
            var content = @"Date;ExecutionTime;Value
2024-01-01T10:00:00.000Z;-5.5;100.5";

            var file = CreateFormFile(content, "negative.csv");

            await Assert.ThrowsAsync<Domain.Exceptions.CsvValidationException>(() =>
                _service.ProcessCsvAsync(file));
        }

        [Fact] // метод является тестом
        public async Task ProcessCsvAsync_ShouldThrowException_WhenValueNegative()
        {
            var content = @"Date;ExecutionTime;Value
2024-01-01T10:00:00.000Z;5.5;-100.5";

            var file = CreateFormFile(content, "negative_value.csv");

            await Assert.ThrowsAsync<Domain.Exceptions.CsvValidationException>(() =>
                _service.ProcessCsvAsync(file));
        }

        [Fact] // метод является тестом
        public async Task ProcessCsvAsync_ShouldThrowException_WhenMoreThan10000Rows()
        {
            var lines = new List<string> { "Date;ExecutionTime;Value" };
            for (int i = 0; i < 10001; i++)
            {
                lines.Add($"2024-01-01T10:{i:D2}:00.000Z;5.5;100.5");
            }
            var content = string.Join("\n", lines); // Join- объединяет элементы коллекции в одну строку с разделителем

            var file = CreateFormFile(content, "too_many.csv");

            await Assert.ThrowsAsync<Domain.Exceptions.CsvValidationException>(() =>
                _service.ProcessCsvAsync(file));
        }

        [Fact] // метод является тестом
        public async Task ProcessCsvAsync_ShouldThrowException_WhenFileEmpty()
        {
            var content = @"Date;ExecutionTime;Value";
            var file = CreateFormFile(content, "empty.csv");

            await Assert.ThrowsAsync<Domain.Exceptions.CsvValidationException>(() =>
                _service.ProcessCsvAsync(file));
        }

        [Fact] // метод является тестом
        public async Task GetLast10ValuesByFileNameAsync_ShouldReturnLast10Records() // последние 10 записей для указанного файла,отсортированные по дате(от самой новой к самой старой)
        {
            var fileName = "test.csv";
            var content = @"Date;ExecutionTime;Value
2024-01-01T10:00:00.000Z;5.5;100.5
2024-01-01T10:01:00.000Z;3.2;200.3
2024-01-01T10:02:00.000Z;4.8;150.7
2024-01-01T10:03:00.000Z;6.1;180.2
2024-01-01T10:04:00.000Z;2.9;220.1
2024-01-01T10:05:00.000Z;5.0;170.8
2024-01-01T10:06:00.000Z;7.2;190.4
2024-01-01T10:07:00.000Z;4.1;160.9
2024-01-01T10:08:00.000Z;5.8;210.6
2024-01-01T10:09:00.000Z;3.5;240.3
2024-01-01T10:10:00.000Z;6.3;230.7";

            var file = CreateFormFile(content, fileName);
            await _service.ProcessCsvAsync(file);

            var result = await _service.GetLast10ValuesByFileNameAsync(fileName);
            Assert.Equal(10, result.Count()); // значения равны
            var sorted = result.OrderByDescending(r => r.Date).ToList(); // от самой новой даты
            Assert.Equal(DateTime.Parse("2024-01-01T10:10:00.000Z"), sorted[0].Date); // строку в DateTime сравниваем с отсортированным списком
        }
    }
}