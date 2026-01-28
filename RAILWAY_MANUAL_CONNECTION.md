# Manual Database Connection Setup for Railway

If Railway doesn't automatically provide database variables, set the connection string manually.

## Option 1: Use DATABASE_URL from PostgreSQL Service

1. Go to your **PostgreSQL service** in Railway
2. Click **"Connect"** tab
3. Copy the **"Postgres Connection URL"** (looks like: `postgresql://postgres:password@host:5432/railway`)

4. Go to your **API service** → **Variables** tab
5. Add:
   ```
   ConnectionStrings__DefaultConnection=<paste-the-connection-url-here>
   ```

## Option 2: Build from Individual Values

From your database details, use these values:

1. Go to **API service** → **Variables** tab
2. Add:
   ```
   ConnectionStrings__DefaultConnection=Host=${{RAILWAY_PRIVATE_DOMAIN}};Port=5432;Database=railway;Username=postgres;Password=KLvAqFlSVnoEvEotbxhKnazCkcpQHlai
   ```

**Note**: Replace `${{RAILWAY_PRIVATE_DOMAIN}}` with the actual private domain from your PostgreSQL service variables.

## Option 3: Use Railway's Private Domain Variable

If Railway provides `RAILWAY_PRIVATE_DOMAIN` in your PostgreSQL service:

1. In **API service** → **Variables**, add:
   ```
   ConnectionStrings__DefaultConnection=postgresql://postgres:KLvAqFlSVnoEvEotbxhKnazCkcpQHlai@${{RAILWAY_PRIVATE_DOMAIN}}:5432/railway
   ```

## Quick Fix: Direct Connection String

Based on your database details, add this to API service Variables:

```
ConnectionStrings__DefaultConnection=postgresql://postgres:KLvAqFlSVnoEvEotbxhKnazCkcpQHlai@<your-postgres-private-domain>:5432/railway
```

Replace `<your-postgres-private-domain>` with the value from your PostgreSQL service's `RAILWAY_PRIVATE_DOMAIN` variable.
