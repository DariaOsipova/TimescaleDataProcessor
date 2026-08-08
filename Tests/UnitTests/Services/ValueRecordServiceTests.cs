using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Domain.Entities;
using Domain.Interfaces;
using Application.Services;
using Application.DTOs;
using AutoMapper; // для автоматического преобразования объектов одного типа в другой
using System.Text;
using Domain.Exceptions;

namespace Tests.UnitTests.Services
{
    public class ValueRecordServiceTests
    {
        private readonly Mock<IValueRecordRepository> _valueRepoMock;
        private readonly Mock<IResultRecordRepository> _resultRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ValueRecordService _service; // поле класса хранит ссылку на тестируемый сервис

        public ValueRecordServiceTests()
        {
            _valueRepoMock = new Mock<IValueRecordRepository>();
            _resultRepoMock = new Mock<IResultRecordRepository>();
            _mapperMock = new Mock<IMapper>();
            _service =
                // создание реального сервиса с фальшивыми(mock) зависимостями для тестирования,.Object-свойство, которое дает доступ к самому фальшивому объекту
                new ValueRecordService(_valueRepoMock.Object, _resultRepoMock.Object, _mapperMock.Object);
        }

        [Fact] // это тест
        public async Task ProcessCsvAsync_ValidFile_ShouldReturnSuccess()
        { // метод ProcessCsvAsync успешно обрабатывает валидный CSV файл
            var content = "Date;ExecutionTime;Value\n2024-01-01T10:00:00.000Z;1.5;10.0";
            var fileMock = CreateFileMock(content, "test.csv");

            _valueRepoMock.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<ValueRecord>>()))
                .Returns(Task.CompletedTask); // завершенная задача
                                              // _valueRepoMock-фальшивый репозиторий,r => r.AddRangeAsync-Вызываем метод AddRangeAsync у репозитория,It.IsAny<IEnumerable<ValueRecord>>()-с любыми параметрами (любым списком ValueRecord)
            _resultRepoMock.Setup(r => r.GetByFileNameAsync(It.IsAny<string>()))
          .ReturnsAsync((ResultRecord?)null); // При вызове этого метода верни null
            _resultRepoMock.Setup(r => r.AddAsync(It.IsAny<ResultRecord>())).Returns(Task.CompletedTask);

            var result = await _service.ProcessCsvAsync(fileMock); // вызов тестируемого метода сервиса с фальшивым файлом

            Assert.NotNull(result); // проверка,что переменная result не равна null
            Assert.Equal("test.csv", result.FileName); // Проверяем равенство
            Assert.Equal(1, result.RecordsCount);
        }

        [Fact] // это тест
        public async Task ProcessCsvAsync_WithInvalidDateFormat_ShouldThrowException()
        {
            //создаем тест,который проверяет,что метод выбрасывает исключение,тестовые данные с неправильным форматом даты
            var content = "Date;ExecutionTime;Value\ninvalid-date;1.5;10.0";
            var fileMock = CreateFileMock(content, "test.csv");
            // проверяем, что метод выбрасывает исключение
            await Assert.ThrowsAsync<CsvValidationException>(() => _service.ProcessCsvAsync(fileMock));
        }

        [Fact] // это тест
        public async Task ProcessCsvAsync_WithNegativeExecutionTime_ShouldThrowException()
        {
            var content = "Date;ExecutionTime;Value\n2024-01-01T10:00:00.000Z;-1.5;10.0";
            var fileMock = CreateFileMock(content, "test.csv");
            // проверяем, что метод выбрасывает исключение
            await Assert.ThrowsAsync<CsvValidationException>(() => _service.ProcessCsvAsync(fileMock));
        }

        [Fact]
        public async Task ProcessCsvAsync_WithNegativeValue_ShouldThrowException()
        {
            var content = "Date;ExecutionTime;Value\n2024-01-01T10:00:00.000Z;1.5;-10.0";
            var fileMock = CreateFileMock(content, "test.csv");

            await Assert.ThrowsAsync<CsvValidationException>(() => _service.ProcessCsvAsync(fileMock));
        }

        [Fact] // это тест
        public async Task ProcessCsvAsync_WithEmptyFile_ShouldThrowException()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);
            fileMock.Setup(f => f.FileName).Returns("empty.csv");

            await Assert.ThrowsAsync<CsvValidationException>(
                () => _service.ProcessCsvAsync(fileMock.Object)); // .Object - сам фальшивый объект, который мы создали с помощью Mock<T>
        }

        [Fact] // это тест
        public async Task ProcessCsvAsync_WithNullFile_ShouldThrowException()
        {
            await Assert.ThrowsAsync<CsvValidationException>(() => _service.ProcessCsvAsync(null!));
        }

        [Fact] // это тест
        public async Task GetLast10ValuesByFileNameAsync_WithEmptyFileName_ShouldThrowException()
        {
            await Assert.ThrowsAsync<CsvValidationException>(
                () => _service.GetLast10ValuesByFileNameAsync(""));
        }

        [Fact] // это тест
        public async Task GetLast10ValuesByFileNameAsync_WithValidFileName_ShouldReturnValues()
        { // пустое имя файла
            var fileName = "test.csv";
            // 1.0-ExecutionTime(время выполнения),10.0-Value(значение)
            var records = new List<ValueRecord> { new ValueRecord(DateTime.UtcNow, 1.0, 10.0, fileName),
                                            new ValueRecord(DateTime.UtcNow.AddMinutes(1), 2.0,
                                                            20.0, fileName) };
            var dtos = records.Select(r => new ValueRecordDto
            {
                Id = r.Id,
                Date = r.Date,
                ExecutionTime = r.ExecutionTime,
                Value = r.Value,
                FileName = r.FileName
            });

            _valueRepoMock.Setup(r => r.GetLast10ByFileNameAsync(fileName)).ReturnsAsync(records);
            _mapperMock.Setup(m => m.Map<IEnumerable<ValueRecordDto>>(records)).Returns(dtos);
            // ValueRecordDto-класс содержит данные для передачи между слоями
            var result = await _service.GetLast10ValuesByFileNameAsync(fileName);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Проверяем равенство
        }

        private IFormFile CreateFileMock(string content, string fileName)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            return fileMock.Object;
        }
    }
}
