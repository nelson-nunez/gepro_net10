using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace GePro.API.Security
{
    /// <summary>Declara el esquema Bearer JWT en el documento OpenAPI para que Swagger UI muestre el botón Authorize.</summary>
    public class BearerSecurityTransformer : IOpenApiDocumentTransformer
    {
        private const string SchemeName = "Bearer";

        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes[SchemeName] = CreateBearerScheme();

            var requirement = new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(SchemeName, document)] = []
            };
            document.Security ??= [];
            document.Security.Add(requirement);

            return Task.CompletedTask;
        }

        private static OpenApiSecurityScheme CreateBearerScheme()
        {
            return new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Pegar solo el token devuelto por /authenticate, sin el prefijo Bearer."
            };
        }
    }
}
