using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

public class SecuritySchemeDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken
    )
    {
        document.Components ??= new OpenApiComponents();

        // Initialize with the correct interface type
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                                                       "Enter 'Bearer' [space] and your token in the text input below.\n\nExample: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
            },
            ["ApiKey"] = new OpenApiSecurityScheme
            {
                Name = "X-API-KEY",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Description =
                                                       "Enter your API key in the text input below.\n\nExample: '12345-abcdef'",
            },
        };

        foreach (var (key, scheme) in securitySchemes)
        {
            if (!document.Components.SecuritySchemes.ContainsKey(key))
            {
                document.Components.SecuritySchemes.Add(key, scheme);
            }
        }

        return Task.CompletedTask;
    }
}