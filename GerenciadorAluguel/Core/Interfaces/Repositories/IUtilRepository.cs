using Core.Models;

namespace Core.Interfaces.Repositories
{
    public interface IUtilRepository
    {
        Task<bool> IsFieldValueUniqueAsync<T>(string table, string field, T value);
        Task UpdateFieldAsync<T>(string table, string fieldToUpdate, T valueToUpdate, string fieldToMatch, object valueToMatch, ControlConnection connection = null);
    }
}
