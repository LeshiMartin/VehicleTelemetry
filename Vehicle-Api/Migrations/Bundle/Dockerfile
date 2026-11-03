FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Vehicle-Api/Vehicle-Api.csproj Vehicle-Api/
COPY Common/Common.csproj Common/
RUN dotnet restore Vehicle-Api/Vehicle-Api.csproj

COPY . .
RUN dotnet tool install --global dotnet-ef --version 10.* \
 && export PATH="$PATH:/github/home/.dotnet/tools" \
 && dotnet ef migrations bundle \
    --project Vehicle-Api/Vehicle-Api.csproj \
    --startup-project Vehicle-Api/Vehicle-Api.csproj \
    --configuration Release \
    --self-contained -r linux-x64 \
    -o /app/EfBundle

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0
WORKDIR /app

COPY --from=build /app/EfBundle /app/EfBundle
COPY Vehicle-Api/Migrations/Bundle/PassConnectionString.sh /app/pass-connection-string.sh

RUN sed -i 's/\r$//' /app/pass-connection-string.sh && \
    chmod +x /app/EfBundle /app/pass-connection-string.sh

ENTRYPOINT ["/app/pass-connection-string.sh"]