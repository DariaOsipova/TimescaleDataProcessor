using Xunit;
using Domain.Entities;
using System;

namespace Tests.UnitTests.Domain.Entities
{
    public class ValueRecordTests
    {
        [Fact] // это тест
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            var date = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var executionTime = 1.5;
            var value = 10.0;
            var fileName = "test.csv";

            // создание объекта
            var record = new ValueRecord(date, executionTime, value, fileName);

            // Assert-проверка результата
            Assert.NotEqual(Guid.Empty, record.Id);
            Assert.Equal(date, record.Date); // проверяем что значения одинаковы
            Assert.Equal(executionTime, record.ExecutionTime);
            Assert.Equal(value, record.Value);
            Assert.Equal(fileName, record.FileName);
        }

        [Fact]
        // тест проверяет, что конструктор ValueRecord правильно устанавливает свойства с разными (крайними) значениями
        public void Constructor_WithDifferentValues_ShouldSetProperties()
        {
            // Arrange
            var date = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc);
            var executionTime = 99.9;
            var value = 999.99;
            var fileName = "large_file.csv";

            // Act
            var record = new ValueRecord(date, executionTime, value, fileName);

            // Assert
            Assert.Equal(date, record.Date);
            Assert.Equal(executionTime, record.ExecutionTime);
            Assert.Equal(value, record.Value);
            Assert.Equal(fileName, record.FileName);
        }

        [Fact]
        public void Constructor_ShouldGenerateUniqueIds()
        {
            // Arrange
            var date = DateTime.UtcNow;

            // Act
            var record1 = new ValueRecord(date, 1.0, 10.0, "file1.csv");
            var record2 = new ValueRecord(date, 2.0, 20.0, "file2.csv");

            // Assert
            Assert.NotEqual(record1.Id, record2.Id);
        }
    }
}