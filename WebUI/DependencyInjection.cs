using Microsoft.Extensions.DependencyInjection;
using Application;
using Infrastructure;

namespace WebUI;

public static class DependencyInjection
{
    public static IServiceCollection AddWebUIDI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationDI();
        services.AddInfrastructureDI(configuration);
        return services;
    }
}
