FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Vehicle-Api/Vehicle-Api.csproj", "Vehicle-Api/"]
COPY ["Common/Common.csproj", "Common/"]

RUN dotnet restore "Vehicle-Api/Vehicle-Api.csproj"

COPY Vehicle-Api ./Vehicle-Api
COPY Common ./Common

WORKDIR "/src/Vehicle-Api"
RUN dotnet publish "Vehicle-Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Vehicle-Api.dll"]