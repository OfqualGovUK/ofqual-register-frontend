#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base                               

LABEL maintainer="OfqualDevs"
LABEL description="This Dockerfile builds and runs the Ofqual Register frontend as a .NET 8.0 ASP.NET application with a multi-stage build process for efficient containerization and fast debugging."

USER root
RUN apt-get update && apt-get upgrade -y && apt-get install -y curl \
    && rm -rf /var/lib/apt/lists/* 
    
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Ofqual.Common.RegisterFrontend.csproj", "."]
RUN dotnet nuget locals all --clear && dotnet restore "./Ofqual.Common.RegisterFrontend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./Ofqual.Common.RegisterFrontend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ofqual.Common.RegisterFrontend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER app

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 CMD curl --fail http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "Ofqual.Common.RegisterFrontend.dll"]