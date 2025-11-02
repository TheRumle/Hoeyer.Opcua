FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 4840

ENV OPCUA_PORT=4840 \
    OPCUA_PROTOCOL=OpcTcp \
    OPCUA_SERVERID=HostedSimulation \
    OPCUA_SERVERNAME=HostedSimulation \
    OPCUA_APPLICATIONNAME=Simulation
    
ENV OPCUA__PORT=$OPCUA_PORT \
    OPCUA__PROTOCOL=$OPCUA_PROTOCOL \
    OPCUA__SERVERID=$OPCUA_SERVERID \
    OPCUA__SERVERNAME=$OPCUA_SERVERNAME \
    OPCUA__APPLICATIONURI=/$OPCUA_APPLICATIONNAME \
    OPCUA__HOST=Localhost
    
USER $APP_UID

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