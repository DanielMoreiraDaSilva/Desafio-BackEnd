using Amazon.Lambda.Core;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Dapper;
using Lambda.Models;
using System.Data;

namespace Lambda.Service
{
    internal class UseCaseProcessMessage : IUseCaseProcessMessage
    {
        private readonly Func<IDbConnection> _connection;
        private readonly IAmazonSimpleNotificationService _client;
        public UseCaseProcessMessage(Func<IDbConnection> connection,
            IAmazonSimpleNotificationService client)
        {
            _connection = connection;
            _client = client;
        }
        public async Task ExecuteAsync(DeliveryOrder deliveryOrder, ILambdaContext context)
        {
            context.Logger.LogInformation("Starting use case");
            ControlConnection connection = new()
            {
                Connection = _connection.Invoke()
            };

            connection.Connection.Open();
            connection.Transaction = connection.Connection.BeginTransaction();

            try
            {
                var listContact = await ListValidUserToNotify();

                foreach (var user in listContact) 
                {
                    // To send a SMS We need to use the E.164 Format at phone number -> '+5519977776666'
                    var request = new PublishRequest()
                    {
                        Message = $"Delivery order {deliveryOrder.Id} disponible",
                        PhoneNumber = user.Contact
                    };

                    context.Logger.LogInformation("Starting send notify");

                    // The publish is commited because we can't send a SMS from localstack aws
                    //await _client.PublishAsync(request);

                    context.Logger.LogInformation($"Notification sent to {user.Name}");

                    await InsertUserNotifyHistory(user.Id, deliveryOrder.Id, connection);
                }

                await InsertNotifyHistory(deliveryOrder, connection);
                context.Logger.LogInformation($"Notify history inserted");

                connection.Transaction.Commit();
                connection.Connection.Close();
            }
            catch (Exception)
            {
                connection.Transaction.Rollback();
                connection.Connection.Close();
                throw;
            }
        }

        private async Task<IEnumerable<User>> ListValidUserToNotify()
        {
            var query = @"
                WITH USERCANBENOTIFIED AS 
                (SELECT U.*
                FROM PUBLIC.USERSYSTEM U
                LEFT JOIN MOTORCYCLERENTAL MR
                    ON MR.IDUSER = U.ID AND NOW() BETWEEN MR.RENTALSTARTDATE AND MR.RENTALENDDATE 
                LEFT JOIN DELIVERYORDERACCEPTANCE D
                    ON D.IDUSER = U.ID
                LEFT JOIN DELIVERYORDER DLO
                    ON DLO.ID = D.IDDELIVERYORDER
                WHERE MR.ID IS NOT NULL AND (D.ID IS NULL OR DLO.IDSTATUSDELIVERYORDER != @STATUS))
                SELECT U.ID, U.NAME,'' AS CONTACT
                FROM USERSYSTEM U
                LEFT JOIN USERCANBENOTIFIED UN
                    ON UN.ID = U.ID
                WHERE UN.ID IS NOT NULL";
            
            DynamicParameters parameters = new ();
            parameters.Add("@STATUS", Guid.Parse(Constantes.STATUSDELIVERYORDER_ACCEPT));

            using IDbConnection connection = _connection.Invoke();

            var result = await connection.QueryAsync<User>(query, parameters);

            return result;
        }

        private async Task InsertNotifyHistory(DeliveryOrder deliveryOrder, ControlConnection connection)
        {
            var query = @"
                INSERT INTO DELIVERYORDERNOTIFYHISTORY (IDDELIVERYORDER, DATADELIVERYORDERCREATE, DATENOTIFY) 
                VALUES (@IDDELIVERYORDER, @DATADELIVERYORDERCREATE, @DATENOTIFY)";

            DynamicParameters parameters = new();
            parameters.Add("@IDDELIVERYORDER", deliveryOrder.Id);
            parameters.Add("@DATADELIVERYORDERCREATE", deliveryOrder.DateDeliveryOrderCreate);
            parameters.Add("@DATENOTIFY", deliveryOrder.DateNotified);

            await connection.Connection.ExecuteAsync(query, parameters, connection.Transaction);
        }

        private async Task InsertUserNotifyHistory(Guid IdUser, Guid IdDeliveryOrder, ControlConnection connection)
        {
            var query = "INSERT INTO UserNotifyHistory (IDUser, IDDeliveryOrder) VALUES (@IDUSER, @IDDELIVERYORDER)";

            DynamicParameters parameters = new();
            parameters.Add("@IDUSER", IdUser);
            parameters.Add("@IDDELIVERYORDER", IdDeliveryOrder);

            await connection.Connection.ExecuteAsync(query, parameters, connection.Transaction);
        }
    }
}
