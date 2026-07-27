using System;

namespace Application.DTOs
{
    public class FilterRequestDto
    {
        public string? FileName { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public double? MinAvgValue { get; set; }
        public double? MaxAvgValue { get; set; }
        public double? MinAvgExecutionTime { get; set; }
        public double? MaxAvgExecutionTime { get; set; }
    }
}