using System.ComponentModel.DataAnnotations;

namespace Vehicle_Api.Features.SignalThresholdFeature;

public record SignalThresholdDto(
    int Id,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    double? WarningMax,
    double? ErrorMin, double? ErrorMax);


public record CreateThresholdDto(
    [Required, Range(double.MinValue, double.MaxValue)] double ErrorMax,
    [Range(double.MinValue, double.MaxValue)] double? WarningMax,
    [Range(double.MinValue, double.MaxValue)] double? ErrorMin);


public static class Extensions
{
    extension(SignalThreshold entity)
    {
        public SignalThresholdDto ToDto() =>
            new(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.WarningMax,
                entity.ErrorMin,
                entity.ErrorMax
            );
    }
}