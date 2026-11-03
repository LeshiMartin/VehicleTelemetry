using Common;

namespace Vehicle_Api.Features.VehicleFeature;

public record VehiclePageRequestDto(int LastId = 0, int Count = 10, string? Sorts = null) : PageRequestDto(LastId, Count, Sorts);
