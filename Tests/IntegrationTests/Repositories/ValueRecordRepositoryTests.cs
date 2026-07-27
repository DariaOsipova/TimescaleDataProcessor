using Xunit;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.IntegrationTests.Repositories
{
    public class ValueRecordRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ValueRecordRepository _repository;

        public ValueRecordRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new ValueRecordRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task AddRangeAsync_ShouldAddMultipleRecords()
        {
            // Arrange
            var fileName = "test.csv";
            var records = new List<ValueRecord>
            {
                new ValueRecord(DateTime.UtcNow, 5.5, 100.5, fileName),
                new ValueRecord(DateTime.UtcNow.AddSeconds(10), 3.2, 200.3, fileName)
            };

            // Act
            await _repository.AddRangeAsync(records);

            // Assert
            var saved = await _context.ValueRecords
                .Where(v => v.FileName == fileName)
                .ToListAsync();
            Assert.Equal(2, saved.Count);
        }

        [Fact]
        public async Task GetLast10ByFileNameAsync_ShouldReturnLast10Records()
        {
            // Arrange
            var fileName = "test.csv";
            var records = new List<ValueRecord>();
            for (int i = 1; i <= 15; i++)
            {
                records.Add(new ValueRecord(
                    DateTime.UtcNow.AddSeconds(i),
                    5.5 + i,
                    100.5 + i,
                    fileName
                ));
            }
            await _context.ValueRecords.AddRangeAsync(records);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetLast10ByFileNameAsync(fileName);

            // Assert
            Assert.Equal(10, result.Count());
            var sorted = result.OrderByDescending(r => r.Date).ToList();
            Assert.Equal(records[14].Date, sorted[0].Date);
        }

        [Fact]
        public async Task DeleteByFileNameAsync_ShouldDeleteAllRecords_WhenFileExists()
        {
            // Arrange
            var fileName = "test.csv";
            var records = new List<ValueRecord>
            {
                new ValueRecord(DateTime.UtcNow, 5.5, 100.5, fileName),
                new ValueRecord(DateTime.UtcNow.AddSeconds(10), 3.2, 200.3, fileName)
            };
            await _context.ValueRecords.AddRangeAsync(records);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteByFileNameAsync(fileName);

            // Assert
            var deleted = await _context.ValueRecords
                .Where(v => v.FileName == fileName)
                .ToListAsync();
            Assert.Empty(deleted);
        }
    }
}