# Quick Start Guide

This guide will help you get the Task Manager application up and running quickly.

## Prerequisites Check

Before starting, verify you have the required tools installed:

```bash
# Check .NET SDK
dotnet --version
# Should show: 8.0.402 or later

# Check Node.js
node --version
# Should show: v20.x.x or later

# Check npm
npm --version
# Should show: 10.x.x or later

# Check Docker (optional, for containerized deployment)
docker --version
docker compose version
```

## Option 1: Docker (Easiest - Recommended)

### Step 1: Start Docker Desktop

Ensure Docker Desktop is running on your machine.

### Step 2: Start All Services

```bash
cd docker
docker-compose up --build
```

This will:
- Build and start PostgreSQL database
- Build and start the .NET API (waits for database to be healthy)
- Build and start the Angular frontend
- Apply database migrations automatically
- Seed the database with default admin user

**Note**: The API includes retry logic and waits for PostgreSQL to be ready. If you see connection errors initially, wait a few seconds - the API will retry automatically.

### Step 3: Access the Application

- **Frontend**: http://localhost:4200
- **API Swagger**: http://localhost:5001/swagger

### Step 4: Login

Use the default credentials:
- **Email**: `admin@taskmanager.local`
- **Password**: `Admin@123`

### Stop Services

```bash
cd docker
docker-compose down
```

## Option 2: Local Development

### Step 1: Setup Backend

1. **Install .NET dependencies**:
```bash
cd backend
dotnet restore
```

2. **Configure database**:
   - Edit `backend/TaskManager.API/appsettings.json`
   - Update `ConnectionStrings:DefaultConnection` with your PostgreSQL connection
   - Set `Jwt:SecretKey` and `Auth:PasswordSalt` to secure random values

3. **Run migrations** (if needed):
```bash
cd backend/TaskManager.Infrastructure
dotnet ef database update --startup-project ../TaskManager.API
```

4. **Start the API**:
```bash
cd backend
dotnet run --project TaskManager.API
```

The API will be available at: http://localhost:5000

### Step 2: Setup Frontend

1. **Install Node dependencies**:
```bash
cd frontend
npm install
```

2. **Configure API URL** (if backend is on different port):
   - Edit `frontend/src/environments/environment.ts`
   - Update `apiBaseUrl` (default: `http://localhost:5001/api`)

3. **Start development server**:
```bash
cd frontend
npm start
```

The frontend will be available at: http://localhost:4200

## Common Commands

### Backend Commands

```bash
# Restore dependencies
cd backend
dotnet restore

# Run the API
dotnet run --project TaskManager.API

# Build for production
dotnet build -c Release

# Run tests (if available)
dotnet test
```

### Frontend Commands

```bash
# Install dependencies
cd frontend
npm install

# Start development server
npm start
# or
ng serve

# Build for production
npm run build

# Run tests
npm test

# Lint code
npm run lint
```

### Docker Commands

```bash
# Start all services
cd docker
docker-compose up --build

# Start in background
docker-compose up -d --build

# View logs
docker-compose logs -f [service-name]

# Stop services
docker-compose down

# Rebuild specific service
docker-compose build [service-name]
docker-compose up -d [service-name]

# Remove all containers and volumes
docker-compose down -v
```

## Troubleshooting

### Port Already in Use

If you get port conflicts:

- **Port 5000/5001**: Change API port in `docker-compose.yml` or `appsettings.json`
- **Port 4200**: Change frontend port in `package.json` or `docker-compose.yml`
- **Port 5432**: Change PostgreSQL port in `docker-compose.yml`

### Database Connection Issues

- Ensure PostgreSQL is running
- Check connection string in `appsettings.json`
- Verify database credentials
- Check if database exists: `psql -U taskuser -d taskmanagerdb`

### Frontend Can't Connect to API

- Verify API is running
- Check `apiBaseUrl` in `environment.ts`
- Check CORS configuration in `Program.cs`
- Check browser console for errors

### Docker Build Fails

- Ensure Docker Desktop is running
- Check Docker has enough resources allocated
- Try: `docker system prune -a` to clean up
- Rebuild: `docker-compose build --no-cache`

## Next Steps

- Read [architecture.md](./architecture.md) for system design details
- Read [api-docs.md](./api-docs.md) for API endpoint documentation
- Read [requirements.md](./requirements.md) for detailed dependency versions
