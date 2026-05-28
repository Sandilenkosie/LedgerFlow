using Microsoft.Extensions.DependencyInjection;
using Application.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            // Register AutoMapper profiles, MediatR handlers and other application services
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
            services.AddSingleton<IJwtService, JwtService>();
            return services;
        }
    }
}
