using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;

namespace Business.Services
{
    internal class MotorcycleService : IMotorcycleService
    {
        private readonly IMotorcycleRepository _motorcycleRepository;
        private readonly IUtilRepository _utilRepository;
        public MotorcycleService(IMotorcycleRepository motorcycleRepository,
            IUtilRepository utilRepository)
        {
            _motorcycleRepository = motorcycleRepository;
            _utilRepository = utilRepository;
        }

        public async Task AddAsync(Motorcycle motorcycle) =>
            await _motorcycleRepository.AddAsync(motorcycle).ConfigureAwait(false);

        public async Task DeleteAsync(Guid idMotorcycle) =>
            await _motorcycleRepository.DeleteAsync(idMotorcycle).ConfigureAwait(false);

        public async Task<IEnumerable<Motorcycle>> GetAsync(MotorcycleFilter filter = null) =>
            await _motorcycleRepository.GetAsync(filter).ConfigureAwait(false);

        public async Task UpdatePlateAsync(Guid idMotorcycle, string plate) =>
            await _motorcycleRepository.UpdatePlateAsync(idMotorcycle, plate).ConfigureAwait(false);

        public async Task<bool> ValidateHireMotorcycleAsync(Guid idMotorcycle) =>
            await _utilRepository.IsFieldValueUniqueAsync("HIREMOTORCYCLE", nameof(idMotorcycle), idMotorcycle).ConfigureAwait(false);

        public async Task<bool> ValidateUniquePlateAsync(string plate) =>
            await _utilRepository.IsFieldValueUniqueAsync("HIREMOTORCYCLE", nameof(plate), plate).ConfigureAwait(false);
    }
}
