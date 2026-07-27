using System.Collections.Generic; // дл€ IEnumerable<ResultRecordDto>
using System.Linq; // дл€ работы с запросами
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper; // установление соответстви€ между элементами разных структур данных
using Domain.Interfaces;

namespace Application.Services
{
    public class ResultRecordService : IResultRecordService
    { // запись, объ€вленный класс реализует интерфейс : IResultRecordService
        private readonly IResultRecordRepository _repository; // IResultRecordRepository=интерфейс репозитори€
        private readonly IMapper _mapper;

        // конструктор класса,присваивает значени€ пол€м при создании объекта
        public ResultRecordService(IResultRecordRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        // IEnumerable-интерфейс-коллекци€ объектов,которую можно перебирать
        public async Task<IEnumerable<ResultRecordDto>> FilterAsync(FilterRequestDto filter)
        { // DTO дл€ передачи данных клиенту
            var results = await _repository.FilterAsync( // паралелльно фильтрием
          filter.FileName, filter.MinDate, filter.MaxDate, filter.MinAvgValue, filter.MaxAvgValue,
          filter.MinAvgExecutionTime, filter.MaxAvgExecutionTime);

            return _mapper.Map<IEnumerable<ResultRecordDto>>(results); // метод map у объекта _mapper передает аргумент results
        }
    }
}