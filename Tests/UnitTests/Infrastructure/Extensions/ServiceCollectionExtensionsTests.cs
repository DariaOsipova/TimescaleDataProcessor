using Xunit;
using Microsoft.Extensions.DependencyInjection; //  Dependency Injection (DI)-внедрение зависимостей
using Domain.Interfaces;
using Infrastructure.Extensions;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;

namespace Tests.UnitTests.Infrastructure.Extensions
{
    public class ServiceCollectionExtensionsTests // коллекция расиширений
    {
        [Fact] // это тест
        public void AddInfrastructure_ShouldRegisterRepositories() // должен регистрировать репозитории
        {
            var services = new ServiceCollection();

            // Регистрируем DbContext с InMemory
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            services.AddInfrastructure(); // Вызывает наш метод расширения, который регистрирует все репозитории

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope(); // serviceProvider.CreateScope()-Создает новый Scope(область) для Scoped сервисов,
                                                             // using var scope-Автоматически освободит ресурсы после теста

            var valueRepo = scope.ServiceProvider.GetService<IValueRecordRepository>(); // Запрашивает экземпляр репозитория из DI контейнера и сохраняет его в переменную
            var resultRepo = scope.ServiceProvider.GetService<IResultRecordRepository>();
            // DEPENDENCY INJECTION-паттерн проектирования, при котором объекты не создают свои зависимости сами, а получают их извне
            Assert.NotNull(valueRepo); // проверяем,что объект не равен null
            Assert.IsType<ValueRecordRepository>(valueRepo); // проверяем,что объект ялвяется экземпляром указанного типа
            Assert.NotNull(resultRepo);
            Assert.IsType<ResultRecordRepository>(resultRepo);
        }

        [Fact] // это тест проверяет конфигурацию
        public void AddInfrastructure_ShouldRegisterRepositories_WithScopedLifetime()
        {
            var services = new ServiceCollection(); // Создает пустой DI контейнер
            // Регистрируем DbContext с InMemory
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            services.AddInfrastructure(); // Вызывает наш метод расширения, который регистрирует все репозитории
            // поиск информации о регистрации сервиса в DI контейнере
            var valueRepoDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IValueRecordRepository));
            var resultRepoDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IResultRecordRepository));

            Assert.NotNull(valueRepoDescriptor); // проверяем,что объект не равен null
            Assert.NotNull(resultRepoDescriptor);
            //  проверка,что жизненный цикл зарегистрированного сервиса равен Scoped(когда объект живет в рамках одной области)
            Assert.Equal(ServiceLifetime.Scoped, valueRepoDescriptor.Lifetime);
            Assert.Equal(ServiceLifetime.Scoped, resultRepoDescriptor.Lifetime);
        }

        [Fact]
        public void AddInfrastructure_ShouldRegisterDbContext() //  тест проверяет, что DbContext зарегистрирован в DI(внедрение зависимостей) контейнере
        {
            var services = new ServiceCollection(); // Создает пустой DI контейнер

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDb")); // Регистрируем DbContext с InMemory

            services.AddInfrastructure(); // Вызывает наш метод расширения, который регистрирует все репозитории

            var dbContextDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(AppDbContext));
            Assert.NotNull(dbContextDescriptor);
        }
    }
}