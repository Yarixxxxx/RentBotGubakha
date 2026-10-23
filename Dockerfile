FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY BotGubakha.csproj NuGet.Config ./
RUN dotnet restore BotGubakha.csproj

COPY . .
RUN dotnet publish BotGubakha.csproj \
    --configuration "$BUILD_CONFIGURATION" \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine AS final
WORKDIR /app
ENV DOTNET_EnableDiagnostics=0
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "BotGubakha.dll"]
