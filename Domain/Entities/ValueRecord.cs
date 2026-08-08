using System;

namespace Domain.Entities
{
    public class ValueRecord
    { // запись результата
        public Guid Id { get; private set; }
        public DateTime Date { get; private set; }
        public double ExecutionTime { get; private set; }
        public double Value { get; private set; } // анализируемое значение
        public string FileName { get; private set; }

        public ValueRecord(DateTime date, double executionTime, double value, string fileName)
        {
            Id = Guid.NewGuid();
            Date = date;
            ExecutionTime = executionTime;
            Value = value;
            FileName = fileName;
        }

        protected ValueRecord() { } // Entity Framework требует пустой конструктор для восстановления объектов из БД, метод используется внутри класса и наслденики
    }
}