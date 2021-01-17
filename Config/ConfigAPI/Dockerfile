FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY . .
RUN dotnet nuget add source "/src/NugetPackages" -n "LocalPackages" --configfile "Config/ConfigAPI/nuget.config"
RUN dotnet restore "Config/ConfigAPI/ConfigAPI.csproj" --configfile "Config/ConfigAPI/nuget.config"
WORKDIR "/src/Config/ConfigAPI"
RUN dotnet build "ConfigAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConfigAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrassLoon.ConfigAPI.dll"]