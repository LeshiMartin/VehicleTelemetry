using Microsoft.EntityFrameworkCore;
using Vehicle_Api;
using Vehicle_Api.Features.SignalThresholdFeature;
using Vehicle_Api.Features.VehicleFeature;
using Vehicle_Api.Features.VehicleSignalFeature;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<VehicleDbContext>(x => x.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
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
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var routeRegisters = serviceProvider.GetServices<IRouteRegister>();
    foreach (var routeRegister in routeRegisters)
        routeRegister.RegisterRoutes(app);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.Run();
