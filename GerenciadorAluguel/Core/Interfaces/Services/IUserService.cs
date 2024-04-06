using Core.Models;

namespace Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserTypeCNH>> GetAllCNHTypeAsync(bool? valid = null);
        Task<UserValidateReturn> ValidateNewUserAsync(User user);
        Task AddAsync(User user);
    }
}
