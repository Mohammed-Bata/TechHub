# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TechHub.Api/TechHub.Api.csproj", "TechHub.Api/"]
COPY ["TechHub.Application/TechHub.Application.csproj", "TechHub.Application/"]
COPY ["TechHub.Domain/TechHub.Domain.csproj", "TechHub.Domain/"]
COPY ["TechHub.Infrastructure/TechHub.Infrastructure.csproj", "TechHub.Infrastructure/"]
RUN dotnet restore "./TechHub.Api/TechHub.Api.csproj"
COPY . .
WORKDIR "/src/TechHub.Api"
RUN dotnet build "./TechHub.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TechHub.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# to handle images 
# Create the directory
RUN mkdir -p /app/wwwroot/images

# Give full permissions (not secure for production, but good for dev)
RUN chmod -R 777 /app/wwwroot/images

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechHub.Api.dll"]