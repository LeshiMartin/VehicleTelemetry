using System.ComponentModel.DataAnnotations;

namespace Vehicle_Api.Features.VehicleFeature;

public record VehicleDto(int Id,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string Name,
    string Vin,
    string Make,
    string Model, int Year, string VehicleState, string VehicleEngineType);

public record CreateVehicleDto(
    [Required, StringLength(100)] string Name,
    [Required, StringLength(100)] string Vin,
    [Required, StringLength(100)] string Make,
    [Required, StringLength(100)] string Model,
    [Required, Range(1900, 2300)] int Year,
    [Required, Range(1, 50)] int VehicleState,
    [Required] int VehicleEngineType
);


public record VehicleStateDto(int Id, string Name);

public record CreateVehicleStatesDto(string Name);

public record VehicleEngineTypeDto(int Value, string ValueName);

