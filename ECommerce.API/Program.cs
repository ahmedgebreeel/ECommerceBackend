using ECommerce.API.Extensions;
using ECommerce.Data;
using Serilog;

namespace ECommerce.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            try
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();

                Log.Information("Starting Server...");

                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Host.ConfigureSerilog();

                builder.Services.AddApplicationServices(builder.Configuration);

                builder.Services.AddIdentityServices(builder.Configuration);

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                app.UseExceptionHandler();

                if (app.Environment.IsDevelopment())
                {
                    app.MapOpenApi();
                    app.UseSwaggerUI(options =>
                        options.SwaggerEndpoint("/openapi/v1.json", "My Api v1"));
                }

                app.UseHttpsRedirection();
                app.UseCors("AngularApp");
                app.UseRateLimiter();
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers()
                    .RequireRateLimiting("standard");

                await DbInitializer.SeedDataAsync(app.Services);

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "server terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
