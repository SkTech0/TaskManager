## Runtime and Tooling Requirements

This document lists all tools and versions required to build and run the Task Management System.

### Operating System

- Windows 10 or later (64-bit)  
- Linux (e.g., Ubuntu 22.04) or macOS 13+ are also supported with equivalent tools installed.

### Backend Requirements

- **.NET SDK**:  
  - **Required**: .NET 8 SDK (8.0.402 or later)
  - **Check version**: `dotnet --version`
  - **Install**: Download from https://dotnet.microsoft.com/download/dotnet/8.0
  - **Verify**: `dotnet --list-sdks` should show 8.0.x

- **Entity Framework Core tools (optional but recommended for local migrations)**:  
  - Installed as part of the solution; you can use:  
    - `dotnet ef` (from `dotnet-ef` global tool if you want CLI access)
    - **Install**: `dotnet tool install --global dotnet-ef`

- **PostgreSQL**:
  - **Server**: PostgreSQL 16 or 17
  - **Client**: `psql` command-line client (optional but useful)  
  - **Check version**: `psql --version`
  - **Install**: Download from https://www.postgresql.org/download/

- **Docker (for containerized deployment)**:
  - **Docker Engine**: 24+ (Desktop on Windows/Mac)  
  - **Docker Compose**: v2.x
  - **Check versions**: 
    - `docker --version`
    - `docker compose version`
  - **Install**: Download Docker Desktop from https://www.docker.com/products/docker-desktop

### Frontend Requirements

- **Node.js and npm**:
  - **Required**: 
    - Node.js: **>= 20.0.0** (recommended: 20 LTS or 22 LTS)
    - npm: **>= 10.0.0** (comes with Node.js)
  - **Check versions**: 
    - `node --version`
    - `npm --version`
  - **Install**: 
    - Download from https://nodejs.org (recommended: 20 LTS)
    - Or use version manager:
      - **nvm (macOS/Linux)**: `nvm install 20 && nvm use 20`
      - **nvm-windows**: `nvm install 20.12.0 && nvm use 20.12.0`

- **Angular CLI**:
  - **Required**: `@angular/cli` 18.2.7 (installed as a dev dependency)
  - **Check version**: `ng version`
  - **Install globally (optional)**: `npm install -g @angular/cli@18.2.7`

### Project-Level Dependencies

#### Backend (.NET)

**Installed via**: `dotnet restore` in `backend/` directory

**Core Framework**:
- `.NET 8.0` (Target Framework: net8.0)
- `Microsoft.NET.Sdk.Web` (ASP.NET Core 8)

**NuGet Packages**:
- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.0)
- `Swashbuckle.AspNetCore` (6.5.0)
- `Serilog.AspNetCore` (8.0.0)
- `Serilog.Sinks.Console` (5.0.0)
- `Microsoft.EntityFrameworkCore` (8.0.0)
- `Microsoft.EntityFrameworkCore.Design` (8.0.0)
- `Npgsql.EntityFrameworkCore.PostgreSQL` (8.0.0)
- `Microsoft.IdentityModel.Tokens` (8.0.1)
- `System.IdentityModel.Tokens.Jwt` (8.0.1)

**Restore command**: `cd backend && dotnet restore`

#### Frontend (Angular)

**Installed via**: `npm install` in `frontend/` directory

**Angular Core (18.2.0)**:
- `@angular/animations` (^18.2.0)
- `@angular/common` (^18.2.0)
- `@angular/compiler` (^18.2.0)
- `@angular/core` (^18.2.0)
- `@angular/forms` (^18.2.0)
- `@angular/platform-browser` (^18.2.0)
- `@angular/platform-browser-dynamic` (^18.2.0)
- `@angular/router` (^18.2.0)

**Runtime Dependencies**:
- `rxjs` (~7.8.0)
- `tslib` (^2.3.0)
- `zone.js` (~0.14.10)

**Dev Dependencies**:
- `@angular/cli` (^18.2.7)
- `@angular-devkit/build-angular` (^18.2.7)
- `@angular/compiler-cli` (^18.2.0)
- `typescript` (~5.5.2)
- `@types/jasmine` (~5.1.0)
- `jasmine-core` (~5.2.0)
- `karma` (~6.4.0)

**Install command**: `cd frontend && npm install`

### Environment Configuration

#### Backend configuration files

- `backend/TaskManager.API/appsettings.json` (local development):
  - `ConnectionStrings:DefaultConnection`: PostgreSQL connection string (host, port, DB name, username, password).
  - `Jwt:Issuer`, `Jwt:Audience`, `Jwt:SecretKey`, `Jwt:ExpiryMinutes`:
    - Set `SecretKey` to a long, random string (32+ characters).
  - `Auth:PasswordSalt`:
    - Set to a strong, random string for password hashing.

- `backend/TaskManager.API/appsettings.Docker.json` (Docker):
  - Same keys as above, but with DB host set to `taskmanager-db`.

#### Docker Compose environment variables

- `docker/docker-compose.yml` defines:
  - PostgreSQL container `taskmanager-db`
  - API container `taskmanager-api`
  - SPA container `taskmanager-frontend`
  - Environment variables for:
    - `ConnectionStrings__DefaultConnection`
    - `Jwt__Issuer`, `Jwt__Audience`, `Jwt__SecretKey`, `Jwt__ExpiryMinutes`
    - `Auth__PasswordSalt`

Update `Jwt__SecretKey` and `Auth__PasswordSalt` with secure values before running in non-development environments.

#### Frontend environment files

- `frontend/src/environments/environment.ts`
- `frontend/src/environments/environment.development.ts`

**Default configuration**:
- `apiBaseUrl`: `http://localhost:5001/api` (for Docker)
- For local development (backend on port 5000): `http://localhost:5000/api`

When running behind a different host/port or reverse proxy, adjust `apiBaseUrl` accordingly.

### Setup Commands

#### Initial Setup

1. **Verify Prerequisites**:
```bash
dotnet --version      # Should be 8.0.402+
node --version        # Should be v20.x.x+
npm --version         # Should be 10.x.x+
docker --version      # Optional, for Docker deployment
```

2. **Backend Setup**:
```bash
cd backend
dotnet restore
```

3. **Frontend Setup**:
```bash
cd frontend
npm install
```

4. **Configure Secrets** (for local development):
   - Edit `backend/TaskManager.API/appsettings.json`:
     - Set `Jwt:SecretKey` to a strong random string (32+ characters)
     - Set `Auth:PasswordSalt` to a strong random string
   - **Note**: Docker Compose already has these configured

5. **Docker Setup** (optional):
   - Ensure Docker Desktop is running
   - Secrets are pre-configured in `docker/docker-compose.yml`
   - No additional configuration needed for development

### Running the Application

**Option 1: Docker (Recommended)**:
```bash
cd docker
docker-compose up --build
```

**Option 2: Local Development**:
```bash
# Terminal 1: Start Backend
cd backend
dotnet run --project TaskManager.API

# Terminal 2: Start Frontend
cd frontend
npm start
```

See [QUICKSTART.md](./QUICKSTART.md) for detailed step-by-step instructions.

