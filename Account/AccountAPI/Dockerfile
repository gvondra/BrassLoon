FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet nuget add source "/src/NugetPackages" -n "LocalPackages" --configfile "Account/AccountAPI/nuget.config"
RUN dotnet restore "Account/AccountAPI/AccountAPI.csproj" --configfile "Account/AccountAPI/nuget.config"
WORKDIR "/src/Account/AccountAPI"
RUN dotnet build "AccountAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AccountAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrassLoon.AccountAPI.dll"]