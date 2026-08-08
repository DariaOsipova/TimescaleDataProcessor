using System;

namespace Domain.Entities
{
    public class ResultRecord
    { // запись результата
        public Guid Id { get; private set; }
        public string FileName { get; private set; }
        public double DeltaTimeSeconds { get; private set; }
        public DateTime MinDate { get; private set; }
        public double AvgExecutionTime { get; private set; }
        public double AvgValue { get; private set; }
        public double MedianValue { get; private set; } // Значение в середине отсортированного списка
        public double MaxValue { get; private set; }
        public double MinValue { get; private set; }
        public DateTime ProcessedAt { get; private set; } // когда создан/обработан объект

        public ResultRecord(string fileName, double deltaTimeSeconds, DateTime minDate,
                            double avgExecutionTime, double avgValue, double medianValue,
                            double maxValue, double minValue)
        { // констуктор-метод создания объекта
            Id = Guid.NewGuid();
            FileName = fileName;
            DeltaTimeSeconds = deltaTimeSeconds;
            MinDate = minDate;
            AvgExecutionTime = avgExecutionTime;
            AvgValue = avgValue;
            MedianValue = medianValue;
            MaxValue = maxValue;
            MinValue = minValue;
            ProcessedAt = DateTime.UtcNow;
        }

        protected ResultRecord() { } // Entity Framework требует пустой конструктор для восстановления объектов из БД, метод используется внутри класса и наслденики

        public void Update(string fileName, double deltaTimeSeconds, DateTime minDate,
                       double avgExecutionTime, double avgValue, double medianValue,
                       double maxValue, double minValue)
        {
            FileName = fileName;
            DeltaTimeSeconds = deltaTimeSeconds;
            MinDate = minDate;
            AvgExecutionTime = avgExecutionTime;
            AvgValue = avgValue;
            MedianValue = medianValue;
            MaxValue = maxValue;
            MinValue = minValue;
            ProcessedAt = DateTime.UtcNow;
        }
    }
}