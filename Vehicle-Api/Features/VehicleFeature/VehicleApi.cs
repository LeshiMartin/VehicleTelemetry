using Common;
using Common.Sorting;
using Microsoft.EntityFrameworkCore;
using Vehicle_Api.Middleware;

namespace Vehicle_Api.Features.VehicleFeature;

public class VehicleApi(ILogger<VehicleApi> logger) : IRouteRegister
{
    public IEndpointRouteBuilder RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("Vehicle")
            .WithTags("Vehicle").WithDisplayName("Vehicle")
            .WithDescription("Endpoint for vehicles");

        group.MapGet("", async (VehicleDbContext vehicleDbContext, [AsParameters] VehiclePageRequestDto pageRequestDto, CancellationToken cancellationToken) =>
        {
            logger.LogTrace("Querying Vehicles request:{request}", pageRequestDto);
            var vehiclesQuery = vehicleDbContext.Vehicles.AsNoTracking();
            var vehicles = vehiclesQuery.Where(x => x.Id > pageRequestDto.LastId).Take(pageRequestDto.Count);
            var totalCount = vehiclesQuery.Count();
            var list = pageRequestDto.GetSorter(VehicleSortingExpressions)
                .ApplySort(vehicles)
                .Select(x => new
                {
                    x.Id,
                    x.CreatedAt,
                    x.UpdatedAt,
                    x.Name,
                    x.Vin,
                    x.Make,
                    x.Model,
                    x.ModelYear.Year,
                    VehicleState = x.VehicleState == null ? "N/A" : x.VehicleState.Name,
                    x.VehicleEngineType
                })
                .AsAsyncEnumerable();

            var lastId = 0;
            var results = new List<VehicleDto>(pageRequestDto.Count);
            await foreach (var data in list.WithCancellation(cancellationToken))
            {
                results.Add(new VehicleDto(
                    data.Id, data.CreatedAt, data.UpdatedAt, data.Name, data.Vin, data.Make, data.Model, data.Year,
                    data.VehicleState, Enum.GetName(typeof(VehicleEngineType), data.VehicleEngineType)??""));
                lastId = data.Id;
            }

            return Results.Ok(new PageResponseDto<VehicleDto>(results.ToList(), lastId, totalCount));
        }).RequireAuthorization(p=>p.RequireRole("Admin" ,"FleetManager"));

        group.MapGet("{id:int}", async (int id, VehicleDbContext vehicleDbContext, CancellationToken cancellationToken) =>
        {

            logger.LogTrace("Retrieving vehicle with id:{id}", id);
            var vehicle = await vehicleDbContext.Vehicles
                .Select(x => new
                {
                    x.Id,
                    x.CreatedAt,
                    x.UpdatedAt,
                    x.Name,
                    x.Vin,
                    x.Make,
                    x.Model,
                    x.ModelYear.Year,
                    VehicleState = x.VehicleState == null ? "N/A" : x.VehicleState.Name,
                    x.VehicleEngineType
                })
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return vehicle is not null ? Results.Ok(new VehicleDto(
                vehicle!.Id,
                vehicle.CreatedAt,
                vehicle.UpdatedAt,
                vehicle.Name, vehicle.Vin,
                vehicle.Make, vehicle.Model,
                vehicle.Year,
                vehicle.VehicleState,
                Enum.GetName(vehicle.VehicleEngineType)??"")) : Results.NotFound();
        }).WithName("SingleVehicle").RequireAuthorization(p => p.RequireRole("Admin", "FleetManager"));

        group.MapPost("", async (VehicleDbContext vehicleDbContext, CreateVehicleDto createVehicleDto, CancellationToken cancellationToken) =>
        {
            logger.LogTrace("Creating vehicle request:{request}", createVehicleDto);
            var state = await vehicleDbContext.VehicleStates.FindAsync([createVehicleDto.VehicleState],
                cancellationToken);
            if (state is null)
                return Results.BadRequest(new {ErrorMessage = $"State matches:{createVehicleDto.VehicleState}"});

            var vehicle = new Vehicle()
            {
                Name = createVehicleDto.Name,
                VehicleStateId = createVehicleDto.VehicleState,
                Make = createVehicleDto.Make,
                Model = createVehicleDto.Model,
                ModelYear = createVehicleDto.Year,
                VehicleEngineType = (VehicleEngineType)createVehicleDto.VehicleEngineType,
                Vin = createVehicleDto.Vin,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };
            await vehicleDbContext.AddAsync(vehicle, cancellationToken);
            await vehicleDbContext.SaveChangesAsync(cancellationToken);
            logger.LogTrace("Successfully created a vehicle:{vehicle}", vehicle);
            return Results.CreatedAtRoute("SingleVehicle", new { id = vehicle.Id }, vehicle.Id);

        }).RequireAuthorization(p => p.RequireRole("Admin", "FleetManager"));

        return endpointRouteBuilder;
    }


}
