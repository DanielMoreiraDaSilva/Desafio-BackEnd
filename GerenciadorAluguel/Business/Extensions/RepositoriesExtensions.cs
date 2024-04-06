using Amazon.S3;
using Amazon.SQS;
using Core.Interfaces.Repositories;
using Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Extensions
{
    public static class RepositoriesExtensions
    {
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUtilRepository, UtilRepository>();
            services.AddScoped<IAmazonS3, AmazonS3Client>(x => new AmazonS3Client(new AmazonS3Config() { ServiceURL = "http://localhost:4566", ForcePathStyle = true }));
            services.AddScoped<IAmazonSQS, AmazonSQSClient>(x => new AmazonSQSClient(new AmazonSQSConfig() { ServiceURL = "http://localhost:4566" }));
        }
    }
}
