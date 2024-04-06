using Core.Models;

namespace Core.Interfaces.Services
{
    public interface IDeliveryOrderService
    {
        Task AddDeliveyAsync(DeliveryOrder deliveryOrder);
        Task<IEnumerable<StatusDeliveryOrder>> GetAllStatusDeliveryAsync();
        Task<bool> IsStatusValidAsync(Guid id);
    }
}
