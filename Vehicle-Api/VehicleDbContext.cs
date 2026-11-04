using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Vehicle_Api.Features.SignalThresholdFeature;
using Vehicle_Api.Features.VehicleFeature;
using Vehicle_Api.Features.VehicleSignalFeature;
using Vehicle_Api.Features.VehicleStateFeature;

namespace Vehicle_Api;

public class VehicleDbContext(DbContextOptions<VehicleDbContext> options) : DbContext(options)
{
    public DbSet<Vehicle> Vehicles { get; init; }
    public DbSet<VehicleSignal> VehicleSignals { get; init; }
    public DbSet<SignalThreshold> SignalThresholds { get; init; }
    public DbSet<VehicleState> VehicleStates { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VehicleDbContext).Assembly);

    }
}


public sealed class VehicleDbContextFactory
    : IDesignTimeDbContextFactory<VehicleDbContext>
{
    public VehicleDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Vehicle-Api/appsettings.json", optional: true)
                .AddJsonFile(
                    $"Vehicle-Api/appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json",
                    optional: true)
                .AddEnvironmentVariables()
                .Build()
                .GetConnectionString("Default")
            ?? "Host=design-time-placeholder;Database=design-time-placeholder;Username=design-time-placeholder;Password=design-time-placeholder";

        return new VehicleDbContext(
            new DbContextOptionsBuilder<VehicleDbContext>()
                .UseNpgsql(connectionString)
                .Options);
    }
}