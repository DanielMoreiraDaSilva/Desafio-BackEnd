using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Business.Extensions
{
    public static class SeaggerExtensions
    {
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API",
                    Description = "API",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "API",
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Example License",
                    }
                });
            });
        }
    }
}
