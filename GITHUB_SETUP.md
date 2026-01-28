# GitHub Repository Setup Guide

This guide will help you prepare the Task Manager project for GitHub and make it accessible to everyone.

## Pre-Push Checklist

Before pushing to GitHub, ensure:

### ✅ 1. Secrets Are Not Committed

**IMPORTANT**: Never commit files with real secrets!

Check that these files are in `.gitignore`:
- `backend/TaskManager.API/appsettings.json` (with real secrets)
- `docker/docker-compose.yml` (with real secrets)

**Verify**:
```bash
git status
```

You should NOT see:
- ❌ `appsettings.json` (only `appsettings.example.json` should appear)
- ❌ `docker-compose.yml` (only `docker-compose.example.yml` should appear)

### ✅ 2. Example Files Are Present

These example files should be committed (they're safe):
- ✅ `backend/TaskManager.API/appsettings.example.json`
- ✅ `docker/docker-compose.example.yml`

### ✅ 3. Build Artifacts Are Ignored

Verify `.gitignore` excludes:
- `bin/`, `obj/`, `dist/`, `node_modules/`
- `.angular/`, `.vs/`, `.idea/`

## Step-by-Step GitHub Setup

### Step 1: Initialize Git Repository (if not already done)

```bash
cd /Users/satyamkumar/Downloads/TaskManager
git init
```

### Step 2: Check What Will Be Committed

```bash
git status
```

**If you see `appsettings.json` or `docker-compose.yml` with secrets:**

1. **Remove them from Git tracking** (keeps local files):
```bash
git rm --cached backend/TaskManager.API/appsettings.json
git rm --cached docker/docker-compose.yml
```

2. **Verify they're now ignored**:
```bash
git status
# Should NOT show these files
```

### Step 3: Stage All Safe Files

```bash
git add .
git status
# Review what will be committed
```

### Step 4: Create Initial Commit

```bash
git commit -m "Initial commit: Task Manager application

- Full-stack task management system
- .NET 8 backend with PostgreSQL
- Angular 18 frontend with standalone components
- Docker support with healthchecks
- Complete documentation"
```

### Step 5: Create GitHub Repository

1. Go to https://github.com/new
2. Repository name: `TaskManager` (or your preferred name)
3. Description: "Full-stack task management system with .NET 8 and Angular 18"
4. Choose Public or Private
5. **DO NOT** initialize with README, .gitignore, or license (we already have these)
6. Click "Create repository"

### Step 6: Connect and Push

```bash
# Add remote (replace YOUR_USERNAME with your GitHub username)
git remote add origin https://github.com/YOUR_USERNAME/TaskManager.git

# Rename branch to main (if needed)
git branch -M main

# Push to GitHub
git push -u origin main
```

## After Pushing to GitHub

### Update README Badge

Edit `README.md` and replace `yourusername` with your actual GitHub username:

```markdown
[![CI](https://github.com/YOUR_USERNAME/TaskManager/actions/workflows/ci.yml/badge.svg)]
```

### Enable GitHub Actions

1. Go to your repository on GitHub
2. Click "Actions" tab
3. Enable workflows if prompted
4. CI will run on every push

### Add Repository Topics

On GitHub, add topics to help others find your project:
- `dotnet`
- `angular`
- `postgresql`
- `docker`
- `task-management`
- `full-stack`

## Making It "Live" (Public Access)

To make the application accessible to everyone online, you need to deploy it. See [DEPLOYMENT.md](./docs/DEPLOYMENT.md) for options:

### Quick Deploy Options:

1. **Railway** (Easiest):
   - Connect GitHub repo
   - Auto-deploys on push
   - Provides public URLs

2. **Render**:
   - Connect GitHub repo
   - Free tier available
   - Auto-deploys on push

3. **Vercel** (Frontend) + **Railway** (Backend):
   - Best performance
   - Free tiers available

## Security Reminders

⚠️ **NEVER commit**:
- Real JWT secret keys
- Real password salts
- Real database passwords
- API keys or tokens
- Personal information

✅ **Always use**:
- Example/template files
- Environment variables
- `.gitignore` to exclude secrets
- Platform secrets management (Railway, Render, etc.)

## Repository Structure for GitHub

Your repository should include:
```
TaskManager/
├── .github/
│   ├── workflows/
│   │   └── ci.yml          # CI/CD pipeline
│   └── ISSUE_TEMPLATE/     # Issue templates
├── backend/                 # .NET backend
├── docker/
│   ├── docker-compose.example.yml  # ✅ Safe to commit
│   └── docker-compose.yml         # ❌ NOT committed (has secrets)
├── frontend/                # Angular frontend
├── docs/                    # Documentation
├── scripts/                 # Utility scripts
├── .gitignore              # Git ignore rules
├── README.md               # Main readme
├── SETUP.md                # Setup instructions
├── CONTRIBUTING.md         # Contribution guide
└── LICENSE                 # License file
```

## Next Steps

1. ✅ Push to GitHub (follow steps above)
2. ✅ Deploy to a platform (see DEPLOYMENT.md)
3. ✅ Share your repository URL
4. ✅ Update README with live demo link (if deployed)

## Troubleshooting

### "Secrets found in repository"

If GitHub warns about secrets:
1. Remove the commit with secrets: `git rebase -i HEAD~n` (where n is commits back)
2. Or use: `git filter-branch` or `git-filter-repo`
3. Force push: `git push --force` (⚠️ only if you're the only contributor)

### "File too large"

If you get file size errors:
- Check `.gitignore` includes `node_modules/`, `bin/`, `obj/`, `dist/`
- Remove large files: `git rm --cached large-file`
- Use Git LFS for large files if needed

## Support

For issues with GitHub setup:
- Check [SETUP.md](./SETUP.md)
- Check [QUICKSTART.md](./docs/QUICKSTART.md)
- Open an issue on GitHub
