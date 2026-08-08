using Microsoft.EntityFrameworkCore; // бд
using Domain.Entities;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ValueRecord> ValueRecords { get; set; } // Каждое DbSet<T>-таблица в БД
        public DbSet<ResultRecord> ResultRecords { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        // Передает настройки в базовый класс EF Core
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ValueRecord>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Date).IsRequired(); // IsRequired()=поле не может быть NULL в БД
                entity.Property(v => v.ExecutionTime).IsRequired().HasPrecision(18, 6); // ремя выполнения операции
                entity.Property(v => v.Value).IsRequired().HasPrecision(18, 6);
                entity.Property(v => v.FileName).IsRequired().HasMaxLength(255);
                entity.HasIndex(v => v.FileName);
                entity.HasIndex(v => v.Date);
            });

            modelBuilder.Entity<ResultRecord>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.FileName).IsRequired().HasMaxLength(255);
                entity.Property(r => r.DeltaTimeSeconds).IsRequired().HasPrecision(18, 6); //  разница во времени между двумя моментами
                entity.Property(r => r.MinDate).IsRequired();
                entity.Property(r => r.AvgExecutionTime).IsRequired().HasPrecision(18, 6);
                entity.Property(r => r.AvgValue).IsRequired().HasPrecision(18, 6);
                // медианное значение набора данных (статистическая мера центральной тенденции)
                entity.Property(r => r.MedianValue).IsRequired().HasPrecision(18, 6);
                entity.Property(r => r.MaxValue).IsRequired().HasPrecision(18, 6);
                entity.Property(r => r.MinValue).IsRequired().HasPrecision(18, 6);
                // временная метка, которая показывает когда запись была создана/обработана
                entity.Property(r => r.ProcessedAt).IsRequired();
                entity.HasIndex(r => r.FileName).IsUnique();
                entity.HasIndex(r => r.MinDate);
                entity.HasIndex(r => r.AvgValue);
                entity.HasIndex(r => r.AvgExecutionTime);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
