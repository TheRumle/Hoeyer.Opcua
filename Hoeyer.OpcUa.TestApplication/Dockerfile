FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Hoeyer.OpcUa.TestApplication/Hoeyer.OpcUa.TestApplication.csproj", "Hoeyer.OpcUa.TestApplication/"]
RUN dotnet restore "Hoeyer.OpcUa.TestApplication/Hoeyer.OpcUa.TestApplication.csproj"
COPY . .
WORKDIR "/src/Hoeyer.OpcUa.TestApplication"
RUN dotnet build "Hoeyer.OpcUa.TestApplication.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Hoeyer.OpcUa.TestApplication.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hoeyer.OpcUa.TestApplication.dll"]
