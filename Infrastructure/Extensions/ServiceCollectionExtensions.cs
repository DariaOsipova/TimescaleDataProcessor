using Microsoft.Extensions.DependencyInjection; // позволяет регистрировать и получать зависимости в вашем приложении
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;

namespace Infrastructure.Extensions
{ // Расширения
    public static class ServiceCollectionExtensions
    {
        // IServiceCollection-Возвращает тот же тип для цепочки вызовов, this IServiceCollection services - Ключевое слово для метода расширения
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // .AddScoped-Создает один экземпляр на один HTTP запрос,<IValueRecordRepository>-Тип,который будут запрашивать,ValueRecordRepository-Реализация,которую будут отдавать
            services.AddScoped<IValueRecordRepository, ValueRecordRepository>();
            services.AddScoped<IResultRecordRepository, ResultRecordRepository>();

            return services;
        }
    }
}
