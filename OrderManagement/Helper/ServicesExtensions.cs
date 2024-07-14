using Core.Services.Contract;
using Service;

namespace OrderManagement.Helper
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
