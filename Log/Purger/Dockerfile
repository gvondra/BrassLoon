FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY . .
RUN dotnet nuget add source "/src/NugetPackages" -n "LocalPackages" --configfile "Log/Purger/nuget.config"
WORKDIR "/src/Log/Purger"
RUN dotnet restore "Purger.csproj" --configfile "nuget.config"
RUN dotnet build "Purger.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Purger.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrassLoon.Log.Purger.dll"]