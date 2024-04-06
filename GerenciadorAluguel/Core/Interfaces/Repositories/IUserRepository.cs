using Core.Models;

namespace Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<TypeCNH>> GetAllCNHTypeAsync(bool? valid = null);
        Task AddAsync(User user);
        Task UpdaloadCnhImageAsync(Stream stream, string contentType, string path);
        Task UpdateCNHImagePathAsync(Guid idUser, string imagePath);
        Task<string> GetLastCNHImagePathOfUserAsync(Guid idUser);
        Task<bool> IsCNHUniqueAsync(string cnhNumber);
        Task<bool> IsCPJUniqueAsync(string cnpj);
        Task<IEnumerable<UserTypeCNH>> GetAllCNHTypeOfUserIdAsync(Guid idUser);
    }
}
