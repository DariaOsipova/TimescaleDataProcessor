using Xunit;
using Domain.Exceptions;

namespace Tests.UnitTests.Domain.Exceptions
{
    public class CsvValidationExceptionTests
    {
        [Fact] // это тест
        public void Constructor_ShouldSetMessage()
        {
            var message = "Invalid CSV format";
            var exception = new CsvValidationException(message);
            Assert.Equal(message, exception.Message); // проверяем что значения одинаковы
        }

        [Fact]
        public void Constructor_WithEmptyMessage_ShouldSetEmptyMessage()
        {
            var exception = new CsvValidationException("");
            Assert.Equal("", exception.Message);
        }
    }
}