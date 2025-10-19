# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the project file first and restore dependencies
COPY ["API.csproj", "./"]
RUN dotnet restore "API.csproj"

# Copy the rest of the project files and publish
COPY . .
RUN dotnet publish "API.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "API.dll"]
