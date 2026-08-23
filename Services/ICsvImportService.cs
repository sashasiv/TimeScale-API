using infotecs.Models.DTOs;

namespace infotecs.Services
{
    public interface ICsvImportService
    {
        // Запись файла
        Task<ImportResultDto> ImportCsvAsync(string fileName, Stream fileStream, bool overwrite = false);
    }
}
