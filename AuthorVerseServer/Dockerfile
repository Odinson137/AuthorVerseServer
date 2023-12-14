# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["AuthorVerseServer/AuthorVerseServer.csproj", "AuthorVerseServer/"]
RUN dotnet restore "./AuthorVerseServer/AuthorVerseServer.csproj"

COPY . .

# Stage 2: Publish
RUN dotnet publish "./AuthorVerseServer/AuthorVerseServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 3: Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AuthorVerseServer.dll"]
