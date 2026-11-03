using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vehicle_Api.Features.SignalThresholdFeature;
using Vehicle_Api.Features.VehicleFeature;

namespace Vehicle_Api.Features.VehicleSignalFeature;

public class VehicleSignal : BaseEntity
{
    private readonly List<SignalThreshold> _signalThresholds = [];
    public required int VehicleId { get; init; }
    public Vehicle? Vehicle { get; init; }
    
    [MaxLength(100)]
    public required string Name { get; init; }
    
    [MaxLength(100)]
    public required string DisplayName { get; init; }
    public required Unit Unit { get; init; }
    public double? MinValue { get; init; }
    public double? MaxValue { get; init; }
    public bool IsEnabled { get; init; } = true;
    public IReadOnlyList<SignalThreshold> SignalThresholds => _signalThresholds.ToImmutableList();
    public void AddSignalThreshold(SignalThreshold threshold) => _signalThresholds.Add(threshold);
}

public readonly record struct Unit(string Name, string? Symbol);

public class VehicleSignalMapper : BaseEntityMapper<VehicleSignal>
{
    protected override void ConfigureAbstract(EntityTypeBuilder<VehicleSignal> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.DisplayName).IsRequired().HasMaxLength(100);
        builder.ComplexProperty(x => x.Unit, opt =>
        {
            opt.Property(c => c.Name).HasColumnName("UnitName").IsRequired().HasMaxLength(50);
            opt.Property(c => c.Symbol).HasColumnName("UnitSymbol").HasMaxLength(10);
        });
        builder.HasMany(x => x.SignalThresholds).WithOne(x => x.VehicleSignal).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Vehicle).WithMany(x => x.Signals).HasForeignKey(x => x.VehicleId);
    }
}
