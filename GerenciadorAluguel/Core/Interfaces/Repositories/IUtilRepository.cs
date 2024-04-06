namespace Core.Interfaces.Repositories
{
    public interface IUtilRepository
    {
        Task<bool> IsFieldValueUniqueAsync<T>(string table, string field, T value);
    }
}
