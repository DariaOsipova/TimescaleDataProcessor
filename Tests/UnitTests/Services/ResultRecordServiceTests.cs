using Xunit;
using Moq; // для создания фальшивых объектов (mocks) для тестирования
using Application.Services;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using AutoMapper; // для автоматического преобразования объектов одного типа в другой

namespace Tests.UnitTests.Services
{
    public class ResultRecordServiceTests
    {
        private IMapper GetMapper()
        {
            var config =
                // Добавляем профиль с настройками маппинга
                new MapperConfiguration(cfg => cfg.AddProfile<Application.Mappings.MappingProfile>());
            return config.CreateMapper();
        }

        [Fact] // это тест
        public async Task FilterAsync_ShouldReturnFilteredResults()
        {
            var mockRepository = new Mock<IResultRecordRepository>();
            var mapper = GetMapper();
            //60:разница между первой и последней датой=1 минута,DateTime.UtcNow.AddDays(-1)-Вчерашняя дата,среднее время,среднее значение
            var results = new List<ResultRecord> {
        new ResultRecord("file1.csv", 60, DateTime.UtcNow.AddDays(-1), 2.5, 15.0, 15.0, 20.0, 10.0),
        new ResultRecord("file2.csv", 120, DateTime.UtcNow.AddDays(-2), 3.0, 25.0, 25.0, 30.0, 20.0)
      };

            mockRepository
                .Setup(r => r.FilterAsync(It.IsAny<string?>(), It.IsAny<DateTime?>(),
                                          It.IsAny<DateTime?>(), It.IsAny<double?>(), It.IsAny<double?>(),
                                          It.IsAny<double?>(), It.IsAny<double?>()))
                .ReturnsAsync(results); // .Setup()-Настраивает поведение фальшивого объекта,r-Фальшивый объект(репозиторий)

            var service = new ResultRecordService(mockRepository.Object, mapper);

            var filter = new FilterRequestDto { FileName = "file1" };

            var result = await service.FilterAsync(filter);
            var resultList = result.ToList();

            Assert.Equal(2, resultList.Count); // Проверяем равенство
            mockRepository.Verify(
                r => r.FilterAsync(filter.FileName, filter.MinDate, filter.MaxDate, filter.MinAvgValue,
                                   filter.MaxAvgValue, filter.MinAvgExecutionTime,
                                   filter.MaxAvgExecutionTime),
                Times.Once); //  проверка,что метод был вызван с определенными параметрами 1 раз
        }
    }
}