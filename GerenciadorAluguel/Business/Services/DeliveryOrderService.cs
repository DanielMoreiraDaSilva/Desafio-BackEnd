using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;

namespace Business.Services
{
    public class DeliveryOrderService : IDeliveryOrderService
    {
        private readonly IDeliveryOrderRepository _deliveryOrderRepository;
        public DeliveryOrderService(IDeliveryOrderRepository deliveryOrderRepository)
        {
            _deliveryOrderRepository = deliveryOrderRepository;
        }
        public async Task AddDeliveyAsync(DeliveryOrder deliveryOrder)
        {
            await _deliveryOrderRepository.AddDeliveyAsync(deliveryOrder).ConfigureAwait(false);

            await _deliveryOrderRepository.SendMessageSQSAsync(deliveryOrder).ConfigureAwait(false);
        }

        public async Task<IEnumerable<StatusDeliveryOrder>> GetAllStatusDeliveryAsync()
        {
            return await _deliveryOrderRepository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task<bool> IsStatusValidAsync(Guid id) =>
            await _deliveryOrderRepository.IsStatusValidAsync(id).ConfigureAwait(false);
    }
}
