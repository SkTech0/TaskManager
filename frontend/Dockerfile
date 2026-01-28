# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files
COPY package.json package-lock.json* ./

# Install dependencies
RUN npm ci

# Copy source code
COPY . .

# Build argument: use relative /api so browser hits same origin; nginx proxies to taskmanager-api:8080
ARG API_URL=/api
ENV API_URL=$API_URL

# Update environment.ts with API URL at build time (relative URL for Docker - nginx proxies to backend)
RUN sed -i "s|apiBaseUrl:.*|apiBaseUrl: '${API_URL}',|g" src/environments/environment.ts && \
    sed -i "s|production:.*|production: true,|g" src/environments/environment.ts

# Build the application
RUN npm run build

# Production stage
FROM nginx:1.25-alpine

# Copy built app from build stage (Angular 18 outputs to browser subdirectory)
COPY --from=build /app/dist/taskmanager-frontend/browser /usr/share/nginx/html

# Nginx template: __API_BACKEND__ replaced at runtime by API_BACKEND_URL (local: taskmanager-api:8080, prod: Railway URL)
COPY nginx.conf.template /etc/nginx/conf.d/default.conf.template

COPY docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

EXPOSE 80

ENTRYPOINT ["/docker-entrypoint.sh"]
