using Amazon.SQS;
using Amazon.SQS.Model;
using Core;
using Core.Interfaces.Repositories;
using Core.Models;
using Core.Models.Configurations;
using Dapper;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text.Json;

namespace Data.Repositories
{
    public class DeliveryOrderRepository : IDeliveryOrderRepository
    {
        private readonly Func<IDbConnection> _connection;
        private readonly IAmazonSQS _client;
        private readonly IUtilRepository _utilRepository;
        private readonly AwsConfig _awsConfig;
        public DeliveryOrderRepository(Func<IDbConnection> connection,
            IAmazonSQS client,
            IUtilRepository utilRepository,
            IOptions<AwsConfig> awsConfig)
        {
            _connection = connection;
            _client = client;
            _utilRepository = utilRepository;
            _awsConfig = awsConfig.Value;
        }
        public async Task AddDeliveryAsync(DeliveryOrder deliveryOrder)
        {
            var query = @"
                INSERT INTO DELIVERYORDER (ID, DATACREATE, COSTDELIVERY, IDSTATUSDELIVERYORDER) 
                VALUES (@ID, @DATECREATE, @COSTDELIVERY, @STATUSID);";

            DynamicParameters parameters = new();
            parameters.Add("@ID", deliveryOrder.Id.Value);
            parameters.Add("@DATECREATE", deliveryOrder.DateCreate.Value);
            parameters.Add("@COSTDELIVERY", deliveryOrder.CostDelivery.Value);
            parameters.Add("@STATUSID", deliveryOrder.IdStatusDeliveryOrder.Value);

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
            await _client.SendMessageAsync(new SendMessageRequest() { QueueUrl = _awsConfig.SQSUrl, MessageBody = JsonSerializer.Serialize(deliveryOrder) }).ConfigureAwait(false);

        }

        public async Task<bool> IsStatusValidAsync(Guid id) =>
            !await _utilRepository.IsFieldValueUniqueAsync(Constantes.TABLE_NAME_STATUSDELIVERYORDER, nameof(id), id).ConfigureAwait(false);

        public async Task AcceptDeliveryOrderByUser(Guid idUser, Guid idDeliveryOrder, ControlConnection connection)
        {
            string query = "INSERT INTO DELIVERYORDERACCEPTANCE (IDUSER, IDDELIVERYORDER) VALUES (@IDUSER, @IDDELIVERYORDER)";

            DynamicParameters parameters = new();
            parameters.Add("@IDUSER", idUser);
            parameters.Add("@IDDELIVERYORDER", idDeliveryOrder);

            await connection.Connection.ExecuteAsync(query, parameters, connection.Transaction);
        }

        public async Task UpdateStatusDeliveryOrderAsync(Guid id, Guid idStatusDeliveryOrder, ControlConnection connection) =>
            await _utilRepository.UpdateFieldAsync(Constantes.TABLE_NAME_DELIVERYORDER, nameof(idStatusDeliveryOrder), idStatusDeliveryOrder, nameof(id), id, connection).ConfigureAwait(false);
    }
}
