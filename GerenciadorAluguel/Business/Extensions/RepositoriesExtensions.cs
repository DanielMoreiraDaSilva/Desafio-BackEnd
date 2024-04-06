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
        }
    }
}
