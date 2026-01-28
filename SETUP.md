# Setup Instructions for GitHub Repository

This guide helps you set up the Task Manager application from the GitHub repository.

## Quick Setup

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/TaskManager.git
cd TaskManager
```

### 2. Configure Secrets

#### Backend Configuration

1. Copy the example configuration:
```bash
cp backend/TaskManager.API/appsettings.example.json backend/TaskManager.API/appsettings.json
```

2. Edit `backend/TaskManager.API/appsettings.json`:
   - Generate a secure JWT Secret Key (32+ characters)
   - Generate a secure Password Salt
   - Update database connection string if not using Docker

**Generate secrets**:
```bash
# JWT Secret Key
openssl rand -base64 32

# Password Salt
openssl rand -base64 24
```

#### Docker Configuration (if using Docker)

1. Copy the example docker-compose file:
```bash
cp docker/docker-compose.example.yml docker/docker-compose.yml
```

2. Edit `docker/docker-compose.yml`:
   - Update `Jwt__SecretKey` with your generated secret
   - Update `Auth__PasswordSalt` with your generated salt
   - Update database password if desired

#### Frontend Configuration

1. Update `frontend/src/environments/environment.ts`:
   - Set `apiBaseUrl` to your backend API URL
   - For local development: `http://localhost:5001/api`
   - For production: `https://your-api-domain.com/api`

### 3. Install Dependencies

#### Backend

```bash
cd backend
dotnet restore
```

#### Frontend

```bash
cd frontend
npm install
```

### 4. Run the Application

#### Option A: Docker (Recommended)

```bash
cd docker
docker-compose up --build
```

#### Option B: Local Development

**Terminal 1 - Backend**:
```bash
cd backend
dotnet run --project TaskManager.API
```

**Terminal 2 - Frontend**:
```bash
cd frontend
npm start
```

### 5. Access the Application

- **Frontend**: http://localhost:4200
- **API Swagger**: http://localhost:5001/swagger

**Default Login**:
- Email: `admin@taskmanager.local`
- Password: `Admin@123`

## What's Included

- ✅ Complete source code
- ✅ Docker configuration
- ✅ Documentation
- ✅ Example configuration files
- ✅ CI/CD workflow (GitHub Actions)

## What's NOT Included (Security)

- ❌ `appsettings.json` with real secrets (use `appsettings.example.json`)
- ❌ `docker-compose.yml` with real secrets (use `docker-compose.example.yml`)
- ❌ Build artifacts (`bin/`, `obj/`, `dist/`, `node_modules/`)
- ❌ IDE-specific files

## Next Steps

1. Read [QUICKSTART.md](./docs/QUICKSTART.md) for detailed instructions
2. Read [DEPLOYMENT.md](./docs/DEPLOYMENT.md) for deployment options
3. Read [requirements.md](./docs/requirements.md) for system requirements

## Troubleshooting

### Port Already in Use

Change ports in:
- `docker-compose.yml` (for Docker)
- `appsettings.json` (for local backend)
- `package.json` (for local frontend)

### Database Connection Errors

- Ensure PostgreSQL is running
- Check connection string format
- Verify credentials

### Frontend Build Errors

- Ensure Node.js 20+ is installed
- Run `npm install` in frontend directory
- Check for version conflicts

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This project is provided as-is for educational and development purposes.
