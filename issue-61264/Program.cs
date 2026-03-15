using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder();

builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "Hello world!");

app.MapGet("/public", () => "This is a public endpoint.").AllowAnonymous();

app.Run();

internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // "bearer" refers to the header name here
                    In = ParameterLocation.Header,
                    BearerFormat = "Json Web Token"
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = securitySchemes;

            var schemeRef = new OpenApiSecuritySchemeReference("Bearer", document);

            // Collect relative paths of endpoints that allow anonymous access
            var anonymousPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var group in context.DescriptionGroups)
            {
                foreach (var apiDesc in group.Items)
                {
                    if (apiDesc.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any())
                    {
                        anonymousPaths.Add("/" + apiDesc.RelativePath);
                    }
                }
            }

            foreach (var (path, pathItem) in document.Paths)
            {
                if (anonymousPaths.Contains(path))
                {
                    continue;
                }

                foreach (var (_, operation) in pathItem.Operations!)
                {
                    operation.Security ??= new List<OpenApiSecurityRequirement>();
                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        [schemeRef] = new List<string>()
                    });
                }
            }
        }
    }
}
