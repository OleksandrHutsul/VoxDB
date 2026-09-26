FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["VoxDB.Components/VoxDB.Components.csproj", "VoxDB.Components/"]
COPY ["VoxDB.Components.Common/VoxDB.Components.Common.csproj", "VoxDB.Components.Common/"]
COPY ["VoxDB.Entities/VoxDB.Entities.csproj", "VoxDB.Entities/"]

RUN dotnet restore "VoxDB.Components/VoxDB.Components.csproj"

COPY . .

RUN dotnet publish "VoxDB.Components/VoxDB.Components.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "VoxDB.Components.dll"]