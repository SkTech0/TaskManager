# Fix: Railway Docker Configuration

## Problem

Railway was trying to use Railpack/Nixpacks which doesn't have .NET SDK, causing this error:
```
sh: 1: dotnet: not found
ERROR: failed to build: failed to solve
```

## Solution

Configure Railway to use **Docker** instead of Railpack.

## Steps to Fix

### Option 1: Using Railway Dashboard (Easiest)

1. **Go to your API service** in Railway dashboard
2. Click **Settings** tab
3. Scroll to **Build** section
4. Change **Builder** from `Nixpacks` to `Dockerfile`
5. Set **Dockerfile Path**: `TaskManager.API/Dockerfile`
6. Set **Docker Context**: `backend` (or leave empty if root is backend)
7. **Save** changes
8. **Redeploy** the service

### Option 2: Using railway.json (Automatic)

The repository now includes `railway.json` files that configure Docker:

- **Root `railway.json`**: Points to `backend/TaskManager.API/Dockerfile`
- **Backend `railway.json`**: Points to `TaskManager.API/Dockerfile` (when root directory is `backend`)

Railway should automatically detect these files and use Docker.

### Option 3: Manual Configuration

If the above doesn't work:

1. **Delete the service** and recreate it
2. When adding the service:
   - Select your GitHub repo
   - Set **Root Directory**: `backend`
   - In **Settings** → **Build**:
     - Select **Dockerfile** as builder
     - Dockerfile Path: `TaskManager.API/Dockerfile`
     - Docker Context: `backend` (or leave empty)

## Verify Configuration

After redeploying, check the build logs. You should see:

```
✅ Building Docker image
✅ Using Dockerfile: TaskManager.API/Dockerfile
✅ Successfully built
```

Instead of:
```
❌ Using Railpack/Nixpacks
❌ dotnet: not found
```

## Current Configuration

The `railway.json` files are configured as:

```json
{
  "build": {
    "builder": "DOCKERFILE",
    "dockerfilePath": "TaskManager.API/Dockerfile"
  }
}
```

## Troubleshooting

### Still seeing Railpack?

1. **Clear Railway cache**:
   - Settings → Advanced → Clear Build Cache
   - Redeploy

2. **Check Root Directory**:
   - Should be `backend` (not root)
   - Dockerfile path is relative to root directory

3. **Verify Dockerfile exists**:
   - Path: `backend/TaskManager.API/Dockerfile`
   - Should be committed to GitHub

### Build fails with Docker?

1. **Check Dockerfile syntax**:
   ```bash
   docker build -f backend/TaskManager.API/Dockerfile -t test ./backend
   ```

2. **Check Railway logs**:
   - Go to Deployments → View Logs
   - Look for Docker build errors

3. **Verify context**:
   - Dockerfile uses `COPY TaskManager.sln ./`
   - Context should be `backend` directory

## Expected Build Output

When using Docker, you should see:

```
Step 1/12 : FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
Step 2/12 : WORKDIR /src
Step 3/12 : COPY TaskManager.sln ./
...
Step 12/12 : ENTRYPOINT ["dotnet", "TaskManager.API.dll"]
Successfully built
```

## After Fix

Once Docker is configured:
1. ✅ Build will use .NET SDK from Docker image
2. ✅ Application will be built and deployed
3. ✅ Check logs for: `Now listening on: http://0.0.0.0:8080`

## Need Help?

- Check Railway documentation: https://docs.railway.app/deploy/dockerfiles
- Verify Dockerfile works locally: `docker build -f backend/TaskManager.API/Dockerfile ./backend`
