using Xunit;
using Application.Validators;
using Domain.Exceptions;

namespace Tests.UnitTests.Validators
{
    public class CsvRecordValidatorTests
    {
        [Fact] // это тест
        public void Validate_ValidRecord_ShouldNotThrow()
        {
            var date = new DateTime(2024, 1, 1);
            var executionTime = 1.5;
            var value = 10.0;

            var exception = Record.Exception(
                () => CsvRecordValidator.Validate(date, executionTime, value, 1, "test.csv")); // вызываем метод валидации
            Assert.Null(exception); // проверка,что переменная  равна null
        }

        [Fact] // это тест
        public void Validate_DateInFuture_ShouldThrow()
        { // при передаче даты в будущем валидатор выбрасывает исключение
            var date = DateTime.UtcNow.AddDays(1);
            var executionTime = 1.5;
            var value = 10.0;

            Assert.Throws<CsvValidationException>(
                () => CsvRecordValidator.Validate(date, executionTime, value, 1, "test.csv")); // вызываем метод валидации
        }

        [Fact] // это тест
        public void Validate_DateBeforeMinDate_ShouldThrow()
        {
            var date = new DateTime(1999, 12, 31);
            var executionTime = 1.5;
            var value = 10.0;

            Assert.Throws<CsvValidationException>(
                () => CsvRecordValidator.Validate(date, executionTime, value, 1, "test.csv"));
        }

        [Fact] // это тест
        public void Validate_ExecutionTimeNegative_ShouldThrow()
        {
            var date = new DateTime(2024, 1, 1);
            var executionTime = -1.0;
            var value = 10.0;

            Assert.Throws<CsvValidationException>(
                () => CsvRecordValidator.Validate(date, executionTime, value, 1, "test.csv"));
        }

        [Fact] // это тест
        public void Validate_ValueNegative_ShouldThrow()
        {
            var date = new DateTime(2024, 1, 1);
            var executionTime = 1.5;
            var value = -5.0;

            Assert.Throws<CsvValidationException>(
                () => CsvRecordValidator.Validate(date, executionTime, value, 1, "test.csv"));
        }
    }
}
