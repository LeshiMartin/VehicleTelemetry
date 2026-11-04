using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Vehicle_Api;

namespace Vehicle_Api_Tests;

public sealed class VehicleApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string TestIssuer = "https://test.local/auth";
    public const string TestAudience = "vehicle-api-tests";
    private const string TestSigningKeyBase64 = "MDEyMzQ1Njc4OWFiY2RlZjAxMjM0NTY3ODlhYmNkZWY=";
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:18").Build();
    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VehicleDbContext>();
        await db.Database.MigrateAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("Jwt:Issuer", TestIssuer);
        builder.UseSetting("Jwt:Audience", TestAudience);
        builder.UseSetting("Jwt:SigningKey", TestSigningKeyBase64);
        builder.UseSetting("Authentication:Schemes:Bearer:SigningKeys:0:Value", TestSigningKeyBase64);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<VehicleDbContext>>();
            services.RemoveAll<VehicleDbContext>();
            services.AddDbContext<VehicleDbContext>(options =>
                options.UseNpgsql(_postgresContainer.GetConnectionString()));
        });


        base.ConfigureWebHost(builder);
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _postgresContainer.DisposeAsync();
    }
}
