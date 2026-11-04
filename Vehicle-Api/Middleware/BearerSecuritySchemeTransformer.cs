using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Vehicle_Api.Middleware;

public sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authScheme = await authenticationSchemeProvider.GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme);
        if (authScheme is null)
            return;

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Name = "Jwt Bearer",
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "Json Web Token",
            In = ParameterLocation.Header,
            Description = "Add the string representation of the jwt without the token"
        };

    }
}