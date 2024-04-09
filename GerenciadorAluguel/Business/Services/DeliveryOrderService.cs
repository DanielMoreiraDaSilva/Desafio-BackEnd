using Core;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Business.Services
{
    public class DeliveryOrderService : IDeliveryOrderService
    {
        private readonly IDeliveryOrderRepository _deliveryOrderRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<DeliveryOrderService> _logger;
        private readonly Func<IDbConnection> _connection;
        public DeliveryOrderService(IDeliveryOrderRepository deliveryOrderRepository, 
            IUserRepository userRepository,
            ILogger<DeliveryOrderService> logger,
            Func<IDbConnection> connection)
        {
            _deliveryOrderRepository = deliveryOrderRepository;
            _userRepository = userRepository;
            _logger = logger;
            _connection = connection;
        }

        public async Task AcceptDeliveryOrderByUser(Guid idUser, Guid idDeliveryOrder)
        {
            ControlConnection connection = new()
            {
                Connection = _connection.Invoke()
            };

            connection.Connection.Open();
            connection.Transaction = connection.Connection.BeginTransaction();

            try
            {
                await _deliveryOrderRepository.AcceptDeliveryOrderByUser(idUser, idDeliveryOrder, connection).ConfigureAwait(false);

                await _deliveryOrderRepository.UpdateStatusDeliveryOrderAsync(idDeliveryOrder, Guid.Parse(Constantes.STATUSDELIVERYORDER_ACCEPT), connection);

                connection.Transaction.Commit();
                connection.Connection.Close();
                _logger.LogInformation("Delivery order accepted by user {UserId}. Delivery order ID: {DeliveryOrderId}", idUser, idDeliveryOrder);
            }
            catch (Exception ex)
            {
                connection.Transaction.Rollback();
                connection.Connection.Close();
                _logger.LogError(ex, "Error occurred while accepting delivery order by user. User ID: {UserId}, Delivery Order ID: {DeliveryOrderId}", idUser, idDeliveryOrder);
                throw;
            }
        }

        public async Task AddDeliveryAsync(DeliveryOrder deliveryOrder)
        {
            try
            {
                await _deliveryOrderRepository.AddDeliveryAsync(deliveryOrder).ConfigureAwait(false);

                _logger.LogInformation("Delivery order added successfully. Delivery order ID: {DeliveryOrderId}", deliveryOrder.Id);

                await _deliveryOrderRepository.SendMessageSQSAsync(deliveryOrder).ConfigureAwait(false);

                _logger.LogInformation("Starting to send notification to user. Delivery order ID: {DeliveryOrderId}", deliveryOrder.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding delivery order. Delivery order ID: {DeliveryOrderId}", deliveryOrder.Id);
                throw;
            }
        }

        public async Task ConcludeDeliveryOrderByUser(Guid idDeliveryOrder) =>
            await _deliveryOrderRepository.UpdateStatusDeliveryOrderAsync(idDeliveryOrder, Guid.Parse(Constantes.STATUSDELIVERYORDER_CONCLUDED));

        public async Task<IEnumerable<StatusDeliveryOrder>> GetAllStatusDeliveryAsync()
        {
            return await _deliveryOrderRepository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task<bool> IsStatusValidAsync(Guid id) =>
            await _deliveryOrderRepository.IsStatusValidAsync(id).ConfigureAwait(false);

        public async Task<bool> IsUserNotifiedValidAsync(Guid idUser, Guid idDeliveryOrder)
        {
            var listUserNotified = await _userRepository.GetListUserNotifiedByIdDeliveryOrder(idDeliveryOrder).ConfigureAwait(false);

            if (listUserNotified.Where(x => x.Id == idUser).Any())
                return true;
            else
                return false;
        }
    }
}
