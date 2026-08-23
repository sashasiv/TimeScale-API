using Microsoft.EntityFrameworkCore;
using infotecs.Data;

namespace infotecs.Services
{
    public class ValuesService : IValuesService
    {
        private readonly ApplicationDbContext _context;

        public ValuesService(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<int?> GetFileIdByNameAsync(string fileName)
        {
            var file = await _context.Files
                .Where(f => f.FileName == fileName)
                .Select(f => new { f.Id })
                .FirstOrDefaultAsync();

            return file?.Id;
        }

        // получить последние 10 значений по имени файла
        public async Task<IEnumerable<object>> GetLast10ValuesByFileNameAsync(string fileName)
        {
            // Находим ID файла по имени
            var fileId = await GetFileIdByNameAsync(fileName);

            if (!fileId.HasValue)
                throw new FileNotFoundException($"Файл '{fileName}' не найден");

            // Ищем данные по ID
            return await GetLast10ValuesByFileIdAsync(fileId.Value);
        }

        // Получить последние 10 значений по id (используется в предыдущем)
        private async Task<IEnumerable<object>> GetLast10ValuesByFileIdAsync(int fileId)
        {
            return await _context.Values
                .Where(v => v.FileId == fileId)
                .OrderByDescending(v => v.Date)
                .Take(10)
                .OrderBy(v => v.Date)
                .Select(v => new
                {
                    v.Date,
                    v.ExecutionTime,
                    v.Value
                })
                .ToListAsync();
        }

        // Аналогично но для всех записей
        // получить все значения по имени файла
        public async Task<IEnumerable<object>> GetAllValuesByFileNameAsync(string fileName)
        {
            // Находим ID файла по имени
            var fileId = await GetFileIdByNameAsync(fileName);

            if (!fileId.HasValue)
                throw new FileNotFoundException($"Файл '{fileName}' не найден");

            // Ищем данные по ID
            return await GetAllValuesByFileIdAsync(fileId.Value);
        }

       // Получить все значения по id (используется в предыдущем)
        private async Task<IEnumerable<object>> GetAllValuesByFileIdAsync(int fileId)
        {
            return await _context.Values
                .Where(v => v.FileId == fileId)
                .OrderBy(v => v.Date)
                .Select(v => new
                {
                    v.Date,
                    v.ExecutionTime,
                    v.Value
                })
                .ToListAsync();
        }
    }
}
