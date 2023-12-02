# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["AuthorVerseForum/AuthorVerseForum.csproj", "AuthorVerseForum/"]
RUN dotnet restore "AuthorVerseForum/AuthorVerseForum.csproj"

COPY . .

# Stage 2: Publish
RUN dotnet publish "AuthorVerseForum/AuthorVerseForum.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AuthorVerseForum.dll"]
