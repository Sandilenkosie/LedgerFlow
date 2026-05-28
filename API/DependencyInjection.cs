using Application;
using Domain;

namespace API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiDI(this IServiceCollection services)
        {
            // Register API services here
            services.AddApplicationDI()
                .AddDomainDI();
            return services;
        }
    }
}
