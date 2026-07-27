using System;

namespace Domain.Exceptions
{
    public class FileProcessingException : Exception
    { // наследуемся от Exception
        public FileProcessingException(string message) : base(message) { } // : base-Передает в родительский класс Exception
        public FileProcessingException(string message, Exception inner) : base(message, inner) { } // inner - ошибка, причина исключения
    }
}