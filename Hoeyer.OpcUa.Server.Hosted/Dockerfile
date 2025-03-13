FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

# Expose OPC UA Server port (4840) and any additional necessary ports
ENV PORT=5000
EXPOSE $PORT

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OpcServerServiceHost/OpcServerServiceHost.csproj", "OpcServerServiceHost/"]
COPY ["Hoeyer.OpcUa.Server/Hoeyer.OpcUa.Server.csproj", "Hoeyer.OpcUa.Server/"]
COPY ["Hoeyer.Common/Hoeyer.Common.csproj", "Hoeyer.Common/"]
COPY ["Hoeyer.OpcUa.Core/Hoeyer.OpcUa.Core.csproj", "Hoeyer.OpcUa.Core/"]
RUN dotnet restore "OpcServerServiceHost/OpcServerServiceHost.csproj"
COPY . .
WORKDIR "/src/OpcServerServiceHost"
RUN dotnet build "OpcServerServiceHost.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OpcServerServiceHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

HEALTHCHECK --interval=30s --timeout=10s --retries=3 \
  CMD curl --fail http://localhost:$PORT/ || exit 1

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpcServerServiceHost.dll"]
