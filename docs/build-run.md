## Build and Run

### Prerequisites

Before running the application, ensure you have:

- **.NET 8 SDK** (8.0.402 or later)
- **Node.js 20+** and **npm 10+**
- **PostgreSQL 16/17** (optional if using Docker)
- **Docker Desktop** and **Docker Compose v2** (for containerized deployment)

See [requirements.md](./requirements.md) for detailed version information.

### Quick Start with Docker (Recommended)

This is the easiest way to run the entire application stack.

1. **Navigate to the docker directory**:
```bash
cd docker
```

2. **Start all services**:
```bash
docker-compose up --build
```

3. **Access the application**:
   - **Frontend**: http://localhost:4200
   - **API**: http://localhost:5001
   - **Swagger UI**: http://localhost:5001/swagger
   - **PostgreSQL**: localhost:5432

4. **Login with default credentials**:
   - Email: `admin@taskmanager.local`
   - Password: `Admin@123`

**Note**: 
- On first startup, the API automatically applies database migrations and seeds the database with a default administrator user and sample task.
- The API waits for PostgreSQL to be healthy before connecting (healthcheck configured).
- If the API fails to start, check logs: `docker-compose logs taskmanager-api`

**Stop services**:
```bash
docker-compose down
```

**View logs**:
```bash
docker-compose logs -f [service-name]  # e.g., taskmanager-api, taskmanager-frontend
```

### Running Backend Locally

1. **Install dependencies**:
```bash
cd backend
dotnet restore
```

2. **Configure database connection**:
   - Edit `backend/TaskManager.API/appsettings.json`
   - Update `ConnectionStrings:DefaultConnection` with your PostgreSQL connection details
   - Update `Jwt:SecretKey` and `Auth:PasswordSalt` with secure random values

3. **Run database migrations** (if needed):
```bash
cd backend/TaskManager.Infrastructure
dotnet ef database update --startup-project ../TaskManager.API
```

4. **Start the API**:
```bash
cd backend
dotnet run --project TaskManager.API
```

5. **Access the API**:
   - **API**: http://localhost:5000
   - **Swagger UI**: http://localhost:5000/swagger

### Running Frontend Locally

1. **Install dependencies**:
```bash
cd frontend
npm install
```

2. **Configure API URL** (if backend is not on default port):
   - Edit `frontend/src/environments/environment.ts`
   - Update `apiBaseUrl` to match your backend URL (default: `http://localhost:5001/api`)

3. **Start development server**:
```bash
cd frontend
npm start
# or
ng serve
```

4. **Access the application**:
   - **Frontend**: http://localhost:4200

The development server will automatically reload when you make changes.

### Building for Production

#### Backend

```bash
cd backend
dotnet publish TaskManager.API/TaskManager.API.csproj -c Release -o ./publish
```

#### Frontend

```bash
cd frontend
npm run build
```

The production build will be in `frontend/dist/taskmanager-frontend/browser/`

### Environment Configuration

#### Backend (`appsettings.json`)

Required configuration:
- `ConnectionStrings:DefaultConnection` - PostgreSQL connection string
- `Jwt:SecretKey` - Long random string (32+ characters) for JWT signing
- `Jwt:Issuer` - JWT issuer name
- `Jwt:Audience` - JWT audience name
- `Jwt:ExpiryMinutes` - Token expiration time
- `Auth:PasswordSalt` - Strong random string for password hashing

#### Frontend (`environment.ts`)

- `apiBaseUrl` - Backend API base URL (default: `http://localhost:5001/api`)

### Default Login Credentials

On first run (with seed data):
- **Email**: `admin@taskmanager.local`
- **Password**: `Admin@123`

You can create additional users via the registration page.

