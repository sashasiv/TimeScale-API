// Controllers/FilesController.cs
using Microsoft.AspNetCore.Mvc;
using infotecs.Services;
using infotecs.Models.DTOs;

namespace infotecs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ICsvImportService _csvImportService;
        private readonly IWebHostEnvironment _env;

        public FilesController(ICsvImportService csvImportService, IWebHostEnvironment env)
        {
            _csvImportService = csvImportService;
            _env = env;
        }

        // ИМПОРТ CSV
        [HttpPost("import")]
        public async Task<ActionResult<ImportResultDto>> ImportCsv(
    IFormFile file,
    [FromQuery] bool overwrite = true)  // ← ПАРАМЕТР ИЗ URL
        {
            // Проверка: файл выбран
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ImportResultDto
                {
                    FileName = file?.FileName ?? "неизвестный",
                    IsSuccess = false,
                    Errors = new List<string> { "Файл не выбран или пуст" }
                });
            }

            // Проверка: расширение .csv
            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new ImportResultDto
                {
                    FileName = file.FileName,
                    IsSuccess = false,
                    Errors = new List<string> { "Файл должен иметь расширение .csv" }
                });
            }

            try
            {
                using var stream = file.OpenReadStream();
                var result = await _csvImportService.ImportCsvAsync(file.FileName, stream, overwrite);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ImportResultDto
                {
                    FileName = file.FileName,
                    IsSuccess = false,
                    Errors = new List<string> { $"Внутренняя ошибка: {ex.Message}" }
                });
            }
        }
    }
}
