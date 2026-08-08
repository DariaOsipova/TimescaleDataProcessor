using AutoMapper; // преобразование объектов одного типа в другой
using Application.DTOs;
using Domain.Entities;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    { // наследуется
        public MappingProfile()
        {
            CreateMap<ValueRecord, ValueRecordDto>();
            CreateMap<ResultRecord, ResultRecordDto>();
        }
    }
}
