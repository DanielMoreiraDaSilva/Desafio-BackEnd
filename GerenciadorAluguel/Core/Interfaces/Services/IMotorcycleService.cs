using Core.Models;

namespace Core.Interfaces.Services
{
    public interface IMotorcycleService
    {
        Task AddAsync(Motorcycle motorcycle);
        Task<IEnumerable<Motorcycle>> GetAsync(MotorcycleFilter filter = null);
        Task<bool> ValidateUniquePlateAsync(string plate);
        Task<bool> ValidateHireMotorcycleAsync(Guid idMotorcycle);
        Task UpdatePlateAsync(Guid idMotorcycle, string plate);
        Task DeleteAsync(Guid idMotorcycle);
    }
}
