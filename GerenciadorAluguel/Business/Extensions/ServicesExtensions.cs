using Business.Services;
using Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Extensions
{
    public static class ServicesExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IMotorcycleService, MotorcycleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDeliveryOrderService, DeliveryOrderService>();
        }
    }
}
