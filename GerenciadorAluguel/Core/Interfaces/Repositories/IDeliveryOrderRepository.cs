using Core.Models;

namespace Core.Interfaces.Repositories
{
    public interface IDeliveryOrderRepository
    {
        Task AddDeliveryAsync(DeliveryOrder deliveryOrder);
        Task<IEnumerable<StatusDeliveryOrder>> GetAllAsync();
        Task<bool> IsStatusValidAsync(Guid id);
        Task SendMessageSQSAsync(DeliveryOrder deliveryOrder);
        Task AcceptDeliveryOrderByUser(Guid idUser, Guid idDeliveryOrder, ControlConnection connection = null);
        Task UpdateStatusDeliveryOrderAsync(Guid id, Guid idStatusDeliveryOrder, ControlConnection connection = null);
    }
}
