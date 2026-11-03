using System.ComponentModel.DataAnnotations;

namespace Vehicle_Api.Features.VehicleSignalFeature;

public record VehicleSignalDto(
    int Id,
    string Name,
    string DisplayName,
    string Unit,
    string UnitSymbol,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    double? MinValue,
    double? MaxValue
    );

public record CreateVehicleSignalDto(
   [Required, StringLength(100)] string Name,
   [Required, StringLength(100)] string DisplayName,
   [Required, StringLength(30)] string Unit,
    [StringLength(20)] string? UnitSymbol,
    [Range(double.MinValue, double.MaxValue)] double? MinValue,
    [Range(double.MinValue, double.MaxValue)] double? MaxValue);