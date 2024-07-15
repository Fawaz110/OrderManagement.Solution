﻿using Core.Services.Contract;
using OrderManagement.Entities;
using Service;

namespace OrderManagement.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }

    }
}