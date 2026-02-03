FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build
COPY src/Companies.Import.Worker/Companies.Import.Worker.csproj src/Companies.Import.Worker/
COPY src/Companies.Application/Companies.Application.csproj src/Companies.Application/
COPY src/Companies.Infrastructure/Companies.Infrastructure.csproj src/Companies.Infrastructure/
RUN dotnet restore "src/Companies.Import.Worker/Companies.Import.Worker.csproj"
COPY . .
WORKDIR "/build/src/Companies.Import.Worker"
RUN dotnet build "Companies.Import.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Companies.Import.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Companies.Import.Worker.dll"]