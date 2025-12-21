using ECommerce.API.Infrastructure;
using ECommerce.API.Middleware;
using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.Interfaces;
using ECommerce.Business.Mappings;
using ECommerce.Business.Services;
using ECommerce.Core.Settings;
using ECommerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;


namespace ECommerce.API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            //Global Error Handling
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            // Register DbConext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("constr"))
            );

            //Register Automapper
            services.AddAutoMapper(cfg =>
                cfg.AddProfile<MappingProfile>()
            );

            //Register HttpContextAccessor to access User Claims in Services
            services.AddHttpContextAccessor();

            //JSON Settings
            services.Configure<FileUploadSettings>(config.GetSection("FileUpload"));

            //Register Custom Services in IOC Container
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IFileStorageService, LocalFileStorageService>();

            // Add CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AngularApp", policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:4200",
                        "https://ec-frontend-amber.vercel.app",
                        "https://finalproject1.runasp.net"
                        )
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            //Controllers Configuration
            services.AddControllers()
                //Enums Conversion to string 
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    );
                })
                //Configuring default 400 validation error responses and logging to look the same as custom errors.
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(e => e.Value?.Errors.Count > 0)
                            .Select(e => $"{e.Key}: {e.Value?.Errors.First().ErrorMessage}")
                            .ToList();

                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogWarning("400 Validation Error: {Error}", errors);

                        var errorResponse = new ApiErrorResponseDto
                        {
                            StatusCode = 400,
                            Message = "Validation failed",
                            Detail = string.Join("; ", errors),
                            TimeStamp = DateTime.UtcNow
                        };

                        return new BadRequestObjectResult(errorResponse);
                    };
                });

            //RateLimiter
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddSlidingWindowLimiter("standard", options =>
                {
                    options.PermitLimit = 60;
                    options.Window = TimeSpan.FromMinutes(1);
                    options.SegmentsPerWindow = 3;
                    options.QueueLimit = 5;
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ECommerce API",
                    Version = "v1",
                    Description = "API for E-Commerce Backend with JWT Authentication"
                });

                c.UseInlineDefinitionsForEnums();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter 'Bearer' [space] and then your valid token.\r\n\r\nExample: \"Bearer eyJhbGci...\"",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            //OpenApi
            //services.AddOpenApi(options =>
            //{
            //    //Configuring Openapi for swagger ui to work with jwt authentication
            //    options.AddDocumentTransformer((document, context, cancellationToken) =>
            //    {
            //        document.Info = new OpenApiInfo
            //        {
            //            Title = "ECommerce API",
            //            Version = "v1",
            //            Description = "API for E-Commerce Backend with JWT Authentication"
            //        };

            //        document.Components ??= new OpenApiComponents();
            //        document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
            //        {
            //            Type = SecuritySchemeType.Http,
            //            Scheme = "bearer",
            //            BearerFormat = "JWT",
            //            In = ParameterLocation.Header,
            //            Description = "Enter your valid token."
            //        });

            //        return Task.CompletedTask;
            //    });

            //    //Configuring Openapi to add support for enum inputs
            //    options.AddSchemaTransformer((schema, context, cancellationToken) =>
            //    {
            //        if (context.JsonTypeInfo.Type.IsEnum)
            //        {
            //            schema.Type = "string";
            //            schema.Format = null;
            //            schema.Enum = Enum.GetNames(context.JsonTypeInfo.Type)
            //                .Select(name => new OpenApiString(JsonNamingPolicy.CamelCase.ConvertName(name)))
            //                .Cast<IOpenApiAny>()
            //                .ToList();
            //        }
            //        return Task.CompletedTask;
            //    });
            //    //Configuring Openapi to work with jwt authentication and fixing products query param "brands" from int to string like this 1,3,6
            //    options.AddOperationTransformer((operation, context, cancellationToken) =>
            //    {
            //        var metadata = context.Description.ActionDescriptor.EndpointMetadata;

            //        bool hasAuthorize = metadata.Any(m => m is Microsoft.AspNetCore.Authorization.AuthorizeAttribute);
            //        bool hasAnonymous = metadata.Any(m => m is Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute);

            //        if (hasAuthorize && !hasAnonymous)
            //        {
            //            operation.Security = new List<OpenApiSecurityRequirement>
            //            {
            //                new()
            //                {
            //                    {
            //                        new OpenApiSecurityScheme
            //                        {
            //                            Reference = new OpenApiReference
            //                            {
            //                                Type = ReferenceType.SecurityScheme,
            //                                Id = "Bearer"
            //                            }
            //                        },
            //                        Array.Empty<string>()
            //                    }
            //                }
            //            };
            //        }
            //        return Task.CompletedTask;
            //    });
            //});

            return services;
        }
    }
}
