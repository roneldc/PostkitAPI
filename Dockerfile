# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and restore packages
COPY Postkit.sln ./
COPY Postkit.API/Postkit.API.csproj Postkit.API/
COPY Postkit.Application/Postkit.Application.csproj Postkit.Application/
COPY Postkit.Domain/Postkit.Domain.csproj Postkit.Domain/
COPY Postkit.Infrastructure/Postkit.Infrastructure.csproj Postkit.Infrastructure/
RUN dotnet restore

# Copy all source files
COPY . .

# Publish the API
WORKDIR /src/Postkit.API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "Postkit.API.dll"]