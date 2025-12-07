FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 4840

ENV OPCUA_PORT=4840 \
    OPCUA_PROTOCOL=OpcTcp \
    OPCUA_SERVERID=HostedSimulation \
    OPCUA_SERVERNAME=HostedSimulation \
    OPCUA_APPLICATION_NAME=Simulation
    
ENV LOG_LEVEL=DEBUG
    
ENV OPCUA__SERVERID=${OPCUA_SERVERID} \
    OPCUA__SERVERNAME=${OPCUA_SERVERNAME} \
    OPCUA__HOST=localhost \
    OPCUA__PORT=${OPCUA_PORT} \
    OPCUA__PROTOCOL=${OPCUA_PROTOCOL} \
    OPCUA__APPLICATIONURI=${OPCUA_APPLICATION_NAME} \
    Logging__LogLevel__Default=${LOG_LEVEL}

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Playground.SimulationServer.Containerized/Playground.SimulationServer.Containerized.csproj", "Playground.SimulationServer.Containerized/"]
RUN dotnet restore "Playground.SimulationServer.Containerized/Playground.SimulationServer.Containerized.csproj"

COPY . .
WORKDIR "/src/Playground.SimulationServer.Containerized"
RUN dotnet build "./Playground.SimulationServer.Containerized.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Playground.SimulationServer.Containerized.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Playground.SimulationServer.Containerized.dll"]

HEALTHCHECK --interval=10s --timeout=3s --start-period=5s --retries=3 \
 CMD curl --fail http://localhost:8080/health/server || exit 1