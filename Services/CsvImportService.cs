using Microsoft.EntityFrameworkCore;
using infotecs.Data;
using infotecs.Models;
using infotecs.Models.DTOs;
using infotecs.Validators;

namespace infotecs.Services
{
    public class CsvImportService : ICsvImportService
    {
        private readonly ApplicationDbContext _context;
        private readonly CsvValidator _validator;

        public CsvImportService(ApplicationDbContext context)
        {
            _context = context;
            _validator = new CsvValidator();
        }

        public async Task<ImportResultDto> ImportCsvAsync(string fileName, Stream fileStream, bool overwrite = true)
        {
            var result = new ImportResultDto
            {
                FileName = fileName,
                IsSuccess = false,
                Errors = new List<string>()
            };

            try
            {
                // Валидация
                fileStream.Position = 0;
                var validationResult = await _validator.ValidateFileAsync(fileStream);

                if (!validationResult.IsValid)
                {
                    result.Errors.AddRange(validationResult.Errors);
                    return result;
                }

                // Проверка существования файла
                var existingFile = await _context.Files
                    .FirstOrDefaultAsync(f => f.FileName == fileName);

                // Файл существует и перезапись НЕ подтверждена
                if (existingFile != null && !overwrite)
                {
                    result.Errors.Add($"Файл '{fileName}' уже существует. Для перезаписи установите overwrite=true");
                    return result;
                }

                // Удаление старого файла
                if (existingFile != null && overwrite)
                {
                    // Удаляем файл (Values и Results удалятся каскадно)
                    _context.Files.Remove(existingFile);
                    await _context.SaveChangesAsync();
                }

                // Сохранение файла
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Сохраняем файл
                    var file = new Files
                    {
                        FileName = fileName,
                        RecordsCount = validationResult.ValidRecords.Count
                    };

                    _context.Files.Add(file);
                    await _context.SaveChangesAsync();

                    // Сохраняем значения
                    var values = validationResult.ValidRecords.Select(r => new Values
                    {
                        FileId = file.Id,
                        Date = r.Date,
                        ExecutionTime = r.ExecutionTime,
                        Value = r.Value
                    });

                    await _context.Values.AddRangeAsync(values);
                    await _context.SaveChangesAsync();

                    // Рассчитываем статистику
                    var statsResult = await CalculateStatisticsAsync(file.Id);

                    await transaction.CommitAsync();

                    result.IsSuccess = true;
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                result.Errors.Add($"Ошибка БД: {ex.InnerException?.Message ?? ex.Message}");
                return result;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Внутренняя ошибка: {ex.Message}");
                return result;
            }
        }

        private async Task<TResults> CalculateStatisticsAsync(int fileId)
        {
            // Получаем все записи для этого файла
            var values = await _context.Values
                .Where(v => v.FileId == fileId)
                .OrderBy(v => v.Date)
                .ToListAsync();

            var startDate = values.Min(v => v.Date);
            var endDate = values.Max(v => v.Date);
            var deltaSeconds = (endDate - startDate).TotalSeconds;

            var executionTimes = values.Select(v => v.ExecutionTime).ToList();
            var valueList = values.Select(v => v.Value).ToList();

            var result = new TResults
            {
                FileId = fileId,
                StartDate = startDate,
                DeltaDate_seconds = deltaSeconds,
                AverageExecutionTime = executionTimes.Average(),
                AverageValue = valueList.Average(),
                MedianValue = CalculateMedian(valueList),
                MaxValue = valueList.Max(),
                MinValue = valueList.Min()
            };

            // Сохраняем результат
            _context.TResults.Add(result);
            await _context.SaveChangesAsync();

            return result;
        }

        // Вычисление медианы
        private double CalculateMedian(List<double> values)
        {
            var sorted = values.OrderBy(x => x).ToList();
            int count = sorted.Count;

            if (count == 0) return 0;

            if (count % 2 == 1)
                return sorted[count / 2];
            else
                return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
        }
    }
}
