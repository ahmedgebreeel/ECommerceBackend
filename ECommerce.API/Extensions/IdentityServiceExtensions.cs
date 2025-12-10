using ECommerce.Business.DTOs.Errors;
using ECommerce.Core.Entities;
using ECommerce.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

namespace ECommerce.API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {

            //Add Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                //Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;

                //lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // Lock for 15 min
                options.Lockout.MaxFailedAccessAttempts = 5; // After 5 wrong tries
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            //Add Authentication & JWT
            services.AddAuthentication(options =>
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
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]!))
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            logger.LogWarning("Token expired for user.");
                        }
                        else if (context.Exception is SecurityTokenInvalidSignatureException)
                        {
                            logger.LogError(context.Exception, "Invalid Token Signature detected!");
                        }
                        else
                        {
                            logger.LogError(context.Exception, "Authentication failed.");
                        }

                        return Task.CompletedTask;
                    },

                    OnChallenge = async context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogWarning("401 Unauthorized triggered. Error: {Error}, Description: {Desc}",
                            context.Error,
                            context.ErrorDescription ?? "Token missing or invalid");
                        context.HandleResponse();
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json";
                        var errorResponse = new ApiErrorResponseDto
                        {
                            StatusCode = 401,
                            Message = "You are not authorized.",
                            Detail = context.ErrorDescription ?? "Token is missing, invalid, or expired.",
                            TimeStamp = DateTime.UtcNow
                        };
                        await context.Response.WriteAsJsonAsync(errorResponse);
                    },
                    OnForbidden = async context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogWarning("403 Forbidden triggered for user: {User}", context.HttpContext.User.Identity?.Name);

                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var errorResponse = new ApiErrorResponseDto
                        {
                            StatusCode = 403,
                            Message = "You are not authorized to access this resource.",
                            Detail = "You do not have the required permissions (Role) to perform this action.",
                            TimeStamp = DateTime.UtcNow
                        };

                        await context.Response.WriteAsJsonAsync(errorResponse);
                    }
                };
            });

            return services;
        }
    }
}