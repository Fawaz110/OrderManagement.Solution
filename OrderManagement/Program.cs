using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderManagement.DbContexts;
using OrderManagement.Entities;
using OrderManagement.Errors;
using OrderManagement.Extensions;
using OrderManagement.Helper;
using OrderManagement.Middlewares;
using System.Text;

namespace OrderManagement
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerDocumentation();

            #region Connection & Identity Configurations
            builder.Services.AddDbContext<StoreDbContext>(options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
                });

            builder.Services.AddIdentityService(builder.Configuration);
            #endregion

            builder.Services.AddServices();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", options =>
                {
                    options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(p => p.Value.Errors.Count() > 0)
                                                            .SelectMany(p => p.Value.Errors)
                                                            .Select(p => p.ErrorMessage).ToList();

                    var validationErrorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(validationErrorResponse);
                };
            }); 

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();

            var scope = app.Services.CreateScope();

            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();

            var logger = loggerFactory.CreateLogger<Program>();

            try
            {
                var context = scope.ServiceProvider.GetService<StoreDbContext>();
                
                await context.Database.MigrateAsync();

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                await ContextSeed.ApplyRolesSeed(roleManager, logger);

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                await ContextSeed.ApplyUserSeeding(userManager, logger);

                await ContextSeed.ApplyProductSeeding(context, logger);

            }
            catch (Exception ex)
            {
                logger.LogWarning("No Pending Migrations");
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("MyPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}