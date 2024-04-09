using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Lambda.Models;
using Lambda.Service;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Lambda;

public class Function
{
    private readonly IServiceProvider _serviceProvider;
    public Function()
    {
        _serviceProvider = ConfigureServices(new ServiceCollection()).BuildServiceProvider();
    }
    public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
        try
        {
            context.Logger.LogInformation($"Starting lambda");
            context.Logger.LogInformation($"Messagebody: {evnt.Records[0].Body}");

            var useCase = _serviceProvider.GetService<IUseCaseProcessMessage>();

            context.Logger.LogInformation("Get service");

            var dateNotified = DateTime.Now;

            context.Logger.LogInformation("Mapping body message and sending notify");

            foreach (var message in evnt.Records)
            {
                var deliveryOrder = JsonSerializer.Deserialize<DeliveryOrder>(message.Body);

                deliveryOrder.DateNotified = dateNotified;

                await useCase.ExecuteAsync(deliveryOrder, context);
            }

            context.Logger.LogInformation("Lambda process concluded");
        }
        catch (Exception ex)
        {
            context.Logger.LogError(ex.Message);
            throw;
        }
    }

    private IServiceCollection ConfigureServices(IServiceCollection services)
    {
        services.RegisterServices();
        return services;
    }
}