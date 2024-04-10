using Amazon;
using Amazon.SimpleNotificationService;
using Lambda.Service;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace Lambda
{
    public static class DependencyContainer
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services) 
        {
            
            // Postgre Connection
            var connectionString = Environment.ExpandEnvironmentVariables("ConnectionString");
            services.AddScoped(provider => new Func<IDbConnection>(() => new NpgsqlConnection(connectionString)));

            services.AddScoped<IUseCaseProcessMessage, UseCaseProcessMessage>();

            services.AddScoped<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>(
                x => new AmazonSimpleNotificationServiceClient(new AmazonSimpleNotificationServiceConfig() { 
                    ServiceURL = Environment.ExpandEnvironmentVariables("HostPathAWS"),
                    RegionEndpoint = RegionEndpoint.SAEast1
                }));

            return services;
        }
    }
}
