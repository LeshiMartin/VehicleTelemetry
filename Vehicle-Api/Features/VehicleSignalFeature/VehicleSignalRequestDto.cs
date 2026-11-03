using Common;

namespace Vehicle_Api.Features.VehicleSignalFeature;

public record VehicleSignalRequestDto(int LastId, int Count, string? Sorts) : PageRequestDto(LastId, Count, Sorts);