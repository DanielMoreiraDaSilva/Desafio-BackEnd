using Core.Models;

namespace Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<TypeCNH>> GetAllCNHTypeAsync(bool? valid = null);
        Task<ValidateReturn> ValidateNewUserAsync(User user);
        Task AddAsync(User user);
        Task UpdaloadCnhImageAsync(Guid idUser, Stream stream, string contentType);
    }
}
