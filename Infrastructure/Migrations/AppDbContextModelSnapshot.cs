using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable// отключаем проверку на null

namespace Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))] // AppDbContext - класс
    partial class AppDbContextModelSnapshot : ModelSnapshot
    { // моментальное состояние бд
        protected override void BuildModel(ModelBuilder modelBuilder)
        { // protected-Метод доступен только внутри этого класса и его наследников,override-Метод переопределяет метод из родительского класса
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0")
          .HasAnnotation("Relational:MaxIdentifierLength", 63);
            //Extensions-статический класс добавляет новые методы к существующим типам без изменения их исходного кода
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);
            // ResultRecord-доменная сущность, которая хранит результаты анализа одного файла
            //b-параметр-строитель (builder) типа EntityTypeBuilder используется для настройки сущности в Entity Framework Core
            modelBuilder.Entity("Domain.Entities.ResultRecord", b =>
            {
                b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid");

                b.Property<double>("AvgExecutionTime") // время операции
                    .HasPrecision(18, 6) // точность числа
                    .HasColumnType("double precision"); // обозначение типа double в PostgreSQL

                b.Property<double>("AvgValue").HasPrecision(18, 6).HasColumnType("double precision");

                b.Property<double>("DeltaTimeSeconds")
                    .HasPrecision(18, 6)
                    .HasColumnType("double precision");

                b.Property<string>("FileName")
                    .IsRequired() // обязательно для заполнения
                    .HasMaxLength(255)
                    .HasColumnType("character varying(255)");

                b.Property<double>("MaxValue").HasPrecision(18, 6).HasColumnType("double precision");

                b.Property<double>("MedianValue").HasPrecision(18, 6).HasColumnType("double precision");

                b.Property<DateTime>("MinDate").HasColumnType("timestamp with time zone");

                b.Property<double>("MinValue").HasPrecision(18, 6).HasColumnType("double precision");

                b.Property<DateTime>("ProcessedAt").HasColumnType("timestamp with time zone");

                b.HasKey("Id");

                b.HasIndex("AvgExecutionTime");

                b.HasIndex("AvgValue");

                b.HasIndex("FileName").IsUnique();

                b.HasIndex("MinDate");

                b.ToTable("ResultRecords");
            });

            modelBuilder.Entity("Domain.Entities.ValueRecord", b =>
            {
                b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid");

                b.Property<DateTime>("Date").HasColumnType("timestamp with time zone");

                b.Property<double>("ExecutionTime").HasPrecision(18, 6).HasColumnType("double precision");

                b.Property<string>("FileName")
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnType("character varying(255)");

                b.Property<double>("Value").HasPrecision(18, 6).HasColumnType("double precision");

                b.HasKey("Id");

                b.HasIndex("Date");

                b.HasIndex("FileName");

                b.ToTable("ValueRecords"); // таблица в бд хранит каждое отдельное значение из обработанного файла
            });
#pragma warning restore 612, 618
        }
    }
}
