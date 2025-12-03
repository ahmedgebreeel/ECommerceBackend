using Microsoft.EntityFrameworkCore;
using MyApp.API.Data;
using MyApp.API.Interfaces;
using MyApp.API.Mappings;
using MyApp.API.Services;

namespace MyApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //Register DbConext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("constr"))
            );

            //Register Automapper
            builder.Services.AddAutoMapper(cfg =>
                cfg.AddProfile<MappingProfile>()
            );

            //Register Service Layer
            builder.Services.AddScoped<IBrandService, BrandService>();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                    options.SwaggerEndpoint("/openapi/v1.json", "My Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
