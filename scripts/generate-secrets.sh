#!/bin/bash

# Script to generate secure secrets for Task Manager
# Usage: ./scripts/generate-secrets.sh

echo "Generating secure secrets for Task Manager..."
echo ""

# Generate JWT Secret Key (32 bytes = 44 base64 characters)
JWT_SECRET=$(openssl rand -base64 32)
echo "JWT Secret Key:"
echo "$JWT_SECRET"
echo ""

# Generate Password Salt (24 bytes = 32 base64 characters)
PASSWORD_SALT=$(openssl rand -base64 24)
echo "Password Salt:"
echo "$PASSWORD_SALT"
echo ""

# Generate Database Password (16 bytes = 24 base64 characters)
DB_PASSWORD=$(openssl rand -base64 16 | tr -d "=+/" | cut -c1-16)
echo "Database Password (optional):"
echo "$DB_PASSWORD"
echo ""

echo "Copy these values to your configuration files:"
echo "- backend/TaskManager.API/appsettings.json"
echo "- docker/docker-compose.yml"
echo ""
echo "⚠️  Keep these secrets secure and never commit them to Git!"
