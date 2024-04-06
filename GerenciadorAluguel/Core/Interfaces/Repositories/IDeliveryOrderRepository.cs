using Core.Models;

namespace Core.Interfaces.Repositories
{
    public interface IDeliveryOrderRepository
    {
        Task AddDeliveyAsync(DeliveryOrder deliveryOrder);
        Task<IEnumerable<StatusDeliveryOrder>> GetAllAsync();
        Task<bool> IsStatusValidAsync(Guid id);
        Task SendMessageSQSAsync(DeliveryOrder deliveryOrder);
    }
}
