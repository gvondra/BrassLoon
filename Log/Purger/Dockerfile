FROM mcr.microsoft.com/dotnet/runtime:6.0-bullseye-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
WORKDIR "/src/Log/Purger"
RUN dotnet nuget add source "/src/NugetPackages" -n "LocalPackages" --configfile "nuget.config"
RUN dotnet build "Purger.csproj" -r linux-x64 --self-contained true -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Purger.csproj" --no-restore --no-build -r linux-x64 --self-contained true -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrassLoon.Log.Purger.dll"]