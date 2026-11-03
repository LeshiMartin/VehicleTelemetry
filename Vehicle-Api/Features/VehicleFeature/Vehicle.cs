using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Common;
using Microsoft.EntityFrameworkCore;
using Vehicle_Api.Features.VehicleSignalFeature;
using Vehicle_Api.Features.VehicleStateFeature;

namespace Vehicle_Api.Features.VehicleFeature;

public class Vehicle : BaseEntity
{
    public int VehicleStateId { get; init; }
    private readonly List<VehicleSignal> _signals = [];
    [MaxLength(100)]
    public required string Vin { get; init; }
    [MaxLength(100)]
    public required string Name { get; init; }
    [MaxLength(100)]
    public required string Make { get; init; }
    [MaxLength(100)]
    public required string Model { get; init; }
    public required ModelYear ModelYear { get; init; }
    public VehicleState? VehicleState { get; init; }
    public required VehicleEngineType VehicleEngineType { get; init; }

    public IReadOnlyList<VehicleSignal> Signals => _signals.ToImmutableList();
    public void AddSignal(VehicleSignal signal) => _signals.Add(signal);
}


public class VehicleMapper : BaseEntityMapper<Vehicle>
{
    protected override void ConfigureAbstract(EntityTypeBuilder<Vehicle> builder)
    {
        builder.Property(x => x.Vin).IsRequired().HasMaxLength(35);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Make).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Model).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ModelYear).HasConversion(x => x.Year, x => x);

        builder.HasOne(x => x.VehicleState).WithMany(x => x.Vehicles).HasForeignKey(x => x.VehicleStateId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(x => x.Signals).WithOne(x => x.Vehicle).HasForeignKey(x => x.VehicleId).OnDelete(DeleteBehavior.Cascade);
    }
}