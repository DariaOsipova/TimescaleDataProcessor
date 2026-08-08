using System;
using System.Collections.Generic; // Коллекции (списки, словари)
using System.Linq; // Методы для работы с коллекциями
using Domain.Entities;

namespace Application.Helpers
{
    public static class StatisticsHelper
    {
        // IEnumerable-Интерфейс для любой коллекции(список, массив, запрос),<ValueRecord>-Тип элементов в коллекции
        public static ResultRecord CalculateStatistics(IEnumerable<ValueRecord> records,
                                                   string fileName)
        { // ResultRecord-доменная сущность хранит статистику и метаданные об обработанном файле
            var list = records.ToList();
            if (!list.Any())
                throw new InvalidOperationException("Нет данных для агрегации.");

            var values = list.Select(r => r.Value).ToList(); // Для каждого элемента берем свойство Value,Превращаем в список
            var executionTimes = list.Select(r => r.ExecutionTime).ToList(); // выполнение 1-ой операции
            var dates = list.Select(r => r.Date).ToList();

            var deltaTime = (dates.Max() - dates.Min()).TotalSeconds;
            var minDate = dates.Min();
            var avgExecutionTime = executionTimes.Average();
            var avgValue = values.Average();
            var medianValue = CalculateMedian(values);
            var maxValue = values.Max();
            var minValue = values.Min();

            return new ResultRecord(fileName, deltaTime, minDate, avgExecutionTime, avgValue, medianValue,
                                    maxValue, minValue);
        }

        private static double CalculateMedian(List<double> values)
        {
            var sorted = values.OrderBy(v => v).ToList(); // по возрастанию
            int count = sorted.Count;
            if (count % 2 == 1)
                return sorted[count / 2];
            else
                return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
        }
    }
}
