﻿FROM mcr.microsoft.com/dotnet/runtime:8.0-bookworm-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
WORKDIR "/src/WorkTask/Purger"
RUN dotnet nuget add source "/src/NugetPackages" -n "LocalPackages" --configfile "nuget.config"
RUN dotnet build "Purger.csproj" -r linux-x64 --no-self-contained -c Release 

FROM build AS publish
RUN dotnet publish "Purger.csproj" --no-restore --no-build -r linux-x64 --no-self-contained -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrassLoon.WorkTask.Purger.dll"]