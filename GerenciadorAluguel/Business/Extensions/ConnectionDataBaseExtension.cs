using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace Business.Extensions
{
    public static class ConnectionDataBaseExtension
    {
        public static void ConfigureConnections(this IServiceCollection services, ConfigurationManager configuration)
        {
            var connectionString = configuration["ConnectionStrings:DefaultConnection"];
            services.AddScoped(provider => new Func<IDbConnection>(() => new NpgsqlConnection(connectionString)));
        }
    }
}
