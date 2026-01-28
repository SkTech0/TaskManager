FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TaskManager.sln ./
COPY TaskManager.API/TaskManager.API.csproj TaskManager.API/
COPY TaskManager.Application/TaskManager.Application.csproj TaskManager.Application/
COPY TaskManager.Domain/TaskManager.Domain.csproj TaskManager.Domain/
COPY TaskManager.Infrastructure/TaskManager.Infrastructure.csproj TaskManager.Infrastructure/
RUN dotnet restore TaskManager.sln

COPY . .
RUN dotnet publish TaskManager.API/TaskManager.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Use PORT environment variable if provided (Railway), otherwise default to 8080
# Railway provides PORT env var, we'll use it in Program.cs
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Docker
ENV PORT=8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TaskManager.API.dll"]

