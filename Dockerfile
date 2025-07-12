# Stage 1: Build the Blazor UI
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-ui
WORKDIR /src

COPY Postkit.UI/Postkit.UI.csproj Postkit.UI/
RUN dotnet restore Postkit.UI/Postkit.UI.csproj

COPY Postkit.UI/ Postkit.UI/
RUN dotnet publish Postkit.UI -c Release -o /out-ui


# Stage 2: Build the API and all modules
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-api
WORKDIR /src

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
COPY Postkit.UI/Postkit.UI.csproj Postkit.UI/

RUN dotnet restore

COPY . .

# Copy UI wwwroot from build-ui stage
RUN rm -rf Postkit.API/wwwroot
RUN mkdir -p Postkit.API/wwwroot
COPY --from=build-ui /out-ui/wwwroot/ Postkit.API/wwwroot/

# Publish API
WORKDIR /src/Postkit.API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false


# Stage 3: Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build-api /app/publish ./

EXPOSE 80
ENTRYPOINT ["dotnet", "Postkit.API.dll"]
