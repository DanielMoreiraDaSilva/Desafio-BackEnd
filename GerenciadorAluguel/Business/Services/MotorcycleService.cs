using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;

namespace Business.Services
{
    internal class MotorcycleService : IMotorcycleService
    {
        private readonly IMotorcycleRepository _motorcycleRepository;
        private readonly IUserRepository _userRepository;
        public MotorcycleService(IMotorcycleRepository motorcycleRepository, 
            IUserRepository userRepository)
        {
            _motorcycleRepository = motorcycleRepository;
            _userRepository = userRepository;

        }

        public async Task AddMotorcycleAsync(Motorcycle motorcycle) =>
            await _motorcycleRepository.AddMotorcycleAsync(motorcycle).ConfigureAwait(false);

        public async Task DeleteAsync(Guid idMotorcycle) =>
            await _motorcycleRepository.DeleteAsync(idMotorcycle).ConfigureAwait(false);

        public async Task<IEnumerable<Motorcycle>> GetListMotorcycleAsync(MotorcycleFilter filter) =>
            await _motorcycleRepository.GetListMotorcycleAsync(filter).ConfigureAwait(false);

        public async Task UpdatePlateAsync(Guid idMotorcycle, string plate) =>
            await _motorcycleRepository.UpdatePlateAsync(idMotorcycle, plate).ConfigureAwait(false);

        public async Task<bool> ThereIsRentalForMotorcycleAsync(Guid idMotorcycle) =>
            await _motorcycleRepository.ThereIsRentalForMotorcycleAsync(idMotorcycle).ConfigureAwait(false);

        public async Task<bool> IsPlateUniqueAsync(string plate) =>
            await _motorcycleRepository.IsPlateUniqueAsync(plate).ConfigureAwait(false);

        public async Task<IEnumerable<RentalPlan>> GetAllRentalPlansAsync() =>
            await _motorcycleRepository.GetListRentalPlansAsync().ConfigureAwait(false);

        public async Task AddMotorcycleRentalAsync(MotorcycleRentalPlan rental)
        {
            var rentalPlan = (await _motorcycleRepository.GetListRentalPlansAsync(rental.IdRentalPlan).ConfigureAwait(false)).First();

            rental.RentalStartDate = DateTime.Now.Date.AddDays(1);
            rental.RentalEndDate = rental.RentalEndDate.AddDays(rentalPlan.DurationInDays);

            await _motorcycleRepository.AddMotorcycleRentalAsync(rental).ConfigureAwait(false);
        }

        public async Task<ValidateReturn> ValidateNewMotorcycleRental(MotorcycleRentalPlan rental)
        {
            ValidateReturn result = new();

            var listDisponibleMotorcycle = await GetListMotorcycleAsync(new() { Disponible = true }).ConfigureAwait(false);

            var validateMotorcycleDisponible = listDisponibleMotorcycle.Where(x => x.Id == rental.IdMotorcycle).Any();

            if (!validateMotorcycleDisponible)
                result.Errors.Add("Está moto não esta disponivel no momento");

            var userCNHTypeList = await _userRepository.GetAllCNHTypeOfUserIdAsync(rental.IdUser).ConfigureAwait(false);

            if (!userCNHTypeList.Where(x => x.IdCNHType == Guid.Parse("d498282f-ffd9-4649-a4bd-4769ae7b7f07")).Any())
                result.Errors.Add("O usuario deve possuir CNH do tipo A");

            return result;
        }

        public async Task<double> CalculateCostPlan(Guid idRentalPlan, DateTime expectedReturnDate)
        {
            var rentalPlan = (await _motorcycleRepository.GetListRentalPlansAsync(idRentalPlan).ConfigureAwait(false)).First();
            var planCost = rentalPlan.DailyCost * rentalPlan.DurationInDays;

            var dateStartPlan = DateTime.Now.Date.AddDays(1);
            var dateEndPlan = dateStartPlan.AddDays(rentalPlan.DurationInDays);

            if (expectedReturnDate < dateEndPlan)
            {
                var daysUsed = expectedReturnDate.Subtract(dateStartPlan).Days;
                var daysNotUsed = dateEndPlan.Subtract(expectedReturnDate).Days;

                var costDaysNotUsed = rentalPlan.DailyCost * daysNotUsed;

                return (rentalPlan.DailyCost * daysUsed) + (costDaysNotUsed * rentalPlan.FactorCalculateCost);
            }
            else if (expectedReturnDate > dateEndPlan)
            {
                var extraDays = dateEndPlan.Subtract(expectedReturnDate).Days;

                return planCost + (extraDays * rentalPlan.CostExtraDays);
            }
            else
                return planCost;
        }
    }
}
