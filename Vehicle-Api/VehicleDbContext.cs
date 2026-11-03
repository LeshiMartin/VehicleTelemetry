using Microsoft.EntityFrameworkCore;
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
