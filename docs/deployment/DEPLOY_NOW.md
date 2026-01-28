# 🚀 Quick Deployment Guide

Your code is now on GitHub! Here's how to deploy it live:

## Option 1: Railway (Recommended - Easiest) ⭐

Railway is the easiest way to deploy your full-stack app.

### Steps:

1. **Go to Railway**: https://railway.app
2. **Sign up/Login** with GitHub
3. **Create New Project** → "Deploy from GitHub repo"
4. **Select your repository**: `SkTech0/TaskManager`

### Deploy Backend:

1. **Add PostgreSQL**:
   - Click "New" → "Database" → "PostgreSQL"
   - Railway will create a database automatically

2. **Add .NET Service**:
   - Click "New" → "GitHub Repo" → Select `TaskManager`
   - Set **Root Directory**: `backend`
   - **IMPORTANT**: Railway should auto-detect Docker (via `railway.json`)
   - If not, go to **Settings** → **Build**:
     - Change **Builder** from `Nixpacks` to `Dockerfile`
     - Set **Dockerfile Path**: `TaskManager.API/Dockerfile`
     - Set **Docker Context**: `backend` (or leave empty)
   - **DO NOT** set Build Command or Start Command (Docker handles this)
   - Go to **Settings** → **Networking** → Set **Custom Port** to `8080`

3. **Configure Port**:
   - Go to your API service → **Settings** → **Networking**
   - Set **Custom Port** to `8080`
   - Railway will automatically set the `PORT` environment variable

4. **Link Database Service** (IMPORTANT!):
   - Go to API service → **Settings** → **Service Dependencies**
   - Click **"Add Dependency"** or **"Link Service"**
   - Select your **PostgreSQL database service**
   - **Save** changes
   - This allows Railway to automatically provide connection variables

5. **Add Environment Variables**:
   
   **Connection String** (choose one):
   - **Option A (Recommended)**: Use Railway's auto-generated variable:
     ```
     ConnectionStrings__DefaultConnection=${DATABASE_URL}
     ```
   - **Option B**: Manual connection string:
     ```
     ConnectionStrings__DefaultConnection=<railway-postgres-url>
     ```
     Get from: PostgreSQL service → "Connect" → Copy connection string
   
   **Other Required Variables:**
   ```
   Jwt__SecretKey=<generate-32-char-secret>
   Auth__PasswordSalt=<generate-salt>
   Jwt__Issuer=TaskManager
   Jwt__Audience=TaskManagerClient
   Jwt__ExpiryMinutes=60
   ASPNETCORE_ENVIRONMENT=Production
   PORT=8080
   FrontendUrl=https://your-frontend-url.up.railway.app
   ```
   
   **To generate secrets**: Run `./scripts/generate-secrets.sh` or use Railway's "Generate" button
   
   **Important**: 
   - Link services first (Step 4) to get `DATABASE_URL` automatically
   - Set `FrontendUrl` after deploying frontend (for CORS)
   - Code automatically converts PostgreSQL URL format (`postgresql://...`) to Npgsql format

### Deploy Frontend:

1. **Add Static Site**:
   - Click "New" → "GitHub Repo" → Select `TaskManager`
   - Set **Root Directory**: `frontend`
   - Set **Build Command**: `npm install && npm run build`
   - Set **Output Directory**: `dist/taskmanager-frontend/browser`

2. **Update API URL**:
   - Edit `frontend/src/environments/environment.ts`
   - Set `apiBaseUrl` to your Railway API URL (e.g., `https://taskmanager-api.up.railway.app/api`)
   - Commit and push the change

3. **Redeploy Frontend** after updating the API URL

### Get Your URLs:

- Railway provides public URLs automatically
- Backend: `https://your-api-name.up.railway.app`
- Frontend: `https://your-frontend-name.up.railway.app`
- API Swagger: `https://your-api-name.up.railway.app/swagger`

### Fixing 502 Errors:

If you get a 502 error:
1. ✅ Verify **Port 8080** is set in **Settings** → **Networking** → **Custom Port**
2. ✅ Check that `PORT=8080` is in your environment variables
3. ✅ Ensure the application is listening on `0.0.0.0:8080` (code already configured)
4. ✅ Check service logs in Railway dashboard for connection errors

---

## Option 2: Render (Free Tier Available)

### Steps:

1. **Go to Render**: https://render.com
2. **Sign up/Login** with GitHub
3. **New** → **Blueprint** → Connect your repo

Or manually:

### Deploy Database:

1. **New** → **PostgreSQL**
2. Name: `taskmanager-db`
3. Plan: Free
4. Create database

### Deploy Backend:

1. **New** → **Web Service**
2. Connect GitHub repo: `SkTech0/TaskManager`
3. Settings:
   - **Name**: `taskmanager-api`
   - **Environment**: `Docker`
   - **Dockerfile Path**: `backend/TaskManager.API/Dockerfile`
   - **Docker Context**: `backend`
4. **Add Environment Variables**:
   ```
   ConnectionStrings__DefaultConnection=<render-postgres-url>
   Jwt__SecretKey=<your-secret>
   Auth__PasswordSalt=<your-salt>
   Jwt__Issuer=TaskManager
   Jwt__Audience=TaskManagerClient
   Jwt__ExpiryMinutes=60
   ```
5. **Create Web Service**

### Deploy Frontend:

1. **New** → **Static Site**
2. Connect GitHub repo: `SkTech0/TaskManager`
3. Settings:
   - **Build Command**: `cd frontend && npm install && npm run build`
   - **Publish Directory**: `frontend/dist/taskmanager-frontend/browser`
4. **Add Environment Variable**:
   ```
   NODE_ENV=production
   ```
5. Update `frontend/src/environments/environment.ts` with Render API URL
6. **Create Static Site**

---

## Option 3: Vercel (Frontend) + Railway (Backend)

### Deploy Backend on Railway:
Follow Option 1 above for backend.

### Deploy Frontend on Vercel:

1. **Go to Vercel**: https://vercel.com
2. **Sign up/Login** with GitHub
3. **Add New Project** → Import `SkTech0/TaskManager`
4. **Configure**:
   - **Framework Preset**: Other
   - **Root Directory**: `frontend`
   - **Build Command**: `npm run build`
   - **Output Directory**: `dist/taskmanager-frontend/browser`
5. **Add Environment Variable**:
   ```
   NODE_ENV=production
   ```
6. Update `frontend/src/environments/environment.ts` with Railway API URL
7. **Deploy**

---

## After Deployment

### 1. Update Frontend API URL

Edit `frontend/src/environments/environment.ts`:

```typescript
export const environment = {
  production: true,
  apiBaseUrl: 'https://your-deployed-api-url.com/api'
};
```

Commit and push:
```bash
git add frontend/src/environments/environment.ts
git commit -m "Update API URL for production"
git push
```

### 2. Test Your Deployment

- ✅ Frontend loads: `https://your-frontend-url.com`
- ✅ Login works: Use `admin@taskmanager.local` / `Admin@123`
- ✅ API Swagger: `https://your-api-url.com/swagger`
- ✅ Create/Edit tasks works

### 3. Update README

Add your live demo links to `README.md`:

```markdown
## Live Demo

- **Frontend**: https://your-frontend-url.com
- **API Swagger**: https://your-api-url.com/swagger
```

---

## Quick Commands

### Generate Secrets (if needed):
```bash
# Linux/macOS
./scripts/generate-secrets.sh

# Windows
.\scripts\generate-secrets.ps1
```

### Update and Redeploy:
```bash
git add .
git commit -m "Your changes"
git push
# Platform will auto-deploy
```

---

## Troubleshooting

### API Not Starting
- Check environment variables are set
- Verify database connection string
- Check logs in platform dashboard

### Frontend Can't Connect to API
- Verify `apiBaseUrl` in `environment.ts`
- Check CORS settings in backend
- Ensure API is accessible

### Database Connection Issues
- Verify connection string format
- Check database is running
- Ensure network connectivity

---

## Recommended: Railway

**Why Railway?**
- ✅ Easiest setup
- ✅ Auto-deploys on git push
- ✅ Free tier available
- ✅ Handles all services (DB, API, Frontend)
- ✅ Automatic HTTPS
- ✅ Great documentation

**Start here**: https://railway.app → Deploy from GitHub → Select `SkTech0/TaskManager`

---

## Need Help?

- Check platform-specific logs
- See [DEPLOYMENT.md](./docs/DEPLOYMENT.md) for detailed guides
- Check [QUICKSTART.md](./docs/QUICKSTART.md) for local setup
