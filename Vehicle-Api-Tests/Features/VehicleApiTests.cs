using System.Net;
using System.Net.Http.Json;
using Vehicle_Api.Features.VehicleFeature;

namespace Vehicle_Api_Tests.Features;

[Trait("Category","Integration")]
public class VehicleApiTests(VehicleApiFactory vehicleApiFactory) : IClassFixture<VehicleApiFactory>
{
    [Fact]
    public async Task Get_Vehicles_With_No_LastId_Or_Count_Specified_Should_Return_200()
    {
        var client = vehicleApiFactory.CreateClient();
        var response = await client.GetAsync("Vehicle");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Vehicle_By_Id_When_None_Present_Should_Return_404()
    {
        var client = vehicleApiFactory.CreateClient();
        var response = await client.GetAsync("Vehicle/100");
        Assert.Equal(HttpStatusCode.NotFound,response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(CreateRequestDtoProvider))]
    public async Task Creating_A_Vehicle_With_Missing_Values_Should_Return_400(CreateVehicleDto dto)
    {
        var client = vehicleApiFactory.CreateClient();
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
            Add(new CreateVehicleDto(string.Create(102,"", (x,i) => { }), "2", "3", "4", 2025, 1, 1));
        }
    }

}
