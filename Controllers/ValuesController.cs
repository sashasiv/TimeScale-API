using infotecs.Data;
using infotecs.Models;
using infotecs.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace infotecs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly IValuesService _valuesService;

        public ValuesController(IValuesService valuesService)
        {
            _valuesService = valuesService;
        }

        // Последние 10 записей по имени
        [HttpGet("last10/byname")]
        public async Task<ActionResult<IEnumerable<object>>> GetLast10ValuesByFileName(string fileName)
        {
            try
            {
                var values = await _valuesService.GetLast10ValuesByFileNameAsync(fileName);

                if (values == null || !values.Any())
                    return NotFound($"Файл '{fileName}' не найден или не содержит данных");

                return Ok(values);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Все записи по имени
        [HttpGet("all/byname")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllValuesByFileName(string fileName)
        {
            try
            {
                var values = await _valuesService.GetAllValuesByFileNameAsync(fileName);

                if (values == null || !values.Any())
                    return NotFound($"Файл '{fileName}' не найден или не содержит данных");

                return Ok(values);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}