FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PlantDB/PlantDB.csproj", "PlantDB/"]
RUN dotnet restore "PlantDB/PlantDB.csproj"
COPY . .
WORKDIR "/src/PlantDB"
RUN dotnet build "PlantDB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlantDB.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlantDB.dll"]
