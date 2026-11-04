using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Vehicle_Api;
using Vehicle_Api.Features.VehicleFeature;

namespace Vehicle_Api_Tests.Features;

[Trait("Category", "Integration")]
public class VehicleApiTests(VehicleApiFactory vehicleApiFactory) : IClassFixture<VehicleApiFactory>
{

    #region Get Vehicles

    [Fact]
    public async Task Get_Vehicles_With_No_LastId_Or_Count_Specified_Should_Return_200()
    {
        var client = vehicleApiFactory.CreateClientWithToken(new Claim(ClaimTypes.Role, "Admin"));
        var response = await client.GetAsync("Vehicle");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Vehicle_With_No_AuthenticationToken_Should_Return_401()
    {
        var client = vehicleApiFactory.CreateClient();
        var response = await client.GetAsync("Vehicle");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_Vehicle_With_AuthenticationToken_With_Not_Approved_Role_Should_Return_403()
    {
        var client = vehicleApiFactory.CreateClientWithToken(new Claim(ClaimTypes.Role, "Test"));
        var response = await client.GetAsync("Vehicle");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region Get Single Vehicle

    [Fact]
    public async Task Get_Vehicle_By_Id_When_None_Present_Should_Return_404()
    {
        var client = vehicleApiFactory.CreateClientWithToken(new Claim(ClaimTypes.Role, "Admin"));
        var response = await client.GetAsync("Vehicle/100");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    [Fact]
    public async Task Get_Vehicle_By_Id_When_Present_Should_Return_200()
    {
        var client = vehicleApiFactory.CreateClientWithToken(new Claim(ClaimTypes.Role, "Admin"));
        await using var scope = vehicleApiFactory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VehicleDbContext>();
        var vehicle = new Vehicle()
        {
            CreatedAt = DateTimeOffset.UtcNow,
            Name = "TestVehicle",
            ModelYear = new ModelYear(2025),
            Make = "TestMake",
            Model = "TestModel",
            VehicleEngineType = VehicleEngineType.Electric,
            Vin = "TestVin",
            UpdatedAt = DateTimeOffset.UtcNow,
            VehicleStateId = 1
        };
        await dbContext.Vehicles.AddAsync(vehicle);
        await dbContext.SaveChangesAsync();

        var response = await client.GetAsync($"Vehicle/{vehicle.Id}");
        var dto = await response.Content.ReadFromJsonAsync<VehicleDto>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(dto);
        Assert.Equal(vehicle.Make,dto.Make);
        Assert.Equal(vehicle.Model,dto.Model);
        Assert.Equal(vehicle.Vin,dto.Vin);
    }


    [Fact]
    public async Task Get_Vehicle_By_Id_With_No_AuthenticationToken_Should_Return_401()
    {
        var client = vehicleApiFactory.CreateClient();
        var response = await client.GetAsync("Vehicle/100");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_Vehicle_By_Id_With_AuthenticationToken_With_Not_Approved_Role_Should_Return_403()
    {
        var client = vehicleApiFactory.CreateClientWithToken(new Claim(ClaimTypes.Role, "Test"));
        var response = await client.GetAsync("Vehicle/100");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }


    #endregion


    [Theory]
    [ClassData(typeof(CreateRequestDtoProvider))]
    public async Task Creating_A_Vehicle_With_Missing_Values_Should_Return_400(CreateVehicleDto dto)
    {
        var client = vehicleApiFactory.CreateClientWithToken(new Claim(ClaimTypes.Role, "Admin"));
        var content = JsonContent.Create(dto);
        var result = await client.PostAsync("Vehicle", content);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    private class CreateRequestDtoProvider : TheoryData<CreateVehicleDto>
    {
        public CreateRequestDtoProvider()
        {
            Add(new CreateVehicleDto("", "1", "2", "3", 2300, 1, 1));
            Add(new CreateVehicleDto("1", "", "2", "3", 2300, 1, 1));
            Add(new CreateVehicleDto("1", "2", "", "3", 2300, 1, 1));
            Add(new CreateVehicleDto("1", "2", "3", "", 2300, 1, 1));
            Add(new CreateVehicleDto("1", "2", "3", "4", 3300, 1, 1));
            Add(new CreateVehicleDto("1", "2", "3", "4", 2025, 10, 1));
            Add(new CreateVehicleDto(string.Create(102, "", (x, i) => { }), "2", "3", "4", 2025, 1, 1));
        }
    }

}
