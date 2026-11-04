using System.Linq.Expressions;
using Common.Sorting;
using Vehicle_Api.Features.SignalThresholdFeature;
using Vehicle_Api.Features.VehicleFeature;
using Vehicle_Api.Features.VehicleSignalFeature;

namespace Vehicle_Api.Middleware;

public static class SortingExpressions
{
    public static readonly SortingExpression<SignalThreshold> SignalThresholdSortingExpression = new()
    {
        [nameof(SignalThreshold.Id)] = x => x.Id,
        [nameof(SignalThreshold.ErrorMax)] = x => x.ErrorMax!,
        [nameof(SignalThreshold.WarningMax)] = x => x.WarningMax!,
        [nameof(SignalThreshold.ErrorMin)] = x => x.ErrorMin!,
        [nameof(SignalThreshold.CreatedAt)] = x => x.CreatedAt,
        [nameof(SignalThreshold.UpdatedAt)] = x => x.UpdatedAt,
    };

    public static readonly SortingExpression<VehicleSignal> VehicleSignalSortingExpressions = new()
    {
        [nameof(VehicleSignal.Id)] = x => x.Id,
        [nameof(VehicleSignal.Name)] = x => x.Name,
        [nameof(VehicleSignal.DisplayName)] = x => x.DisplayName,
        [nameof(VehicleSignal.MaxValue)] = x => x.MaxValue!,
        [nameof(VehicleSignal.MinValue)] = x => x.MinValue!,
        [nameof(VehicleSignal.CreatedAt)] = x => x.CreatedAt,
        [nameof(VehicleSignal.UpdatedAt)] = x => x.UpdatedAt,
    };

    public static readonly SortingExpression<Vehicle> VehicleSortingExpressions = new()
    {
        [nameof(Vehicle.Id)] = v => v.Id,
        [nameof(CreateVehicleDto.Year)] = v => v.ModelYear.Year,
        [nameof(CreateVehicleDto.Name)] = x => x.Name,
        [nameof(CreateVehicleDto.Vin)] = x => x.Vin,
        [nameof(CreateVehicleDto.Make)] = x => x.Make,
        [nameof(CreateVehicleDto.Model)] = x => x.Model,
        [nameof(CreateVehicleDto.VehicleState)] = x => x.VehicleStateId,
        [nameof(Vehicle.CreatedAt)] = x => x.CreatedAt,
        [nameof(Vehicle.UpdatedAt)] = x => x.UpdatedAt,
    };
}
