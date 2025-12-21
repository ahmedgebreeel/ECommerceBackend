using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ECommerce.API.Infrastructure
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // 1. Check metadata for [Authorize] attribute
            var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
            bool hasAuthorize = metadata.Any(m => m is AuthorizeAttribute);
            bool hasAnonymous = metadata.Any(m => m is AllowAnonymousAttribute);

            // 2. Only add the lock if Authorized AND not Anonymous
            if (hasAuthorize && !hasAnonymous)
            {
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
                };
            }
        }
    }
}