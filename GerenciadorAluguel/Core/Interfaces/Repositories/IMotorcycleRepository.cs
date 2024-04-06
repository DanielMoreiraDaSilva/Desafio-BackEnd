using Core.Models;

namespace Core.Interfaces.Repositories
{
    public interface IMotorcycleRepository
    {
        Task AddAsync(Motorcycle motorcycle);
        Task UpdatePlateAsync(Guid idMotorcycle, string plate);
        Task DeleteAsync(Guid idMotorcycle);
        Task<IEnumerable<Motorcycle>> GetAsync(MotorcycleFilter filter = null);
    }
}
