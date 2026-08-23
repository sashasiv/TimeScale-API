using infotecs.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace infotecs.Services
{
    public class ResultsOutputService : IResultsOutputService
    {
        private readonly ApplicationDbContext _context;

        public ResultsOutputService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Сортировка по имени

        public async Task<IEnumerable<object>> GetResultByFileNameAsync(
            string fileName)
        {
            return await _context.TResults
                .Include(r => r.File)
                .Where(r => r.File.FileName == fileName)
                .Select(r => new
                {
                    FileName = r.File.FileName,
                    StartDate = r.StartDate,
                    DeltaDate_seconds = r.DeltaDate_seconds,
                    AverageExecutionTime = r.AverageExecutionTime,
                    AverageValue = r.AverageValue,
                    MedianValue = r.MedianValue,
                    MaxValue = r.MaxValue,
                    MinValue = r.MinValue
                })
                .ToListAsync();
        }

        // Сортировка по дате

        public async Task<IEnumerable<object>> GetResultsSortedByStartDateAsync(
            DateTimeOffset? minDate = null,
            DateTimeOffset? maxDate = null)
        {
            var query = _context.TResults
                .Include(r => r.File)
                .AsQueryable();

            // Фильтрация по диапазону дат
            DateTimeOffset min = minDate ?? DateTimeOffset.MinValue;
            DateTimeOffset max = maxDate ?? DateTimeOffset.MaxValue;

                query = query.Where(r => r.StartDate >= min && r.StartDate <= max);

            return await query
                .OrderBy(r => r.StartDate)
                .Select(r => new
                {
                    FileName = r.File.FileName,
                    StartDate = r.StartDate,
                    DeltaDate_seconds = r.DeltaDate_seconds,
                    AverageExecutionTime = r.AverageExecutionTime,
                    AverageValue = r.AverageValue,
                    MedianValue = r.MedianValue,
                    MaxValue = r.MaxValue,
                    MinValue = r.MinValue
                })
                .ToListAsync();
        }

        // Фильтрация по среднему времени
        public async Task<IEnumerable<object>> GetResultsSortedByAverageExecutionTimeAsync(
            double? minExecutionTime = null,
            double? maxExecutionTime = null)
        {
            double min = minExecutionTime ?? double.MinValue;
            double max = maxExecutionTime ?? double.MaxValue;

            var query = _context.TResults
                .Include(r => r.File)
                .AsQueryable();

            query = query.Where(r => r.AverageExecutionTime >= min && r.AverageExecutionTime <= max);


            return await query
                .OrderBy(r => r.AverageExecutionTime)
                .Select(r => new
                {
                    FileName = r.File.FileName,
                    StartDate = r.StartDate,
                    DeltaDate_seconds = r.DeltaDate_seconds,
                    AverageExecutionTime = r.AverageExecutionTime,
                    AverageValue = r.AverageValue,
                    MedianValue = r.MedianValue,
                    MaxValue = r.MaxValue,
                    MinValue = r.MinValue
                })
                .ToListAsync();
        }

        // Фильтрация по  Value
        public async Task<IEnumerable<object>> GetResultsSortedByAverageValueAsync(
            double? minValue = null,
            double? maxValue = null)
        {
            double min = minValue ?? double.MinValue;
            double max = maxValue ?? double.MaxValue;

            var query = _context.TResults
                .Include(r => r.File)
                .AsQueryable();
            
                query = query.Where(r => r.AverageValue >= min && r.AverageValue <= max);

            return await query
                .OrderBy(r => r.AverageValue)
                .Select(r => new
                {
                    FileName = r.File.FileName,
                    StartDate = r.StartDate,
                    DeltaDate_seconds = r.DeltaDate_seconds,
                    AverageExecutionTime = r.AverageExecutionTime,
                    AverageValue = r.AverageValue,
                    MedianValue = r.MedianValue,
                    MaxValue = r.MaxValue,
                    MinValue = r.MinValue
                })
                .ToListAsync();
        }
    }
}
