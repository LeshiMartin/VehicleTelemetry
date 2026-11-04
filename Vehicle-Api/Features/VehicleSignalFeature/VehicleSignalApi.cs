using Common;
using Common.Sorting;
using Microsoft.EntityFrameworkCore;
using Vehicle_Api.Middleware;

namespace Vehicle_Api.Features.VehicleSignalFeature;

public class VehicleSignalApi(ILogger<VehicleSignalApi> logger) : IRouteRegister
{
    public IEndpointRouteBuilder RegisterRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("Vehicle/{vehicleId:int}/Signal").WithTags("Signal")
            .WithDisplayName("Signals").WithDescription("Endpoint for vehicle signals");

        group.MapGet("", async (int vehicleId, [AsParameters] VehicleSignalRequestDto pageRequestDto, VehicleDbContext vehicleDbContext, CancellationToken cancellationToken) =>
        {
            logger.LogTrace("Querying signals for vehicle with id:{id} request:{request}", vehicleId, pageRequestDto);
            var query = vehicleDbContext.VehicleSignals
                .AsNoTracking()
                .Where(x => x.VehicleId == vehicleId);
            
            var totalCount = await query.CountAsync(cancellationToken);
            var signalsAsync =
                pageRequestDto
                    .GetSorter(VehicleSignalSortingExpressions)
                    .ApplySort(query)
                    .Where(x => x.Id > pageRequestDto.LastId).AsAsyncEnumerable();
            var results = new List<VehicleSignalDto>(pageRequestDto.Count);
            var lastId = 0;
            await foreach (var signal in signalsAsync.WithCancellation(cancellationToken))
            {
                results.Add(new VehicleSignalDto(signal.Id, signal.Name, signal.DisplayName, signal.Unit.Name, signal.Unit.Symbol??"", signal.CreatedAt, signal.UpdatedAt, signal.MinValue, signal.MaxValue));
                lastId = signal.Id;
            }

            return Results.Ok(new PageResponseDto<VehicleSignalDto>(results, lastId, totalCount));
        });
        group.MapGet("{id:int}", async (int vehicleId, int id, VehicleDbContext vehicleDbContext, CancellationToken cancellationToken) =>
        {
            logger.LogTrace("Retrieving the signal with id :{id}", id);
            var signal = await vehicleDbContext.VehicleSignals.FirstOrDefaultAsync(x => x.VehicleId == vehicleId && x.Id == id, cancellationToken);
            return signal
                ? Results.Ok(new VehicleSignalDto(signal!.Id, signal.Name, signal.DisplayName, signal.Unit.Name,
                    signal.Unit.Symbol ?? "", signal.CreatedAt, signal.UpdatedAt, signal.MinValue, signal.MaxValue))
                : Results.NotFound();


        }).WithName("GetSignal");
        group.MapPost("", async (int vehicleId, CreateVehicleSignalDto dto, VehicleDbContext vehicleDbContext, CancellationToken cancellationToken) =>
        {

            logger.LogTrace("Creating a new signal dto:{dto}", dto);
            var vehicle = await vehicleDbContext.Vehicles.FindAsync([vehicleId], cancellationToken);
            if (vehicle is null)
                return Results.NotFound();

            var signal = new VehicleSignal
            {
                Name = dto.Name,
                DisplayName = dto.DisplayName,
                Unit = new Unit(dto.Unit, dto.UnitSymbol),
                VehicleId = vehicleId,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                MaxValue = dto.MaxValue,
                MinValue = dto.MinValue
            };
            await vehicleDbContext.VehicleSignals.AddAsync(signal, cancellationToken);
            await vehicleDbContext.SaveChangesAsync(cancellationToken);
            return Results.CreatedAtRoute("GetSignal",
                new { vehicleId, id = signal.Id }, signal.Id);

        });

        return routeBuilder;
    }
}
