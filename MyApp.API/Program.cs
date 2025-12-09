using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyApp.API.Data;
using MyApp.API.Entities;
using MyApp.API.Interfaces;
using MyApp.API.Mappings;
using MyApp.API.Middleware;
using MyApp.API.Services;
using Serilog;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {

            try
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();

                Log.Information("Starting Server...");

                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.


                //1.Register SeriLog
                builder.Host.UseSerilog((context, loggerConfiguration) =>
                {
                    loggerConfiguration.WriteTo.Console();
                    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
                });

                //2.Add GlobalExceptionHandler
                builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
                builder.Services.AddProblemDetails();

                //3.Register DbConext
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("constr"))
                );

                //4.Add Identity
                builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.User.RequireUniqueEmail = true;
                })
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

                //5.Jwt Authentication Configuration
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new()
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = builder.Configuration["Jwt:Issuer"],
                            ValidAudience = builder.Configuration["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
                        };
                    });

                //6.Register Automapper
                builder.Services.AddAutoMapper(cfg =>
                    cfg.AddProfile<MappingProfile>()
                );

                //7.Register Services in IOC Container
                builder.Services.AddScoped<IBrandService, BrandService>();
                builder.Services.AddScoped<ICategoryService, CategoryService>();
                builder.Services.AddScoped<IProductService, ProductService>();
                builder.Services.AddScoped<IProductImageService, ProductImageService>();
                builder.Services.AddScoped<IOrderService, OrderService>();
                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddScoped<ITokenService, TokenService>();

                //8. AddControllers
                builder.Services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        // accept enums as strings, case insensitive
                        options.JsonSerializerOptions.Converters.Add(
                            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                        );
                    });

                //9.Add OpenApi
                // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                builder.Services.AddOpenApi();


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

                app.UseAuthentication();

                app.UseAuthorization();

                app.MapControllers();

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
