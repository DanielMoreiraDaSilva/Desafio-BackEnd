using Core.Models;

namespace Core.Interfaces.Services
{
    public interface IDeliveryOrderService
    {
        Task AddDeliveryAsync(DeliveryOrder deliveryOrder);
        Task<IEnumerable<StatusDeliveryOrder>> GetAllStatusDeliveryAsync();
        Task<bool> IsStatusValidAsync(Guid id);
        Task<bool> IsUserNotifiedValidAsync(Guid idUser, Guid idDeliveryOrder);
        Task AcceptDeliveryOrderByUser(Guid idUser, Guid idDeliveryOrder);
        Task ConcludeDeliveryOrderByUser(Guid idDeliveryOrder);
    }
}
