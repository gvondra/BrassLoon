FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["LogAPI/LogAPI.csproj", "LogAPI/"]
COPY ["Log.Core/Log.Core.csproj", "Log.Core/"]
COPY ["Log.Data/Log.Data.csproj", "Log.Data/"]
COPY ["../CommonCore/CommonCore.csproj", "../CommonCore/"]
COPY ["Log.Framework/Log.Framework.csproj", "Log.Framework/"]
COPY ["Interface.Log/Interface.Log.csproj", "Interface.Log/"]
RUN dotnet restore "LogAPI/LogAPI.csproj"
COPY . .
WORKDIR "/src/LogAPI"
RUN dotnet build "LogAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LogAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LogAPI.dll"]