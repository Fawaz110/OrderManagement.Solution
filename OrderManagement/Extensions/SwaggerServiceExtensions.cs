using Microsoft.OpenApi.Models;

namespace OrderManagement.Extensions
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Management", Version = "v1" });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Description = "Jwt Bearer Token: bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "bearer"
                    }
                };

                c.AddSecurityDefinition("bearer", securityScheme);

                var securityRequirements = new OpenApiSecurityRequirement
                {
                    {securityScheme, new[] {"bearer"} }
                };

                c.AddSecurityRequirement(securityRequirements);
            });

            return services;
        }
    }
}
