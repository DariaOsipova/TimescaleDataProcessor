using Xunit;
using Domain.Exceptions;
using System;

namespace Tests.UnitTests.Domain.Exceptions
{
    public class FileProcessingExceptionTests
    {
        [Fact] // это тест
        public void Constructor_ShouldSetMessage()
        {
            var message = "File processing failed";

            var exception = new FileProcessingException(message);

            // Assert-проверка результата
            Assert.Equal(message, exception.Message); // Equal - что значения одинаковы
            Assert.Null(exception.InnerException); // внутреннее исключение,которое было причиной текущего исключения
        }

        [Fact]
        public void Constructor_WithInnerException_ShouldSetBoth()
        { // тест проверяет,что конструктор CsvValidationException правильно сохраняет и сообщение, внутреннее исключение
          // Arrange
            var message = "File processing failed";
            var inner = new Exception("Inner exception");

            var exception = new FileProcessingException(message, inner);

            Assert.Equal(message, exception.Message);
            Assert.Equal(inner, exception.InnerException);
        }

        [Fact]
        public void Constructor_WithNullInnerException_ShouldSetMessageOnly()
        { // внутреннее исключение пустое
            var message = "File processing failed";

            var exception = new FileProcessingException(message, null);

            Assert.Equal(message, exception.Message);
            Assert.Null(exception.InnerException);
        }
    }
}