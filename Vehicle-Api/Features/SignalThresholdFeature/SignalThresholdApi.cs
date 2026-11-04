using Common;
using Common.Sorting;
using Microsoft.EntityFrameworkCore;
using Vehicle_Api.Middleware;

namespace Vehicle_Api.Features.SignalThresholdFeature;

public class SignalThresholdApi(ILogger<SignalThresholdApi> logger) : IRouteRegister
{
    public IEndpointRouteBuilder RegisterRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder
            .MapGroup("Vehicle/{vehicleId:int}/Signal/{signalId:int}/SignalThreshold")
            .WithTags("SignalThreshold")
            .WithDisplayName("Signal Thresholds")
            .WithDescription("Endpoint for SignalThresholds");

        group.MapGet("", async (int vehicleId, int signalId, [AsParameters] SignalThresholdRequestDto pageRequestDto, VehicleDbContext vehicleDbContext, CancellationToken cancellationToken) =>
        {

            logger.LogTrace("Querying the SignalThresholds for the vehicle:{vehicleId} for signal:{signalId}, request:{request}", vehicleId, signalId, pageRequestDto);
            var query = vehicleDbContext.SignalThresholds.AsNoTracking().Where(x => x.VehicleSignalId == signalId);
            var totalCount = await query.CountAsync(cancellationToken);

            var thresholdsAsync = pageRequestDto
                .GetSorter(SignalThresholdSortingExpression)
                .ApplySort(query)
                .Where(x => x.VehicleSignalId == signalId && x.Id > pageRequestDto.LastId)
                .Take(pageRequestDto.Count)
                .AsAsyncEnumerable();

            var result = new List<SignalThresholdDto>(pageRequestDto.Count);
            var lastId = 0;
            await foreach (var threshold in thresholdsAsync.WithCancellation(cancellationToken))
            {
                result.Add(new SignalThresholdDto(threshold.Id, threshold.CreatedAt, threshold.UpdatedAt, threshold.WarningMax, threshold.ErrorMin, threshold.ErrorMax));
                lastId = threshold.Id;
            }
            return Results.Ok(new PageResponseDto<SignalThresholdDto>(result, lastId, totalCount));

        });

        group.MapGet("{id:int}", async (int vehicleId, int signalId, int id, VehicleDbContext vehicleDbContext, CancellationToken cancellationToken) =>
        {

            logger.LogTrace("Getting the threshold vehicleId:{vehicleId} signalId:{signalId}, id:{id}", vehicleId, signalId, id);
            var threshold = await vehicleDbContext.SignalThresholds.FirstOrDefaultAsync(x => x.VehicleSignalId == signalId && x.Id == id, cancellationToken);
            return !threshold ? Results.NotFound() : Results.Ok(threshold!.ToDto());

        }).WithName("GetSignalThreshold");

        group.MapPost("", async (int vehicleId, int signalId, CreateThresholdDto dto, VehicleDbContext vehicleDbContext, CancellationToken cancellationToken) =>
        {
            var vehicle = await vehicleDbContext.Vehicles.FindAsync([vehicleId], cancellationToken);
            if (vehicle is null)
                return Results.NotFound();
            var signal = await vehicleDbContext.VehicleSignals.FindAsync([signalId], cancellationToken);
            if (signal is null)
                return Results.NotFound();

            var entity = new SignalThreshold
            {
                VehicleSignalId = signalId,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                ErrorMax = dto.ErrorMax,
                ErrorMin = dto.ErrorMin,
                WarningMax = dto.WarningMax
            };
            await vehicleDbContext.AddAsync(entity, cancellationToken);
            await vehicleDbContext.SaveChangesAsync(cancellationToken);
            return Results.CreatedAtRoute("GetSignalThreshold",
                new { vehicleId, signalId, id = entity.Id }, entity.Id);

        });

        return routeBuilder;
    }
}
