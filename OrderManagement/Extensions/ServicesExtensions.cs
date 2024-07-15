using Core.Repositories.Contract;
using Core.Services.Contract;
using OrderManagement.Entities;
using Repository;
using Service;
using Service.Profiles;

namespace OrderManagement.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IAuthService, AuthService>();

            services.AddAutoMapper(x => x.AddProfile(new ProductProfile()));

            return services;
        }

    }
}
