using Xunit;
using Domain.Entities;
using System;

namespace Tests.UnitTests.Domain.Entities
{
    public class ResultRecordTests
    {
        [Fact] // это тест
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            var fileName = "test.csv";
            var deltaTime = 120.5; // разница во времени между самой ранней и самой поздней датой в данных
            var minDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var avgExecutionTime = 2.5;
            var avgValue = 25.0;
            var medianValue = 25.0;
            var maxValue = 30.0;
            var minValue = 10.0;

            // // создание объекта
            var record = new ResultRecord(fileName, deltaTime, minDate, avgExecutionTime, avgValue,
                                          medianValue, maxValue, minValue);

            // Assert--проверка результата
            Assert.NotEqual(Guid.Empty, record.Id);
            Assert.Equal(fileName, record.FileName); // проверяем что значения одинаковы
            Assert.Equal(deltaTime, record.DeltaTimeSeconds);
            Assert.Equal(minDate, record.MinDate);
            Assert.Equal(avgExecutionTime, record.AvgExecutionTime);
            Assert.Equal(avgValue, record.AvgValue);
            Assert.Equal(medianValue, record.MedianValue);
            Assert.Equal(maxValue, record.MaxValue);
            Assert.Equal(minValue, record.MinValue);
            Assert.True(record.ProcessedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void Update_ShouldChangeAllProperties()
        { // тест проверяет,что метод Update изменяет все свойства объекта ResultRecord
          // Arrange
            var record = new ResultRecord("old.csv", 60, DateTime.UtcNow, 2.0, 20.0, 20.0, 30.0, 10.0);
            var newFileName = "new.csv";
            var newDeltaTime = 180.0;
            var newMinDate = new DateTime(2024, 2, 1, 10, 0, 0, DateTimeKind.Utc);
            var newAvgExecutionTime = 3.0;
            var newAvgValue = 30.0;
            var newMedianValue = 30.0;
            var newMaxValue = 40.0;
            var newMinValue = 20.0;

            record.Update(newFileName, newDeltaTime, newMinDate, newAvgExecutionTime, newAvgValue,
                          newMedianValue, newMaxValue, newMinValue);

            Assert.Equal(newFileName, record.FileName);
            Assert.Equal(newDeltaTime, record.DeltaTimeSeconds);
            Assert.Equal(newMinDate, record.MinDate);
            Assert.Equal(newAvgExecutionTime, record.AvgExecutionTime);
            Assert.Equal(newAvgValue, record.AvgValue);
            Assert.Equal(newMedianValue, record.MedianValue);
            Assert.Equal(newMaxValue, record.MaxValue);
            Assert.Equal(newMinValue, record.MinValue);
            //проверка, что свойство ProcessedAt было установлено на текущее время(с допустимой погрешностью в 1 секунду
            Assert.True(record.ProcessedAt >= DateTime.UtcNow.AddSeconds(-1));
        }
    }
}