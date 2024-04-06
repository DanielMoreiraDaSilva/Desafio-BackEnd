using Amazon.SQS;
using Core.Interfaces.Repositories;
using Core.Models;
using Dapper;
using System.Data;
using System.Text.Json;

namespace Data.Repositories
{
    internal class DeliveryOrderRepository : IDeliveryOrderRepository
    {
        private readonly Func<IDbConnection> _connection;
        private readonly IAmazonSQS _client;
        private readonly IUtilRepository _utilRepository;
        public DeliveryOrderRepository(Func<IDbConnection> connection, 
            IAmazonSQS client,
            IUtilRepository utilRepository)
        {
            _connection = connection;
            _client = client;
            _utilRepository = utilRepository;
        }
        public async Task AddDeliveyAsync(DeliveryOrder deliveryOrder)
        {
            var query = @"
                INSERT INTO DELIVERYORDER (ID, DATACREATE, COSTDELIVERY, IDSTATUSDELIVERYORDER) 
                VALUES (@Id, @DateCreate, @CostDelivery, @StatusId);";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", deliveryOrder.Id);
            parameters.Add("@DateCreate", deliveryOrder.DateCreate);
            parameters.Add("@CostDelivery", deliveryOrder.CostDelivery);
            parameters.Add("@StatusId", deliveryOrder.IdStatusDeliveryOrder);

            using IDbConnection connection = _connection.Invoke();

            await connection.ExecuteAsync(query, parameters);
        }

        public async Task<IEnumerable<StatusDeliveryOrder>> GetAllAsync()
        {
            var query = "SELECT ID, STATUS FROM STATUSDELIVERYORDER";

            using IDbConnection connection = _connection.Invoke();

            return await connection.QueryAsync<StatusDeliveryOrder>(query);
        }

        public async Task SendMessageSQSAsync(DeliveryOrder deliveryOrder)
        {
            try
            {
                var response = await _client.SendMessageAsync("http://localhost:4566/000000000000/delivery-order", JsonSerializer.Serialize(deliveryOrder));
            }
            catch (AmazonSQSException ex)
            {
                //log
            }
            catch (Exception ex)
            {
                //log
            }
        }

        public async Task<bool> IsStatusValidAsync(Guid id) =>
            await _utilRepository.IsFieldValueUniqueAsync("STATUSDELIVERYORDER", nameof(id), id);
    }
}
