# Deployment Guide

This guide covers deploying the Task Manager application to various cloud platforms.

## Pre-Deployment Checklist

1. ✅ Update all secrets in configuration files
2. ✅ Set secure JWT secret key (32+ characters)
3. ✅ Set secure password salt
4. ✅ Update database connection strings
5. ✅ Update frontend API base URL for production
6. ✅ Test locally with Docker before deploying

## Deployment Options

### Option 1: Railway (Recommended for Full Stack)

Railway supports deploying all three services (PostgreSQL, API, Frontend) easily.

#### Steps:

1. **Create Railway Account**: https://railway.app

2. **Install Railway CLI** (optional):
```bash
npm install -g @railway/cli
railway login
```

3. **Deploy Backend**:
   - Create new project in Railway
   - Connect your GitHub repository
   - Add PostgreSQL service (Railway will auto-create)
   - Add .NET service:
     - Set root directory: `backend`
     - Set build command: `dotnet publish TaskManager.API/TaskManager.API.csproj -c Release -o ./publish`
     - Set start command: `dotnet TaskManager.API.dll`
     - Set port: `8080`
   - Add environment variables:
     ```
     ConnectionStrings__DefaultConnection=<railway-postgres-url>
     Jwt__SecretKey=<your-secret-key>
     Auth__PasswordSalt=<your-salt>
     Jwt__Issuer=TaskManager
     Jwt__Audience=TaskManagerClient
     Jwt__ExpiryMinutes=60
     ```

4. **Deploy Frontend**:
   - Add static site service
   - Set root directory: `frontend`
   - Set build command: `npm install && npm run build`
   - Set output directory: `dist/taskmanager-frontend/browser`
   - Update `environment.ts` with Railway API URL

**Railway URLs**: Railway provides public URLs automatically.

### Option 2: Render

#### Backend Deployment:

1. Create new **Web Service** on Render
2. Connect GitHub repository
3. Settings:
   - **Build Command**: `cd backend && dotnet publish TaskManager.API/TaskManager.API.csproj -c Release -o ./publish`
   - **Start Command**: `cd backend/publish && dotnet TaskManager.API.dll`
   - **Environment**: `Docker` or `Production`
4. Add PostgreSQL database (Render provides connection string)
5. Add environment variables (same as Railway)

#### Frontend Deployment:

1. Create new **Static Site** on Render
2. Connect GitHub repository
3. Settings:
   - **Build Command**: `cd frontend && npm install && npm run build`
   - **Publish Directory**: `frontend/dist/taskmanager-frontend/browser`
4. Update `environment.ts` with Render API URL

### Option 3: Vercel (Frontend) + Railway/Render (Backend)

#### Frontend on Vercel:

1. Install Vercel CLI:
```bash
npm install -g vercel
```

2. Deploy:
```bash
cd frontend
vercel
```

3. Configure:
   - Build command: `npm run build`
   - Output directory: `dist/taskmanager-frontend/browser`
   - Update `environment.ts` with your backend URL

#### Backend on Railway/Render:
Follow steps from Option 1 or 2 above.

### Option 4: Docker Compose on VPS

For deploying on your own server (DigitalOcean, AWS EC2, etc.):

1. **Setup Server**:
   - Install Docker and Docker Compose
   - Clone repository
   - Copy `docker-compose.example.yml` to `docker-compose.yml`
   - Update secrets in `docker-compose.yml`

2. **Deploy**:
```bash
cd docker
docker-compose up -d --build
```

3. **Setup Domain** (optional):
   - Use Nginx as reverse proxy
   - Configure SSL with Let's Encrypt

## Environment Variables

### Backend (API)

Required environment variables:

```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Host=your-db-host;Port=5432;Database=taskmanagerdb;Username=user;Password=pass
Jwt__Issuer=TaskManager
Jwt__Audience=TaskManagerClient
Jwt__SecretKey=your-secure-secret-key-32-chars-min
Jwt__ExpiryMinutes=60
Auth__PasswordSalt=your-secure-salt-value
```

### Frontend

Update `frontend/src/environments/environment.ts`:

```typescript
export const environment = {
  production: true,
  apiBaseUrl: 'https://your-api-domain.com/api'
};
```

## Generating Secure Secrets

### Generate JWT Secret Key:

**Using OpenSSL**:
```bash
openssl rand -base64 32
```

**Using Node.js**:
```bash
node -e "console.log(require('crypto').randomBytes(32).toString('base64'))"
```

**Using .NET**:
```bash
dotnet run --project backend/TaskManager.API -- generate-secret
```

### Generate Password Salt:

Use the same methods as above, or any secure random string generator.

## Security Checklist

- [ ] JWT Secret Key is 32+ characters and randomly generated
- [ ] Password Salt is strong and randomly generated
- [ ] Database password is strong
- [ ] Environment variables are set in platform (not in code)
- [ ] HTTPS is enabled for production
- [ ] CORS is configured for your frontend domain only
- [ ] API is not exposed publicly without authentication
- [ ] Database is not publicly accessible

## Post-Deployment

1. **Test Login**: Use default admin credentials
2. **Create Test User**: Register a new user
3. **Test API**: Access Swagger UI at `https://your-api-domain.com/swagger`
4. **Monitor Logs**: Check application logs for errors
5. **Update Documentation**: Update API base URL in frontend if needed

## Troubleshooting

### API Not Starting

- Check database connection string
- Verify environment variables are set
- Check logs: `docker-compose logs taskmanager-api`

### Frontend Can't Connect to API

- Verify `apiBaseUrl` in `environment.ts`
- Check CORS configuration in `Program.cs`
- Ensure API is accessible from frontend domain

### Database Connection Issues

- Verify database is running
- Check connection string format
- Ensure network connectivity between services

## Support

For deployment issues, check:
- Platform-specific documentation
- Application logs
- [Quick Start Guide](./QUICKSTART.md)
- [Build & Run Guide](./build-run.md)
