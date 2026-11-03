using Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vehicle_Api.Features.VehicleSignalFeature;

namespace Vehicle_Api.Features.SignalThresholdFeature;

public class SignalThreshold: BaseEntity
{
    public required int VehicleSignalId { get; init; }
    public VehicleSignal? VehicleSignal { get; init; }
    public double? WarningMax { get; init; }
    public double? ErrorMin { get; init; }
    public double? ErrorMax { get; init; }
    public bool IsEnabled { get; init; } = true;
}

public class SignalThresholdMapper : BaseEntityMapper<SignalThreshold>
{
    protected override void ConfigureAbstract(EntityTypeBuilder<SignalThreshold> builder)
    {
        builder.HasOne(x => x.VehicleSignal)
            .WithMany(x => x.SignalThresholds)
            .HasForeignKey(x => x.VehicleSignalId);
    }
}
