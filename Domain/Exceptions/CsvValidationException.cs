using System;

namespace Domain.Exceptions
{
    public class CsvValidationException : Exception
    { // наследуемся от Exception
        public CsvValidationException(string message) : base(message) { } // : base-Передает в родительский класс Exception
    }
}
