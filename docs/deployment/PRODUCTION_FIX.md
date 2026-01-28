# Production CORS Fix

If you're getting CORS errors in production, follow these steps:

## Quick Fix

### Step 1: Set FrontendUrl in Backend API Service

1. Go to your **API service** in Railway
2. Click **Variables** tab
3. Add or update:
   ```
   FrontendUrl=https://taskmanager-production-9b1e.up.railway.app
   ```
   (Replace with your actual frontend URL)

4. **Redeploy** the API service

### Step 2: Update Frontend API URL

1. Go to your **Frontend service** in Railway
2. Go to **Settings** → **Build**
3. Add build argument:
   ```
   API_URL=https://exquisite-analysis-production-1b41.up.railway.app/api
   ```
   (Replace with your actual API URL)

4. **Redeploy** the frontend service

## Complete Environment Variables

### Backend API Service Variables:
```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=<your-postgres-url>
Jwt__SecretKey=<your-secret>
Auth__PasswordSalt=<your-salt>
Jwt__Issuer=TaskManager
Jwt__Audience=TaskManagerClient
Jwt__ExpiryMinutes=60
PORT=8080
FrontendUrl=https://taskmanager-production-9b1e.up.railway.app
```

### Frontend Service Build Arguments:
```
API_URL=https://exquisite-analysis-production-1b41.up.railway.app/api
```

## Verify

After redeploying both services:

1. Check API logs - should see: `CORS allowed origins: http://localhost:4200, https://taskmanager-production-9b1e.up.railway.app`
2. Try logging in from frontend
3. CORS error should be resolved

## Troubleshooting

### Still getting CORS errors?

1. **Check FrontendUrl is set correctly**:
   - Must match exactly (including https:// and no trailing slash)
   - Check in API service Variables

2. **Check API logs**:
   - Should show allowed origins in startup logs
   - Verify your frontend URL is in the list

3. **Verify URLs match**:
   - Frontend URL in `FrontendUrl` variable = Your actual frontend URL
   - API URL in frontend build = Your actual API URL

4. **Redeploy both services** after making changes
