# Task Manager Application

A full-stack task management system built with .NET 8 and Angular 18.

[![CI](https://github.com/yourusername/TaskManager/actions/workflows/ci.yml/badge.svg)](https://github.com/yourusername/TaskManager/actions/workflows/ci.yml)

> **Note**: Replace `yourusername` in the badge URL with your actual GitHub username after creating the repository.

## Features

- User authentication and authorization (JWT-based)
- Task CRUD operations (Create, Read, Update, Delete)
- Task search and filtering
- Responsive web interface
- RESTful API with Swagger documentation
- Docker containerization support

## Tech Stack

### Backend
- **.NET 8.0** - ASP.NET Core Web API
- **PostgreSQL 16** - Database
- **Entity Framework Core 8.0** - ORM
- **JWT Authentication** - Security
- **Serilog** - Logging

### Frontend
- **Angular 18.2** - Frontend framework (Standalone components)
- **TypeScript 5.5** - Programming language
- **RxJS 7.8** - Reactive programming
- **Nginx** - Web server (production)

## Quick Start

### Prerequisites

- .NET 8 SDK (8.0.402+)
- Node.js 20+ and npm 10+
- Docker Desktop (optional, for containerized deployment)
- PostgreSQL 16/17 (optional, if not using Docker)

### Run with Docker (Recommended)

```bash
cd docker
docker-compose up --build
```

Access the application:
- **Frontend**: http://localhost:4200
- **API Swagger**: http://localhost:5001/swagger

Default login:
- Email: `admin@taskmanager.local`
- Password: `Admin@123`

### Run Locally

**Backend**:
```bash
cd backend
dotnet restore
dotnet run --project TaskManager.API
```

**Frontend**:
```bash
cd frontend
npm install
npm start
```

## Documentation

- **[Quick Start Guide](./docs/QUICKSTART.md)** - Get started quickly
- **[Requirements](./docs/requirements.md)** - System requirements and dependencies
- **[Build & Run](./docs/build-run.md)** - Detailed build and run instructions
- **[API Documentation](./docs/api-docs.md)** - API endpoint reference
- **[Architecture](./docs/architecture.md)** - System architecture overview
- **[Database Design](./docs/db-design.md)** - Database schema and design

## Project Structure

```
TaskManager/
├── backend/                 # .NET backend solution
│   ├── TaskManager.API/     # Web API project
│   ├── TaskManager.Application/  # Application layer
│   ├── TaskManager.Domain/  # Domain entities
│   └── TaskManager.Infrastructure/  # Data access & infrastructure
├── frontend/                # Angular frontend
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/        # Services, guards, interceptors
│   │   │   └── features/    # Feature modules (auth, tasks)
│   │   └── environments/    # Environment configuration
│   └── Dockerfile           # Frontend Docker configuration
├── docker/                  # Docker Compose configuration
│   └── docker-compose.yml
└── docs/                    # Documentation
```

## Development

### Backend Commands

```bash
# Restore dependencies
dotnet restore

# Run the API
dotnet run --project TaskManager.API

# Run migrations
dotnet ef database update --project TaskManager.Infrastructure --startup-project TaskManager.API

# Build for production
dotnet build -c Release
```

### Frontend Commands

```bash
# Install dependencies
npm install

# Start development server
npm start

# Build for production
npm run build

# Run tests
npm test
```

## Configuration

### Backend

Edit `backend/TaskManager.API/appsettings.json`:
- Database connection string
- JWT settings (SecretKey, Issuer, Audience)
- Password salt

### Frontend

Edit `frontend/src/environments/environment.ts`:
- API base URL (default: `http://localhost:5001/api`)

## Deployment

See [DEPLOYMENT.md](./docs/DEPLOYMENT.md) for detailed deployment instructions to:
- Railway
- Render
- Vercel
- Docker on VPS

## Getting Started

### From GitHub

1. **Clone the repository**:
```bash
git clone https://github.com/yourusername/TaskManager.git
cd TaskManager
```

2. **Generate secure secrets**:
```bash
# Linux/macOS
./scripts/generate-secrets.sh

# Windows PowerShell
.\scripts\generate-secrets.ps1
```

3. **Configure the application**:
   - Copy `backend/TaskManager.API/appsettings.example.json` to `appsettings.json`
   - Copy `docker/docker-compose.example.yml` to `docker/docker-compose.yml`
   - Update with generated secrets

4. **Run with Docker**:
```bash
cd docker
docker-compose up --build
```

See [SETUP.md](./SETUP.md) for detailed setup instructions.

### Deploy to Production

See [DEPLOYMENT.md](./docs/DEPLOYMENT.md) for deployment guides to:
- Railway
- Render  
- Vercel
- Docker on VPS

## License

This project is provided as-is for educational and development purposes.
