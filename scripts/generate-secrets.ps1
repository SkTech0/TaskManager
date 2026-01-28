# PowerShell script to generate secure secrets for Task Manager
# Usage: .\scripts\generate-secrets.ps1

Write-Host "Generating secure secrets for Task Manager..." -ForegroundColor Green
Write-Host ""

# Generate JWT Secret Key (32 bytes)
$jwtSecret = [Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))
Write-Host "JWT Secret Key:" -ForegroundColor Yellow
Write-Host $jwtSecret
Write-Host ""

# Generate Password Salt (24 bytes)
$passwordSalt = [Convert]::ToBase64String((1..24 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))
Write-Host "Password Salt:" -ForegroundColor Yellow
Write-Host $passwordSalt
Write-Host ""

# Generate Database Password (16 bytes)
$dbPassword = -join ((48..57) + (65..90) + (97..122) | Get-Random -Count 16 | ForEach-Object {[char]$_})
Write-Host "Database Password (optional):" -ForegroundColor Yellow
Write-Host $dbPassword
Write-Host ""

Write-Host "Copy these values to your configuration files:" -ForegroundColor Cyan
Write-Host "- backend/TaskManager.API/appsettings.json"
Write-Host "- docker/docker-compose.yml"
Write-Host ""
Write-Host "⚠️  Keep these secrets secure and never commit them to Git!" -ForegroundColor Red
