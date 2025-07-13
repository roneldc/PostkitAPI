# Stage 1: Build API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and API project files
COPY Postkit.sln ./
COPY Postkit.API/Postkit.API.csproj Postkit.API/
COPY Postkit.Comments/Postkit.Comments.csproj Postkit.Comments/
COPY Postkit.Identity/Postkit.Identity.csproj Postkit.Identity/
COPY Postkit.Infrastructure/Postkit.Infrastructure.csproj Postkit.Infrastructure/
COPY Postkit.Notifications/Postkit.Notifications.csproj Postkit.Notifications/
COPY Postkit.Posts/Postkit.Posts.csproj Postkit.Posts/
COPY Postkit.Reactions/Postkit.Reactions.csproj Postkit.Reactions/
COPY Postkit.Shared/Postkit.Shared.csproj Postkit.Shared/
COPY Postkit.Tests/Postkit.Tests.csproj Postkit.Tests/

# Restore packages
RUN dotnet restore

# Copy all source code
COPY . .

# Publish API
WORKDIR /src/Postkit.API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./

EXPOSE 80
ENTRYPOINT ["dotnet", "Postkit.API.dll"]