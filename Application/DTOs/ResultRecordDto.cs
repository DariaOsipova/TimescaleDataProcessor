using System;

namespace Application.DTOs
{
    public class ResultRecordDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty; // инициализация свойства пустой строкой при создании объекта
        public double DeltaTimeSeconds { get; set; }
        public DateTime MinDate { get; set; }
        public double AvgExecutionTime { get; set; }
        public double AvgValue { get; set; }
        public double MedianValue { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}