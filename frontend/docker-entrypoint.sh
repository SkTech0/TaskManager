#!/bin/sh
set -e
API_BACKEND="${API_BACKEND_URL:-http://taskmanager-api:8080}"
sed "s|__API_BACKEND__|$API_BACKEND|g" /etc/nginx/conf.d/default.conf.template > /etc/nginx/conf.d/default.conf
exec nginx -g "daemon off;"
