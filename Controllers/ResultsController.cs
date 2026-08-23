using infotecs.Data;
using infotecs.Models;
using infotecs.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace infotecs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly IResultsOutputService _resultsService;

        public ResultsController(IResultsOutputService resultsService)
        {
            _resultsService = resultsService;
        }
    
        [HttpGet("sirch/byfilename")]
        public async Task<ActionResult<IEnumerable<object>>> GetResultByFileName
            (string fileName)
        {
            try
            {
                var result = await _resultsService.GetResultByFileNameAsync(fileName);

                if (result == null)
                    return NotFound($"Результаты для файла '{fileName}' не найдены");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("sirch/bystartdate")]
        public async Task<ActionResult<IEnumerable<object>>> GetResultsSortedByStartDate(
            [FromQuery] DateTimeOffset? fromDate = null,
            [FromQuery] DateTimeOffset? toDate = null)
        {
            try
            {
                var results = await _resultsService.GetResultsSortedByStartDateAsync(fromDate, toDate);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("sirch/byaverageexecutiontime")]
        public async Task<ActionResult<IEnumerable<object>>> GetResultsSortedByAverageExecutionTime(
            [FromQuery] double? minExecutionTime = null,
            [FromQuery] double? maxExecutionTime = null)
        {
            try
            {
                var results = await _resultsService.GetResultsSortedByAverageExecutionTimeAsync(
                    minExecutionTime, maxExecutionTime);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("sirch/byaveragevalue")]
        public async Task<ActionResult<IEnumerable<object>>> GetResultsSortedByAverageValue(
            [FromQuery] double? minValue = null,
            [FromQuery] double? maxValue = null)
        {
            try
            {
                var results = await _resultsService.GetResultsSortedByAverageValueAsync(
                    minValue, maxValue);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}

