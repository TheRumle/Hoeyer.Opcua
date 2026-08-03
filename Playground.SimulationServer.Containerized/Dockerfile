FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 4840
ENV OPCUA_PORT=4840 \
    OPCUA_PROTOCOL=OpcTcp \
    OPCUA_SERVERID=HostedSimulation \
    OPCUA_SERVERNAME=HostedSimulation \
    OPCUA_APPLICATION_NAME=Simulation \
    CERTIFICATE_PKI_ROOT="/root/.local/share/OPC Foundation/pki" \
    CERTIFICATE_SUBJECTNAME="CN=HostedSimulation" \
    CERTIFICATE_ACCEPT_UNTRUSTED="true" \
    CERTIFICATE_ADD_TO_TRUSTED_STORE="true" \
    CERTIFICATE_ACCEPT_REJECTED="false"
    
ENV LOG_LEVEL=DEBUG
    
ENV OPCUA__SERVERID=${OPCUA_SERVERID} \
    OPCUA__SERVERNAME=${OPCUA_SERVERNAME} \
    OPCUA__HOST=localhost \
    OPCUA__PORT=${OPCUA_PORT} \
    OPCUA__PROTOCOL=${OPCUA_PROTOCOL} \
    OPCUA__APPLICATIONURI=${OPCUA_APPLICATION_NAME} \
    Logging__LogLevel__Default=${LOG_LEVEL}
    
ENV OPCUA__CertificateConfiguration__PkiRoot=${CERTIFICATE_PKI_ROOT} \
    OPCUA__CertificateConfiguration__CertificateSubjectName=${CERTIFICATE_SUBJECTNAME} \
    OPCUA__CertificateConfiguration__AutoAcceptUntrustedCertificates=${CERTIFICATE_ACCEPT_UNTRUSTED} \
    OPCUA__CertificateConfiguration__AddAppCertToTrustedStore=${CERTIFICATE_ADD_TO_TRUSTED_STORE} \
    OPCUA__CertificateConfiguration__AcceptRejectedCertificates=${CERTIFICATE_ACCEPT_REJECTED}

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
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

HEALTHCHECK --interval=30s --timeout=3s --start-period=3s --retries=3 \
 CMD curl --fail http://localhost:8080/health/server || exit 1