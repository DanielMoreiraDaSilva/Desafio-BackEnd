using Core.Models;

namespace Core.Interfaces.Repositories
{
    public interface IMotorcycleRepository
    {
        Task AddMotorcycleAsync(Motorcycle motorcycle);
        Task UpdatePlateAsync(Guid idMotorcycle, string plate);
        Task DeleteAsync(Guid idMotorcycle);
        Task<IEnumerable<Motorcycle>> GetListMotorcycleAsync(MotorcycleFilter filter);
        Task<bool> ThereIsRentalForMotorcycleAsync(Guid idMotorcycle);
        Task<bool> IsPlateUniqueAsync(string plate);
        Task<IEnumerable<RentalPlan>> GetListRentalPlansAsync(Guid? id = null);
        Task AddMotorcycleRentalAsync(MotorcycleRentalPlan rental);
    }
}
