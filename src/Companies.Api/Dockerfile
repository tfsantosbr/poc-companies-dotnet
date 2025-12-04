FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /build
COPY src/Companies.Api/Companies.Api.csproj src/Companies.Api/
COPY src/Companies.Application/Companies.Application.csproj src/Companies.Application/
COPY src/Companies.Infrastructure/Companies.Infrastructure.csproj src/Companies.Infrastructure/
RUN dotnet restore "src/Companies.Api/Companies.Api.csproj"
COPY . .
WORKDIR "/build/src/Companies.Api"
RUN dotnet build "Companies.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Companies.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Companies.Api.dll"]