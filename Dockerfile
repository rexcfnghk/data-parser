FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DataParser.Console/DataParser.Console.fsproj", "DataParser.Console/"]
RUN dotnet restore "DataParser.Console/DataParser.Console.fsproj"
COPY . .
WORKDIR "/src/DataParser.Console"
RUN dotnet build "DataParser.Console.fsproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DataParser.Console.fsproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataParser.Console.dll"]
