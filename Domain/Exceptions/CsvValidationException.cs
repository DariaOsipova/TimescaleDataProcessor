using System;

namespace Domain.Exceptions
{
    public class CsvValidationException : Exception
    { // наследуемс€ от Exception
        public CsvValidationException(string message) : base(message) { } // : base-ѕередает в родительский класс Exception
    }
}