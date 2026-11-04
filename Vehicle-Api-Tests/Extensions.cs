using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Vehicle_Api_Tests;

internal static class Extensions
{
    extension(VehicleApiFactory vehicleApiFactory)
    {
        public HttpClient CreateClientWithToken(params Span<Claim> claims)
        {
            var authenticationOptions = vehicleApiFactory.Services.GetService<IOptions<AuthenticationData>>();
            return authenticationOptions is null ? vehicleApiFactory.CreateClient() : vehicleApiFactory.CreateClient(authenticationOptions.Value, claims);
        }

        private HttpClient CreateClient(AuthenticationData authenticationData, params Span<Claim> claims)
        {

            var credentials = new SigningCredentials(new SymmetricSecurityKey(authenticationData.SigningKey),
                SecurityAlgorithms.HmacSha256);
            var securityToken = new JwtSecurityToken(
                issuer: authenticationData.Issuer,
                audience: authenticationData.Audience,
                claims: claims.ToArray(),
                expires: DateTimeOffset.UtcNow.AddMinutes(2).UtcDateTime,
                signingCredentials: credentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            var client = vehicleApiFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}
