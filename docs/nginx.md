Nginx & API Flow – Short Doc
1. Local dev (no Docker frontend)
Setup
Backend: dotnet run → http://localhost:8080
Frontend: ng serve → http://localhost:4200
environment.development.ts / environment.ts:
apiBaseUrl = 'http://localhost:8080/api'
Flow
Browser (localhost:4200) → API directly at http://localhost:8080/api/...
Nginx not used in this mode.


2. Local Docker (full stack via docker-compose up)
Containers
taskmanager-api (ASP.NET) → listens on 8080, published as 8080:8080
taskmanager-frontend (nginx + built Angular) → published as 4200:80
Angular build (Dockerfile)
At build time: ARG API_URL=/api → environment.ts gets apiBaseUrl = '/api'
Nginx config (nginx.conf.template + docker-entrypoint.sh)
Env var: API_BACKEND_URL (set in docker-compose.yml to http://taskmanager-api:8080)
On container start, entrypoint generates default.conf with:
    location /api/ {        proxy_pass http://taskmanager-api:8080/api/;        ...    }
Flow
Browser → http://localhost:4200 (nginx serves Angular)
Angular calls /api/... (same origin: localhost:4200/api/...)
Nginx reverse‑proxies /api/... → http://taskmanager-api:8080/api/... inside Docker network.


3. Production (Railway API)
Goal
Keep using same frontend image, only change where /api is proxied.
Runtime env (frontend container in prod)
Set:
API_BACKEND_URL=https://exquisite-analysis-production-1b41.up.railway.app
Generated nginx config
Entry script substitutes:
    location /api/ {        proxy_pass https://exquisite-analysis-production-1b41.up.railway.app/api/;        ...    }
Flow
Browser → your frontend URL (e.g. https://your-frontend.example.com)
Angular still calls /api/... (relative).
Nginx reverse‑proxies /api/... →
https://exquisite-analysis-production-1b41.up.railway.app/api/... (Railway API).


4. Reverse proxy summary
Angular always uses environment.apiBaseUrl = '/api' in Docker builds.
Nginx in the frontend container acts as a reverse proxy:
Accepts /api/... from the browser.
Forwards them to the real backend:
Local Docker: http://taskmanager-api:8080/api/...
Prod: https://exquisite-analysis-production-1b41.up.railway.app/api/...
The only thing that changes between local Docker and prod is API_BACKEND_URL, not the frontend code.
