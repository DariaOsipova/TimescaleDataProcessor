using Xunit;
using Application.Helpers;
using Domain.Entities;
using System;
using System.Collections.Generic; // использование коллекций(List)

namespace Tests.UnitTests.Helpers
{
    public class StatisticsHelperTests
    {
        [Fact] // это тест
        public void CalculateStatistics_ValidRecords_ShouldReturnCorrectResult()
        {
            var records = new List<ValueRecord> {
        new ValueRecord(new DateTime(2024, 1, 1, 10, 0, 0), 1.5, 10.0, "test.csv"),
        new ValueRecord(new DateTime(2024, 1, 1, 10, 1, 0), 2.0, 20.0, "test.csv"),
        new ValueRecord(new DateTime(2024, 1, 1, 10, 2, 0), 2.5, 30.0, "test.csv")
      };

            var result = StatisticsHelper.CalculateStatistics(records, "test.csv");

            // Assert-проверка результата, // Equal - что значения одинаковы
            Assert.Equal("test.csv", result.FileName); // "test.csv"-ожидаемое значение, result.FileName-фактическое
            Assert.Equal(120, result.DeltaTimeSeconds);  // 2 минуты = 120 секунд
            Assert.Equal(new DateTime(2024, 1, 1, 10, 0, 0), result.MinDate);
            Assert.Equal(2.0, result.AvgExecutionTime);
            Assert.Equal(20.0, result.AvgValue);
            Assert.Equal(20.0, result.MedianValue);
            Assert.Equal(30.0, result.MaxValue);
            Assert.Equal(10.0, result.MinValue);
        }

        [Fact]
        public void CalculateStatistics_EvenNumberOfRecords_ShouldCalculateMedianCorrectly()
        {
            // тест проверяет, что медиана правильно вычисляется для четного количества записей
            var records = new List<ValueRecord> {
        new ValueRecord(new DateTime(2024, 1, 1, 10, 0, 0), 1.0, 10.0, "test.csv"),
        new ValueRecord(new DateTime(2024, 1, 1, 10, 1, 0), 2.0, 30.0, "test.csv"),
        new ValueRecord(new DateTime(2024, 1, 1, 10, 2, 0), 3.0, 20.0, "test.csv"),
        new ValueRecord(new DateTime(2024, 1, 1, 10, 3, 0), 4.0, 40.0, "test.csv")
      };

            var result = StatisticsHelper.CalculateStatistics(records, "test.csv");

            Assert.Equal(25.0, result.MedianValue);  // (20 + 30) / 2 = 25
        }

        [Fact]
        public void CalculateStatistics_EmptyRecords_ShouldThrowException()
        {
            var records = new List<ValueRecord>();

            Assert.Throws<InvalidOperationException>(
                () => StatisticsHelper.CalculateStatistics(records, "test.csv"));
        }

        [Fact]
        public void CalculateStatistics_SingleRecord_ShouldReturnCorrectValues()
        {
            // корректно работает с одной записью?
            var records = new List<ValueRecord> { new ValueRecord(new DateTime(2024, 1, 1, 10, 0, 0), 1.5,
                                                            10.0, "test.csv") };

            var result = StatisticsHelper.CalculateStatistics(records, "test.csv");

            // Assert-проверка результата
            Assert.Equal(0, result.DeltaTimeSeconds);
            Assert.Equal(1.5, result.AvgExecutionTime);
            Assert.Equal(10.0, result.AvgValue);
            Assert.Equal(10.0, result.MedianValue);
            Assert.Equal(10.0, result.MaxValue);
            Assert.Equal(10.0, result.MinValue);
        }
    }
}
