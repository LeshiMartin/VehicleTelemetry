using Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json;
using Vehicle_Api;
using Vehicle_Api.Features.SignalThresholdFeature;
using Vehicle_Api.Features.VehicleFeature;
using Vehicle_Api.Features.VehicleSignalFeature;
using Vehicle_Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(opt =>
{
    opt.AddOperationTransformer<BearerSecurityOperationSchemeTransformer>();
    opt.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
builder.Services.AddDbContext<VehicleDbContext>(x =>
        x.UseNpgsql(
            Environment.GetEnvironmentVariable("ConnectionStrings__Default")??
            builder.Configuration.GetConnectionString("Default") ??
            "Host=design-time-placeholder;Database=design-time-placeholder;Username=design-time-placeholder;Password=design-time-placeholder"
        ));

builder.Services.AddHealthChecks()
    .AddCheck("self",() => HealthCheckResult.Healthy(), tags: ["live"])
    .AddDbContextCheck<VehicleDbContext>();

var config = builder.Configuration;
var authenticationData = GetAuthenticationData(config);
builder.Services.AddOptions<AuthenticationData>().Configure(x =>
{
    x.Issuer = authenticationData.Issuer;
    x.Audience = authenticationData.Audience;
    x.SigningKey = authenticationData.SigningKey;
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = authenticationData.Issuer,

            ValidateAudience = true,
            ValidAudience = authenticationData.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(authenticationData.SigningKey),
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddValidation();
builder.Services.AddScoped<IRouteRegister, VehicleApi>();
builder.Services.AddScoped<IRouteRegister, VehicleSignalApi>();
builder.Services.AddScoped<IRouteRegister, SignalThresholdApi>();
builder.Services.AddScoped<LoggingMiddleware>();
builder.Services.AddExceptionHandler<BadHttpRequestExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
var app = builder.Build();

app.UseExceptionHandler();
app.UseMiddleware<LoggingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var routeRegisters = serviceProvider.GetServices<IRouteRegister>();
    foreach (var routeRegister in routeRegisters)
        routeRegister.RegisterRoutes(app);
}
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
}).ExcludeFromDescription();

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = HealthCheckResponseWriter.WriteJson
}).ExcludeFromDescription();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.MapOpenApi();
    app.MapScalarApiReference(x=>
    {
        x.Theme = ScalarTheme.Kepler;
        x.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.RestSharp);
    });
}

app.Run();


AuthenticationData GetAuthenticationData(IConfiguration configuration)
{
    var (issuer, audience) = (configuration["Jwt:Issuer"], configuration["Jwt:Audience"]);
    var authenticationSchemeKey = configuration["Authentication:Schemes:Bearer:SigningKeys:0:Value"];
    var jwtKey = configuration["Jwt:SigningKey"];
    var isInContainer = Environment.GetEnvironmentVariable("IN_CONTAINER") == "true";
    if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience) ||
        (string.IsNullOrEmpty(authenticationSchemeKey) && string.IsNullOrEmpty(jwtKey)))
        throw new InvalidOperationException("The jwt authentication data could not be parse");

    var signingKey =
        authenticationSchemeKey is not null
            ? Convert.FromBase64String(authenticationSchemeKey)
            : isInContainer 
                ? Convert.FromBase64String(jwtKey!) 
                : Encoding.UTF8.GetBytes(jwtKey!);
    return new AuthenticationData()
    {
        Audience = audience,
        Issuer = issuer,
        SigningKey = signingKey
    };
}

public static class HealthCheckResponseWriter
{
    public static Task WriteJson(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration,
                exception = entry.Value.Exception?.Message,
                data = entry.Value.Data.Count == 0
                    ? null
                    : entry.Value.Data
            })
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
    }
}