namespace infotecs.Services
{
    public interface IResultsOutputService
    {
        // Получить результат по имени файла
        Task<IEnumerable<object>> GetResultByFileNameAsync(
            string fileName);
        // Получить результаты в диапооне начальных дат
        Task<IEnumerable<object>> GetResultsSortedByStartDateAsync(
            DateTimeOffset? minDate = null,
            DateTimeOffset? maxDate = null);
        // Получить результаты в по среднему времени выполнения
        Task<IEnumerable<object>> GetResultsSortedByAverageExecutionTimeAsync(
            double? minExecutionTime = null,
            double? maxExecutionTime = null);
        // Получить результаты в по среднему значению
        Task<IEnumerable<object>> GetResultsSortedByAverageValueAsync(
            double? minValue = null,
            double? maxValue = null);
    }
}
