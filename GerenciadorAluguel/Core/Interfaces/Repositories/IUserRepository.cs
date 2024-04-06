using Core.Models;

namespace Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserTypeCNH>> GetAllCNHTypeAsync(bool? valid = null);
        Task AddAsync(User user);
    }
}
