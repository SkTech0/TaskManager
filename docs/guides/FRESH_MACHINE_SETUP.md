# Fresh Machine Setup Guide

This guide helps you set up and run the Task Manager application on a fresh machine for both local development and production deployment.

## ✅ What Works Out of the Box

The project is configured to work for **both local and production** without code changes:

- ✅ **Local Development**: Uses `localhost` URLs by default
- ✅ **Docker**: Uses service names for internal communication
- ✅ **Production**: Uses environment variables to override URLs
- ✅ **No hardcoded production URLs** in source code

## 🚀 Quick Start on Fresh Machine

### Option 1: Docker (Recommended - No Configuration Needed)

```bash
# Clone the repository
git clone https://github.com/SkTech0/TaskManager.git
cd TaskManager

# Copy example docker-compose file
cp docker/docker-compose.example.yml docker/docker-compose.yml

# Generate secure secrets (optional, but recommended)
./scripts/generate-secrets.sh  # Linux/macOS
# OR
.\scripts\generate-secrets.ps1  # Windows

# Edit docker-compose.yml and update:
# - Jwt__SecretKey (use generated secret)
# - Auth__PasswordSalt (use generated salt)
# - POSTGRES_PASSWORD (optional, for security)

# Start all services
cd docker
docker-compose up --build
```

**That's it!** The application will be available at:
- Frontend: http://localhost:4200
- API: http://localhost:5001
- Swagger: http://localhost:5001/swagger

### Option 2: Local Development (Without Docker)

#### Backend Setup

1. **Install .NET 8 SDK** (8.0.402+)
2. **Install PostgreSQL 16/17**
3. **Configure database**:
   ```bash
   # Copy example configuration
   cp backend/TaskManager.API/appsettings.example.json backend/TaskManager.API/appsettings.json
   
   # Edit appsettings.json:
   # - Update ConnectionStrings__DefaultConnection with your PostgreSQL details
   # - Generate and set Jwt__SecretKey (32+ characters)
   # - Generate and set Auth__PasswordSalt
   ```

4. **Run backend**:
   ```bash
   cd backend
   dotnet restore
   dotnet run --project TaskManager.API
   ```

#### Frontend Setup

1. **Install Node.js 20+ and npm 10+**
2. **Install dependencies**:
   ```bash
   cd frontend
   npm install
   ```

3. **Run frontend**:
   ```bash
   npm start
   ```

The frontend will automatically use `http://localhost:5001/api` (configured in `environment.development.ts`).

## 🔧 Configuration Files

### Files You Need to Configure

1. **`docker/docker-compose.yml`** (for Docker)
   - Copy from `docker-compose.example.yml`
   - Update secrets (JWT, Password Salt, DB password)

2. **`backend/TaskManager.API/appsettings.json`** (for local backend)
   - Copy from `appsettings.example.json`
   - Update connection string and secrets

### Files That Work Automatically

- ✅ `frontend/src/environments/environment.ts` - Uses localhost for local dev
- ✅ `frontend/src/environments/environment.development.ts` - Uses localhost
- ✅ `frontend/Dockerfile` - Uses Docker service names by default
- ✅ `backend/TaskManager.API/Program.cs` - Reads from environment variables

## 🌐 Production Deployment

For production, you **don't need to change any code**. Just set environment variables:

### Railway Deployment

1. **Backend Environment Variables**:
   ```
   ConnectionStrings__DefaultConnection=<postgres-url>
   Jwt__SecretKey=<your-secret>
   Auth__PasswordSalt=<your-salt>
   FrontendUrl=https://your-frontend-url.com
   PORT=8080
   ```

2. **Frontend Build Argument** (in Railway):
   ```
   API_URL=https://your-api-url.com/api
   ```

### Docker Production

```bash
# Build frontend with production API URL
cd frontend
docker build --build-arg API_URL=https://your-api-url.com/api -t taskmanager-frontend .

# Backend uses environment variables from docker-compose or Railway
```

## 📋 Checklist for Fresh Machine

- [ ] Install .NET 8 SDK (for local backend)
- [ ] Install Node.js 20+ and npm (for local frontend)
- [ ] Install Docker Desktop (for Docker deployment)
- [ ] Install PostgreSQL (for local database, optional if using Docker)
- [ ] Clone repository
- [ ] Copy `docker-compose.example.yml` to `docker-compose.yml`
- [ ] Copy `appsettings.example.json` to `appsettings.json` (if running locally)
- [ ] Generate secrets using `scripts/generate-secrets.sh`
- [ ] Update secrets in configuration files
- [ ] Run `docker-compose up` or start services individually

## 🔒 Security Notes

**Never commit:**
- ❌ `appsettings.json` (with real secrets)
- ❌ `docker-compose.yml` (with real secrets)
- ❌ Any file with actual passwords or API keys

**Always commit:**
- ✅ `appsettings.example.json` (template)
- ✅ `docker-compose.example.yml` (template)
- ✅ All source code

## 🐛 Troubleshooting

### Port Already in Use

If ports 4200, 5001, or 5432 are in use:

**Docker**: Edit `docker-compose.yml` and change port mappings:
```yaml
ports:
  - "4201:80"  # Frontend
  - "5002:8080"  # API
  - "5433:5432"  # Database
```

**Local**: 
- Backend: Edit `Properties/launchSettings.json`
- Frontend: Edit `package.json` scripts or use `npm start -- --port 4201`

### Database Connection Issues

- Check PostgreSQL is running
- Verify connection string format
- Ensure credentials match

### Frontend Can't Connect to API

- Verify API is running on correct port
- Check CORS configuration in backend
- Ensure `apiBaseUrl` in `environment.ts` matches API URL

## 📚 Next Steps

- Read [Quick Start Guide](../QUICKSTART.md) for detailed instructions
- Read [Build & Run Guide](../build-run.md) for development setup
- Read [Deployment Guide](../deployment/DEPLOYMENT.md) for production deployment

## ✅ Summary

**For Local Development:**
- No code changes needed
- Just copy example config files and set secrets
- Use Docker or run services individually

**For Production:**
- No code changes needed
- Set environment variables in your deployment platform
- Override API URL in Docker build args if needed

The project is designed to work seamlessly in both environments! 🎉
