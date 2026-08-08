using System;

namespace Application.DTOs
{
    public class ValueRecordDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public double ExecutionTime { get; set; } //  время выполнения одной конкретной операции
        public double Value { get; set; }
        public string FileName { get; set; } = string.Empty;
    }
}
