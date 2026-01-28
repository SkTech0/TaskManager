# 🚀 Railway Deployment Steps (Updated for Docker)

Complete step-by-step guide to deploy Task Manager on Railway using Docker.

## Prerequisites

- GitHub account with repository: `SkTech0/TaskManager`
- Railway account (sign up at https://railway.app)

---

## Step 1: Create Railway Project

1. Go to https://railway.app
2. Sign up/Login with GitHub
3. Click **"New Project"**
4. Select **"Deploy from GitHub repo"**
5. Select your repository: `SkTech0/TaskManager`
6. Click **"Deploy Now"**

---

## Step 2: Deploy PostgreSQL Database

1. In your Railway project, click **"New"**
2. Select **"Database"** → **"PostgreSQL"**
3. Railway will automatically:
   - Create the database
   - Generate connection credentials
   - Provide connection URL

4. **Copy the connection string**:
   - Click on the PostgreSQL service
   - Go to **"Connect"** tab
   - Copy the **"Postgres Connection URL"** (you'll need this for the API)

---

## Step 3: Deploy Backend API (Using Docker)

### 3.1 Add .NET Service

1. Click **"New"** → **"GitHub Repo"**
2. Select `TaskManager` repository
3. Railway will create a new service

### 3.2 Configure Service Settings

1. Click on the newly created service
2. Go to **Settings** tab

#### Root Directory:
- Set **Root Directory**: `backend`

#### Build Configuration (IMPORTANT):
- Go to **Build** section
- **Builder**: Should be `Dockerfile` (Railway auto-detects from `railway.json`)
- If it shows `Nixpacks` or `Railpack`:
  - Change to **`Dockerfile`**
  - Set **Dockerfile Path**: `TaskManager.API/Dockerfile`
  - Set **Docker Context**: `backend` (or leave empty if root is backend)

**⚠️ DO NOT SET:**
- ❌ Build Command (Docker handles this)
- ❌ Start Command (Docker handles this via ENTRYPOINT)

#### Port Configuration:
- Go to **Networking** section
- Set **Custom Port**: `8080`
- Railway will automatically set `PORT=8080` environment variable

### 3.3 Add Environment Variables

Go to **Variables** tab and add:

```
ConnectionStrings__DefaultConnection=<paste-postgres-url-from-step-2>
Jwt__SecretKey=<generate-32-char-secret>
Auth__PasswordSalt=<generate-salt>
Jwt__Issuer=TaskManager
Jwt__Audience=TaskManagerClient
Jwt__ExpiryMinutes=60
ASPNETCORE_ENVIRONMENT=Production
PORT=8080
FrontendUrl=https://your-frontend-url.up.railway.app
```

**How to get values:**

1. **PostgreSQL URL**: 
   - From Step 2, copy the connection string
   - Format: `postgresql://user:password@host:port/database`

2. **Generate Secrets**:
   - Option A: Use Railway's "Generate" button for `Jwt__SecretKey` and `Auth__PasswordSalt`
   - Option B: Run locally:
     ```bash
     ./scripts/generate-secrets.sh
     ```

3. **FrontendUrl**: 
   - You'll get this after deploying frontend (Step 4)
   - Can update later if needed

### 3.4 Deploy

1. Railway will automatically start building
2. Watch the **Deployments** tab for build progress
3. Wait for build to complete (should see "Build successful")
4. Check **Logs** tab - should see:
   ```
   Now listening on: http://0.0.0.0:8080
   Database initialized and seeded successfully.
   ```

### 3.5 Get API URL

1. Go to **Settings** → **Networking**
2. Copy the **Public Domain** URL (e.g., `https://taskmanager-api.up.railway.app`)
3. **Save this URL** - you'll need it for the frontend

---

## Step 4: Deploy Frontend

### 4.1 Add Static Site Service

1. Click **"New"** → **"GitHub Repo"**
2. Select `TaskManager` repository
3. Railway will create a new service

### 4.2 Configure Service Settings

1. Click on the frontend service
2. Go to **Settings** tab

#### Root Directory:
- Set **Root Directory**: `frontend`

#### Build Configuration:
- **Build Command**: `npm install && npm run build`
- **Output Directory**: `dist/taskmanager-frontend/browser`

#### Environment Variables:
Add:
```
NODE_ENV=production
```

### 4.3 Update Frontend API URL

**Before deploying**, update the API URL:

1. In your local repository, edit `frontend/src/environments/environment.ts`:
   ```typescript
   export const environment = {
     production: true,
     apiBaseUrl: 'https://your-api-url.up.railway.app/api'
   };
   ```
   Replace `your-api-url` with the API URL from Step 3.5

2. Commit and push:
   ```bash
   git add frontend/src/environments/environment.ts
   git commit -m "Update API URL for production"
   git push
   ```

3. Railway will auto-redeploy the frontend

### 4.4 Get Frontend URL

1. Go to **Settings** → **Networking**
2. Copy the **Public Domain** URL (e.g., `https://taskmanager-frontend.up.railway.app`)

### 4.5 Update Backend CORS (Important!)

1. Go back to your **API service**
2. Update the `FrontendUrl` environment variable:
   ```
   FrontendUrl=https://your-frontend-url.up.railway.app
   ```
   (Use the URL from Step 4.4)

3. **Redeploy** the API service (or it will auto-redeploy)

---

## Step 5: Verify Deployment

### Test API:
1. Open: `https://your-api-url.up.railway.app/swagger`
2. Should see Swagger UI

### Test Frontend:
1. Open: `https://your-frontend-url.up.railway.app`
2. Should see login page

### Test Login:
- Email: `admin@taskmanager.local`
- Password: `Admin@123`

---

## Troubleshooting

### Backend Build Fails

**Error**: `dotnet: not found`
- **Fix**: Ensure Builder is set to `Dockerfile`, not `Nixpacks`

**Error**: `502 Bad Gateway`
- **Fix**: 
  1. Check Custom Port is set to `8080`
  2. Check `PORT=8080` in environment variables
  3. Check logs for "Now listening on: http://0.0.0.0:8080"

### Frontend Can't Connect to API

**Error**: CORS error
- **Fix**: Update `FrontendUrl` in API environment variables

**Error**: 404 on API calls
- **Fix**: Verify `apiBaseUrl` in `environment.ts` includes `/api` suffix

### Database Connection Issues

**Error**: Connection refused
- **Fix**: 
  1. Verify PostgreSQL service is running
  2. Check connection string format
  3. Ensure database service is in same project

---

## Quick Reference

### Backend Service:
- **Root Directory**: `backend`
- **Builder**: `Dockerfile`
- **Dockerfile Path**: `TaskManager.API/Dockerfile`
- **Port**: `8080`
- **No Build/Start Commands** (Docker handles it)

### Frontend Service:
- **Root Directory**: `frontend`
- **Build Command**: `npm install && npm run build`
- **Output Directory**: `dist/taskmanager-frontend/browser`

### Required Environment Variables (Backend):
```
ConnectionStrings__DefaultConnection=<postgres-url>
Jwt__SecretKey=<32-char-secret>
Auth__PasswordSalt=<salt>
Jwt__Issuer=TaskManager
Jwt__Audience=TaskManagerClient
Jwt__ExpiryMinutes=60
ASPNETCORE_ENVIRONMENT=Production
PORT=8080
FrontendUrl=<frontend-url>
```

---

## Summary of Changes from Old Instructions

✅ **Removed**:
- Build Command: `dotnet publish...` (Docker handles this)
- Start Command: `dotnet TaskManager.API.dll` (Docker handles this)

✅ **Added**:
- Dockerfile configuration in Settings → Build
- Explicit Docker builder selection
- Port configuration in Settings → Networking

✅ **Kept**:
- Root Directory: `backend`
- Environment variables
- Port: `8080`

---

## Next Steps

1. ✅ Deploy backend and database
2. ✅ Deploy frontend
3. ✅ Update API URL in frontend
4. ✅ Update FrontendUrl in backend
5. ✅ Test the application
6. ✅ Share your live URLs!

---

## Need Help?

- See `RAILWAY_DOCKER_FIX.md` for Docker troubleshooting
- See `RAILWAY_PORT_SETUP.md` for port configuration
- Check Railway logs in Deployments tab
