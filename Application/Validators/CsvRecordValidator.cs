using System;
using Domain.Exceptions;

namespace Application.Validators
{
    public static class CsvRecordValidator
    {
        private static readonly DateTime MinDate = new DateTime(2000, 1, 1);

        public static void Validate(DateTime date, double executionTime, double value, int lineNumber,
                                    string fileName)
        {
            if (date > DateTime.UtcNow)
                throw new CsvValidationException(
                    $"Строка {lineNumber}: Дата '{date:yyyy-MM-ddTHH:mm:ss.fffZ}' не может быть позже текущей.");

            if (date < MinDate)
                throw new CsvValidationException(
                    $"Строка {lineNumber}: Дата '{date:yyyy-MM-ddTHH:mm:ss.fffZ}' не может быть раньше 01.01.2000.");

            if (executionTime < 0) // время выполнения одной операции
                throw new CsvValidationException(
                    $"Строка {lineNumber}: Время выполнения '{executionTime}' не может быть меньше 0.");

            if (value < 0)
                throw new CsvValidationException(
                    $"Строка {lineNumber}: Значение '{value}' не может быть меньше 0.");
        }
    }
}
