# Railway Port 8080 Configuration Guide

This guide explains how to configure port 8080 for your Task Manager API on Railway.

## ✅ Code is Already Configured

The application code is already set up to:
- Listen on `0.0.0.0` (all interfaces, not just localhost)
- Use the `PORT` environment variable (Railway provides this)
- Default to port 8080 if PORT is not set

## Railway Dashboard Configuration

### Step 1: Set Custom Port in Railway

1. Go to your Railway project dashboard
2. Click on your **API service** (the .NET backend)
3. Go to **Settings** tab
4. Scroll to **Networking** section
5. Set **Custom Port** to `8080`
6. Save changes

Railway will automatically:
- Set the `PORT` environment variable to `8080`
- Route traffic from your public URL to port 8080

### Step 2: Verify Environment Variables

In your API service, ensure these environment variables are set:

```
PORT=8080
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=<your-postgres-url>
Jwt__SecretKey=<your-secret>
Auth__PasswordSalt=<your-salt>
Jwt__Issuer=TaskManager
Jwt__Audience=TaskManagerClient
Jwt__ExpiryMinutes=60
FrontendUrl=https://your-frontend-url.up.railway.app
```

**Note**: Railway may automatically set `PORT`, but it's good to explicitly set it.

## How It Works

### In Code (Program.cs):

```csharp
// Configure port from environment variable (Railway provides PORT)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");
```

This ensures:
- ✅ Uses Railway's `PORT` environment variable
- ✅ Listens on `0.0.0.0` (all network interfaces)
- ✅ Defaults to 8080 if PORT is not set

### In Dockerfile:

```dockerfile
ENV ASPNETCORE_URLS=http://+:8080
ENV PORT=8080
```

The `+` in `http://+:8080` means "listen on all interfaces".

## Troubleshooting 502 Errors

If you see a 502 Bad Gateway error:

### 1. Check Port Configuration

✅ **Railway Dashboard**:
- Settings → Networking → Custom Port = `8080`

✅ **Environment Variables**:
- `PORT=8080` is set

### 2. Check Application Logs

In Railway dashboard:
- Go to your API service
- Click **Deployments** → **View Logs**
- Look for:
  - `Now listening on: http://0.0.0.0:8080`
  - Any port binding errors

### 3. Verify Service is Running

Check logs for:
- ✅ "Application started"
- ✅ "Now listening on: http://0.0.0.0:8080"
- ❌ "Address already in use" (port conflict)
- ❌ "Permission denied" (port binding issue)

### 4. Common Issues

**Issue**: Application not listening on correct port
- **Fix**: Ensure `PORT=8080` environment variable is set

**Issue**: Application listening on localhost instead of 0.0.0.0
- **Fix**: Code already uses `0.0.0.0`, but verify in logs

**Issue**: Port mismatch between Railway and application
- **Fix**: Set Custom Port to `8080` in Railway settings

## Testing

After deployment:

1. **Check API Health**:
   ```
   curl https://your-api.up.railway.app/swagger
   ```

2. **Check Port in Logs**:
   - Railway Dashboard → API Service → Logs
   - Should see: `Now listening on: http://0.0.0.0:8080`

3. **Test API Endpoint**:
   ```
   curl https://your-api.up.railway.app/api/tasks
   ```

## Quick Checklist

Before deploying:
- [ ] Custom Port set to `8080` in Railway Settings → Networking
- [ ] `PORT=8080` in environment variables (optional, Railway sets it automatically)
- [ ] `FrontendUrl` set to your frontend URL (for CORS)
- [ ] Database connection string configured
- [ ] JWT secrets configured

After deploying:
- [ ] Check logs show: `Now listening on: http://0.0.0.0:8080`
- [ ] Swagger UI accessible: `https://your-api.up.railway.app/swagger`
- [ ] API responds to requests
- [ ] No 502 errors

## Additional Notes

- Railway automatically provides the `PORT` environment variable
- The application code reads `PORT` and uses it
- If `PORT` is not set, it defaults to 8080
- Always listen on `0.0.0.0`, never `localhost` or `127.0.0.1` in production
- Railway maps your public domain to the port you specify

## Need Help?

- Check Railway logs: Dashboard → Service → Logs
- Verify environment variables: Settings → Variables
- Test locally with Docker: `docker-compose up` (uses port 8080)
