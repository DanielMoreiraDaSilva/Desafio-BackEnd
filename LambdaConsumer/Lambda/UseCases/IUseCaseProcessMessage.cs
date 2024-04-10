using Amazon.Lambda.Core;
using Lambda.Models;

namespace Lambda.Service
{
    public interface IUseCaseProcessMessage
    {
        Task ExecuteAsync(DeliveryOrder deliveryOrder, ILambdaContext context);
    }
}
