namespace infotecs.Services
{
    public interface IValuesService
    {
        // Поиск по имени файла (использует поиск по id внутри)
        Task<IEnumerable<object>> GetLast10ValuesByFileNameAsync(string fileName);
        Task<IEnumerable<object>> GetAllValuesByFileNameAsync(string fileName);
    }
}
