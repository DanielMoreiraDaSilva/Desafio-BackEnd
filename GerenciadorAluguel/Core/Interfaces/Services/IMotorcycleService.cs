using Core.Models;
using Core.Models.Filters;

namespace Core.Interfaces.Services
{
    public interface IMotorcycleService
    {
        Task AddMotorcycleAsync(Motorcycle motorcycle);
        Task<IEnumerable<Motorcycle>> GetListMotorcycleAsync(MotorcycleFilter filter = null);
        Task<bool> IsPlateUniqueAsync(string plate);
        Task<bool> ThereIsRentalForMotorcycleAsync(Guid idMotorcycle);
        Task UpdatePlateAsync(Guid idMotorcycle, string plate);
        Task DeleteAsync(Guid idMotorcycle);
        Task<IEnumerable<RentalPlan>> GetAllRentalPlansAsync();
        Task AddMotorcycleRentalAsync(MotorcycleRentalPlan rental);
        Task<ValidateReturn> ValidateNewMotorcycleRental(MotorcycleRentalPlan rental);
        Task<double> CalculateCostPlan(Guid idRentalPlan, DateTime expectedReturnDate);
    }
}
