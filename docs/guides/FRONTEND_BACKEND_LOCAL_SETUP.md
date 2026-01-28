# Frontend-Backend Local Communication Guide

## Understanding the URL Formation Issue

### The Problem
When you see `http://taskmanager-api:8080/api/auth/login` instead of `http://localhost:5001/api/auth/login`, it means your frontend application is using a Docker container's internal network hostname instead of your local machine's hostname.

### How URLs Are Formed in This Project

#### 1. **Environment Configuration Files**

The frontend uses environment files to configure the API base URL:

- **`frontend/src/environments/environment.ts`** - Default environment
- **`frontend/src/environments/environment.development.ts`** - Development environment

Both currently contain:
```typescript
export const environment = {
  production: false,
  apiBaseUrl: 'http://localhost:5001/api'
};
```

#### 2. **Service Layer Usage**

The `AuthService` (and `TaskService`) use this environment variable:

```typescript
// frontend/src/app/core/services/auth.service.ts
private baseUrl = `${environment.apiBaseUrl}/auth`;

login(request: LoginRequest): Observable<AuthResponse> {
  return this.http.post<AuthResponse>(`${this.baseUrl}/login`, request);
}
```

**URL Formation Flow:**
1. `environment.apiBaseUrl` = `'http://localhost:5001/api'`
2. `baseUrl` = `'http://localhost:5001/api/auth'`
3. Final URL = `'http://localhost:5001/api/auth/login'`

#### 3. **Docker Build-Time Configuration**

When building the frontend Docker image, the `Dockerfile` modifies the environment file:

```dockerfile
# frontend/Dockerfile (lines 15-20)
ARG API_URL=http://taskmanager-api:8080/api
ENV API_URL=$API_URL

# This replaces apiBaseUrl at build time
RUN sed -i "s|apiBaseUrl:.*|apiBaseUrl: '${API_URL}',|g" src/environments/environment.ts
```

**Why `taskmanager-api`?**
- `taskmanager-api` is the **Docker service name** defined in `docker-compose.yml`
- Docker containers can communicate using service names within the same Docker network
- This works **only inside Docker's internal network**, not from your browser

---

## Complete Guide: Frontend Calls Backend Locally

### Scenario 1: Running Everything Locally (No Docker)

**Setup:**
1. Backend runs on: `http://localhost:5001` (or `http://localhost:5000`)
2. Frontend runs on: `http://localhost:4200` (Angular dev server)

**Configuration:**
- Use `environment.development.ts` with:
  ```typescript
  apiBaseUrl: 'http://localhost:5001/api'
  ```

**How It Works:**
```
Browser (localhost:4200) 
  → HTTP Request to http://localhost:5001/api/auth/login
  → Backend API (localhost:5001)
  → Response back to Browser
```

**To Run:**
```bash
# Terminal 1: Start Backend
cd TaskManager/backend/TaskManager.API
dotnet run

# Terminal 2: Start Frontend
cd TaskManager/frontend
npm start
# or
ng serve
```

---

### Scenario 2: Running Frontend Locally, Backend in Docker

**Setup:**
1. Backend in Docker: `http://localhost:5001` (mapped from container port 8080)
2. Frontend locally: `http://localhost:4200`

**Configuration:**
- Use `environment.development.ts` with:
  ```typescript
  apiBaseUrl: 'http://localhost:5001/api'
  ```

**How It Works:**
```
Browser (localhost:4200) 
  → HTTP Request to http://localhost:5001/api/auth/login
  → Docker port mapping (localhost:5001 → container:8080)
  → Backend API Container
  → Response back to Browser
```

**To Run:**
```bash
# Terminal 1: Start Backend in Docker
cd TaskManager/docker
docker-compose up taskmanager-api taskmanager-db

# Terminal 2: Start Frontend locally
cd TaskManager/frontend
npm start
```

---

### Scenario 3: Running Everything in Docker

**Setup:**
1. Backend in Docker: Service name `taskmanager-api` on port `8080` (internal)
2. Frontend in Docker: `http://localhost:4200` (mapped from container port 80)

**Configuration:**
- The Dockerfile **builds** the frontend with:
  ```typescript
  apiBaseUrl: 'http://taskmanager-api:8080/api'
  ```
- This works because both containers are in the same Docker network

**How It Works:**
```
Browser (localhost:4200) 
  → Frontend Container (serves Angular app)
  → Angular app makes HTTP request to http://taskmanager-api:8080/api/auth/login
  → Docker network resolves 'taskmanager-api' to backend container
  → Backend API Container
  → Response back to Frontend Container
  → Response back to Browser
```

**The Problem:**
- When the browser loads the Angular app, it executes JavaScript
- The JavaScript tries to make HTTP requests to `http://taskmanager-api:8080/api/auth/login`
- The browser **cannot resolve** `taskmanager-api` because it's not a real hostname on your machine
- The browser only knows `localhost` or actual domain names

**To Run:**
```bash
cd TaskManager/docker
docker-compose up
```

---

## Why You're Seeing `taskmanager-api:8080`

### Root Cause Analysis

1. **You're running the frontend from a Docker container** that was built with `API_URL=http://taskmanager-api:8080/api`
2. **The Angular app is served from Docker**, so the JavaScript code has `apiBaseUrl: 'http://taskmanager-api:8080/api'`
3. **Your browser is trying to resolve `taskmanager-api`** as a hostname, which doesn't exist on your local machine
4. **The browser can't reach the backend** because it's looking for a hostname that only exists inside Docker's network

### Visual Representation

```
┌─────────────────────────────────────────────────────────┐
│ Your Local Machine                                       │
│                                                          │
│  ┌──────────────┐         ┌──────────────┐             │
│  │   Browser    │         │   Docker     │             │
│  │              │         │   Network    │             │
│  │ localhost:   │         │              │             │
│  │ 4200         │         │  ┌────────┐ │             │
│  │              │         │  │Frontend│ │             │
│  │ ❌ Cannot    │         │  │Container│ │             │
│  │ resolve      │         │  │         │ │             │
│  │ taskmanager- │         │  │ API URL:│ │             │
│  │ api          │         │  │ taskman │ │             │
│  │              │         │  │ ger-api │ │             │
│  └──────────────┘         │  └────────┘ │             │
│                           │      │       │             │
│                           │      │ ✅    │             │
│                           │      │ Can   │             │
│                           │      │ reach │             │
│                           │  ┌───▼────┐ │             │
│                           │  │Backend │ │             │
│                           │  │Container│ │             │
│                           │  │(taskman │ │             │
│                           │  │ ger-api)│ │             │
│                           │  └────────┘ │             │
│                           └──────────────┘             │
└─────────────────────────────────────────────────────────┘
```

---

## Solutions for Local Development

### Solution 1: Run Frontend Locally (Recommended for Development)

**Best for:** Active development, hot reload, debugging

1. **Keep backend in Docker** (or run locally):
   ```bash
   cd TaskManager/docker
   docker-compose up taskmanager-api taskmanager-db
   ```

2. **Run frontend locally**:
   ```bash
   cd TaskManager/frontend
   npm start
   ```

3. **Verify environment file** has:
   ```typescript
   apiBaseUrl: 'http://localhost:5001/api'
   ```

**Result:** Browser can reach `localhost:5001` directly ✅

---

### Solution 2: Use Docker with Correct Port Mapping

**Best for:** Testing Docker setup, production-like environment

1. **Modify docker-compose.yml** to expose backend properly:
   ```yaml
   taskmanager-api:
     ports:
       - "5001:8080"  # Already configured ✅
   ```

2. **Rebuild frontend with localhost URL**:
   ```bash
   cd TaskManager/frontend
   docker build --build-arg API_URL=http://localhost:5001/api -t taskmanager-frontend .
   ```

3. **Or modify Dockerfile build arg** in docker-compose.yml:
   ```yaml
   taskmanager-frontend:
     build:
       context: ../frontend
       dockerfile: Dockerfile
       args:
         API_URL: http://localhost:5001/api  # Use localhost instead
   ```

**Result:** Frontend container uses `localhost:5001`, which browser can resolve ✅

---

### Solution 3: Use Host Network Mode (Advanced)

**Best for:** Docker setup that needs to access host services

Modify `docker-compose.yml`:
```yaml
taskmanager-frontend:
  network_mode: "host"  # Use host network
  # Remove ports mapping (not needed)
```

**Note:** This makes the container use your host's network directly.

---

## URL Resolution Summary

| Scenario | Frontend Location | API URL in Code | Browser Can Resolve? | Works? |
|----------|------------------|-----------------|---------------------|--------|
| Both Local | Local (npm start) | `localhost:5001` | ✅ Yes | ✅ Yes |
| Frontend Local, Backend Docker | Local (npm start) | `localhost:5001` | ✅ Yes | ✅ Yes |
| Both Docker (current issue) | Docker Container | `taskmanager-api:8080` | ❌ No | ❌ No |
| Both Docker (fixed) | Docker Container | `localhost:5001` | ✅ Yes | ✅ Yes |

---

## Key Takeaways

1. **Environment files** (`environment.ts`, `environment.development.ts`) control the API URL
2. **Docker service names** (`taskmanager-api`) only work inside Docker networks
3. **Browsers** can only resolve `localhost` or real domain names, not Docker service names
4. **For local development**, use `localhost:5001` in your environment files
5. **Docker builds** can override the API URL at build time using build arguments
6. **Production** uses different URLs (like Railway domains) configured during deployment

---

## Quick Reference: Current Configuration

### Environment Files
- **File:** `frontend/src/environments/environment.ts`
- **Current:** `apiBaseUrl: 'http://localhost:5001/api'` ✅
- **File:** `frontend/src/environments/environment.development.ts`
- **Current:** `apiBaseUrl: 'http://localhost:5001/api'` ✅

### Docker Configuration
- **Dockerfile Build Arg:** `API_URL=http://taskmanager-api:8080/api` (for Docker-to-Docker communication)
- **Docker Compose Service:** `taskmanager-api` (internal Docker hostname)
- **Port Mapping:** `5001:8080` (host:container)

### Backend Configuration
- **Local Development:** `http://localhost:5001` or `http://localhost:5000`
- **Docker Internal:** `http://taskmanager-api:8080`
- **Docker Exposed:** `http://localhost:5001` (via port mapping)

---

## Troubleshooting

### Issue: Seeing `taskmanager-api:8080` in browser
**Cause:** Frontend was built with Docker using the Docker service name
**Fix:** Run frontend locally OR rebuild Docker image with `API_URL=http://localhost:5001/api`

### Issue: CORS errors
**Cause:** Backend not configured to accept requests from frontend origin
**Fix:** Check backend CORS configuration in `Program.cs`

### Issue: Connection refused
**Cause:** Backend not running or wrong port
**Fix:** Verify backend is running on `localhost:5001` (check `launchSettings.json`)

### Issue: 404 Not Found
**Cause:** API route doesn't exist or wrong base path
**Fix:** Verify backend routes match `/api/auth/login` pattern

---

## Next Steps

1. **For local development:** Use Solution 1 (frontend locally, backend in Docker)
2. **Check your current setup:** Are you running frontend from Docker or locally?
3. **Verify environment file:** Ensure it has `localhost:5001` for local development
4. **Test the connection:** Open browser DevTools → Network tab → Check API requests
