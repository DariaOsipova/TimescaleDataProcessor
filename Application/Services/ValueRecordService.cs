using System;
using System.Collections.Generic; // обобщенное программирование
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.Validators;
using Application.Helpers;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class ValueRecordService : IValueRecordService
    {
        private readonly IValueRecordRepository _repository;
        private readonly IResultRecordRepository _resultRepository;
        private readonly IMapper _mapper;

        public ValueRecordService(IValueRecordRepository repository,
                                  IResultRecordRepository resultRepository, IMapper mapper)
        {
            _repository = repository;
            _resultRepository = resultRepository;
            _mapper = mapper;
        }
        // обрабатываем файл,возвращаем результат загрузки
        public async Task<UploadResultDto> ProcessCsvAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new CsvValidationException("Файл пуст или не выбран.");

            var fileName = file.FileName;
            var records = new List<ValueRecord>(); // создаем список для хранения объектов

            using var stream = file.OpenReadStream(); // октрываем поток для чтения файла
            using var reader = new StreamReader(stream);

            string? line;
            int lineNumber = 0;
            // Читает одну строку из файла не блокируя поток
            while ((line = await reader.ReadLineAsync()) != null)
            {
                lineNumber++;

                // Пропускаем заголовок
                if (lineNumber == 1 && line.StartsWith("Date;ExecutionTime;Value")) // время выполнения одной записи 
                    continue;

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(';');
                if (parts.Length != 3)
                    throw new CsvValidationException(
                        $"Строка {lineNumber}: неверный формат. Ожидается 3 поля.");

                try
                {
                    // Преврати строку в дату,но только если она соответствует точно заданному формату
                    // CultureInfo.InvariantCulture-Использовать независимый от языка формат
                    // Coordinated Universal Time
                    var date =
              DateTime.ParseExact(parts[0], "yyyy-MM-ddTHH:mm:ss.fffZ",
                                  CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

                    var executionTime = double.Parse(parts[1], CultureInfo.InvariantCulture);
                    var value = double.Parse(parts[2], CultureInfo.InvariantCulture);
                    // Класс находится в Application.Validators,содержит метод Validate,одной конкретной записи из CSV файла
                    CsvRecordValidator.Validate(date, executionTime, value, lineNumber, fileName);
                    // в список records новый объект типа ValueRecord,DateTime.SpecifyKind(date, DateTimeKind.Utc)-Устанавливает тип даты как UTC
                    // executionTime-время выполнения
                    records.Add(new ValueRecord(DateTime.SpecifyKind(date, DateTimeKind.Utc), executionTime,
                                      value, fileName));
                }
                catch (FormatException ex)
                {
                    throw new CsvValidationException(
                        $"Строка {lineNumber}: неверный формат данных. {ex.Message}");
                }
            }

            if (!records.Any())
                throw new CsvValidationException("Файл не содержит данных.");

            if (records.Count < 1 || records.Count > 10000)
                throw new CsvValidationException(
                    $"Количество строк ({records.Count}) должно быть от 1 до 10000.");

            var isNewFile = true;
            var existingResult = await _resultRepository.GetByFileNameAsync(fileName);

            if (existingResult != null)
            { // Если existingResult не равен null значит файл уже был обработан
                isNewFile = false;
                await _repository.DeleteByFileNameAsync(fileName);
            }

            await _repository.AddRangeAsync(records); // добавление множества записей в базу данных за одну операцию

            var result = StatisticsHelper.CalculateStatistics(records, fileName);

            if (existingResult != null)
            {
                existingResult.Update(result.FileName, result.DeltaTimeSeconds, result.MinDate,
                                      result.AvgExecutionTime, result.AvgValue, result.MedianValue,
                                      result.MaxValue, result.MinValue);
                await _resultRepository.UpdateAsync(existingResult);
            }
            else
            {
                await _resultRepository.AddAsync(result);
            }

            return new UploadResultDto
            {
                FileName = fileName,
                RecordsCount = records.Count,
                IsNewFile = isNewFile
            }; // один объект с результатом
        }

        public async Task<IEnumerable<ValueRecordDto>> GetLast10ValuesByFileNameAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new CsvValidationException("Имя файла не может быть пустым.");

            var records = await _repository.GetLast10ByFileNameAsync(fileName); // последние записи для указанного файла
                                                                                // _mapper-Объект AutoMapper(для преобразования),IEnumerable-коллекция.которую можно перебирать.ValueRecordDto-тип элементов в коллекции
            return _mapper.Map<IEnumerable<ValueRecordDto>>(records);
        }
    }
}