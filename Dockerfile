# Stage 1: Build API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and csproj files
COPY Postkit.sln ./
COPY Postkit.API/Postkit.API.csproj Postkit.API/
COPY Postkit.Identity/*.csproj Postkit.Identity/
COPY Postkit.Comments/*.csproj Postkit.Comments/
COPY Postkit.Posts/*.csproj Postkit.Posts/
COPY Postkit.Notifications/*.csproj Postkit.Notifications/
COPY Postkit.Reactions/*.csproj Postkit.Reactions/
COPY Postkit.Infrastructure/*.csproj Postkit.Infrastructure/
COPY Postkit.Shared/*.csproj Postkit.Shared/
COPY Postkit.Tests/*.csproj Postkit.Tests/

# Restore dependencies
RUN dotnet restore

# Copy entire source
COPY . .

# Build and publish the API
WORKDIR /src/Postkit.API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Expose default port
EXPOSE 80

# Run the API
ENTRYPOINT ["dotnet", "Postkit.API.dll"]