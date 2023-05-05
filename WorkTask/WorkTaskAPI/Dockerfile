FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
WORKDIR "/src/WorkTask/WorkTaskAPI"
RUN dotnet nuget add source "/src/NugetPackages" -n "LocalPackages" --configfile "nuget.config"
RUN dotnet restore "WorkTaskAPI.csproj" -c Release -r linux-x64 -o /app/build --configfile "nuget.config"

FROM build AS publish
RUN dotnet publish "WorkTaskAPI.csproj" --no-restore -c Release  -r linux-x64 -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrassLoon.WorkTaskAPI.dll"]