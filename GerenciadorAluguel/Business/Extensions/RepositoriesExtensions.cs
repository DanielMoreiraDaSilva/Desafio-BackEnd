using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SQS;
using Core.Interfaces.Repositories;
using Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Extensions
{
    public static class RepositoriesExtensions
    {
        public static void ConfigureRepositories(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDeliveryOrderRepository, DeliveryOrderRepository>();
            services.AddScoped<IUtilRepository, UtilRepository>();
            services.AddScoped<IAmazonS3, AmazonS3Client>(x => new AmazonS3Client(new BasicAWSCredentials(configuration["localstackAccessKey"], configuration["localstackSecretKey"]), new AmazonS3Config() { ServiceURL = configuration["LocalStackAWS:HostPath"], ForcePathStyle = true }));
            services.AddScoped<IAmazonSQS, AmazonSQSClient>(x => new AmazonSQSClient(new BasicAWSCredentials(configuration["localstackAccessKey"], configuration["localstackSecretKey"]), new AmazonSQSConfig() { ServiceURL = configuration["LocalStackAWS:HostPath"]}));
        }
    }
}
