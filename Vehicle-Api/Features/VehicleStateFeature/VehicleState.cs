using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vehicle_Api.Features.VehicleFeature;

namespace Vehicle_Api.Features.VehicleStateFeature;

public class VehicleState : BaseEntity
{
    private readonly List<Vehicle> _vehicles = [];
    [MaxLength(100)]
    public required string Name { get; init; }
    public IReadOnlyList<Vehicle> Vehicles => _vehicles.ToImmutableList();
}

public class VehicleStateMapper : BaseEntityMapper<VehicleState>
{
    protected override void ConfigureAbstract(EntityTypeBuilder<VehicleState> builder)
    {
        builder.HasData(
            [
                new VehicleState
                {
                    Id = 1,
                    Name = "Active",
                    CreatedAt = DateTimeOffset.Parse("2026-10-25T00:00:00Z"),
                    UpdatedAt = DateTimeOffset.Parse("2026-10-25T00:00:00Z")
                },
                new VehicleState
                {
                    Id=2,
                    Name = "InActive",
                    CreatedAt = DateTimeOffset.Parse("2026-10-25T00:00:00Z"),
                    UpdatedAt = DateTimeOffset.Parse("2026-10-25T00:00:00Z")
                },
                new VehicleState
                {
                    Id = 3,
                    Name = "Archived",
                    CreatedAt = DateTimeOffset.Parse("2026-10-25T00:00:00Z"),
                    UpdatedAt = DateTimeOffset.Parse("2026-10-25T00:00:00Z")
                }
            ]

        );
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasMany(x => x.Vehicles).WithOne(x => x.VehicleState).HasForeignKey(x => x.VehicleStateId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}