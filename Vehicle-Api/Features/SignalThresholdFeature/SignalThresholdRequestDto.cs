using Common;

namespace Vehicle_Api.Features.SignalThresholdFeature;

public record SignalThresholdRequestDto(int LastId, int Count, string? Sorts) : PageRequestDto(LastId, Count, Sorts);
