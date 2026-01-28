# Fix: Railway Database Connection "Name or service not known"

## Problem

Error: `Name or service not known` when trying to connect to PostgreSQL.

This means the API service cannot resolve the database hostname.

## Root Cause

The API service and PostgreSQL service are **not linked** in Railway, or the connection string is incorrect.

## Solution

### Step 1: Link Database Service to API Service

1. **Go to your Railway project dashboard**
2. **Click on your API service** (the .NET backend)
3. Go to **Settings** tab
4. Scroll to **Service Dependencies** section
5. **Click "Add Dependency"** or **"Link Service"**
6. **Select your PostgreSQL database service**
7. **Save** changes

This creates a network link between services and Railway automatically provides connection variables.

### Step 2: Use Railway's Auto-Generated Connection String

After linking, Railway automatically provides these environment variables:

- `PGHOST` - Database host
- `PGPORT` - Database port
- `PGDATABASE` - Database name
- `PGUSER` - Database user
- `PGPASSWORD` - Database password
- `DATABASE_URL` - Full connection string

### Step 3: Update Connection String Format

Railway provides the connection string in PostgreSQL URL format:
```
postgresql://user:password@host:port/database
```

But .NET needs it in Npgsql format:
```
Host=host;Port=port;Database=database;Username=user;Password=password
```

### Option A: Use Railway's Connection String (Recommended)

1. **In your API service**, go to **Variables** tab
2. **Remove** the manually set `ConnectionStrings__DefaultConnection`
3. **Add** this variable:
   ```
   ConnectionStrings__DefaultConnection=${DATABASE_URL}
   ```
   
   **OR** if Railway provides `POSTGRES_URL`:
   ```
   ConnectionStrings__DefaultConnection=${POSTGRES_URL}
   ```

4. **Update Program.cs** to parse the PostgreSQL URL format (see below)

### Option B: Convert PostgreSQL URL to Npgsql Format

If Railway provides `DATABASE_URL` in PostgreSQL format, you need to convert it:

1. **Get the connection string** from PostgreSQL service → "Connect" tab
2. **Convert** from:
   ```
   postgresql://user:password@host:port/database
   ```
   **To**:
   ```
   Host=host;Port=port;Database=database;Username=user;Password=password
   ```

3. **Set** `ConnectionStrings__DefaultConnection` to the converted format

### Option C: Use Individual Environment Variables

Railway provides these when services are linked:

```
ConnectionStrings__DefaultConnection=Host=${PGHOST};Port=${PGPORT};Database=${PGDATABASE};Username=${PGUSER};Password=${PGPASSWORD}
```

## Update Code to Handle PostgreSQL URL Format

If Railway provides `DATABASE_URL` in PostgreSQL URL format, update `Program.cs`:

```csharp
// In Program.cs, before builder.Build()
var connectionString = configuration.GetConnectionString("DefaultConnection");

// If connection string is in PostgreSQL URL format, convert it
if (connectionString?.StartsWith("postgresql://") == true)
{
    var uri = new Uri(connectionString);
    var host = uri.Host;
    var port = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');
    var username = uri.UserInfo.Split(':')[0];
    var password = uri.UserInfo.Split(':').Length > 1 ? uri.UserInfo.Split(':')[1] : "";
    
    connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";
    
    // Update configuration
    configuration["ConnectionStrings:DefaultConnection"] = connectionString;
}
```

## Quick Fix Checklist

- [ ] **Link** PostgreSQL service to API service (Settings → Service Dependencies)
- [ ] **Check** Railway provides `DATABASE_URL` or `POSTGRES_URL` environment variable
- [ ] **Verify** connection string format matches what .NET expects
- [ ] **Redeploy** API service after linking
- [ ] **Check logs** - should see "Database initialized and seeded successfully"

## Verify Connection String

After linking services, check environment variables:

1. Go to API service → **Variables** tab
2. Look for:
   - `DATABASE_URL` or `POSTGRES_URL`
   - `PGHOST`, `PGPORT`, `PGDATABASE`, etc.

3. **Test connection string format**:
   - Should start with `Host=` (Npgsql format)
   - OR be in `postgresql://` format (needs conversion)

## Common Issues

### Issue 1: Services Not Linked
**Symptom**: `Name or service not known`
**Fix**: Link services (Settings → Service Dependencies)

### Issue 2: Wrong Connection String Format
**Symptom**: Connection string has `postgresql://` but .NET needs `Host=...`
**Fix**: Convert format or update code to parse PostgreSQL URL

### Issue 3: Connection String Not Set
**Symptom**: Connection string is empty or null
**Fix**: Set `ConnectionStrings__DefaultConnection` environment variable

### Issue 4: Database Service Not Running
**Symptom**: Connection refused
**Fix**: Ensure PostgreSQL service is running and healthy

## Step-by-Step Fix

1. **Link Services**:
   - API Service → Settings → Service Dependencies
   - Add PostgreSQL service

2. **Check Variables**:
   - API Service → Variables
   - Look for `DATABASE_URL` or `POSTGRES_URL`

3. **Set Connection String**:
   ```
   ConnectionStrings__DefaultConnection=${DATABASE_URL}
   ```
   OR manually convert and set

4. **Redeploy**:
   - API Service → Deployments → Redeploy

5. **Verify**:
   - Check logs for "Database initialized and seeded successfully"

## After Fix

You should see in logs:
```
[INFO] Database initialized and seeded successfully.
[INFO] Now listening on: http://0.0.0.0:8080
```

Instead of:
```
[WRN] Database connection attempt X/10 failed... Error: Name or service not known
```
