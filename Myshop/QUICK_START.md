# 🏃 QUICK START GUIDE FOR DEPLOYMENT

## For Team Members: Getting Started

### Prerequisites Installation

```bash
# 1. Install Railway CLI
npm install -g @railway/cli

# 2. Install Vercel CLI (optional)
npm install -g vercel

# 3. Verify .NET SDK
dotnet --version  # Should be 9.0 or higher

# 4. Verify Node.js
node --version    # Should be 18+ 
npm --version     # Should be 9+
```

### Local Development Setup

```bash
# Clone and navigate to project
git clone <your-repo>
cd Myshop

# Backend setup
cd Myshop.Api
cp appsettings.Development.json appsettings.Development.Local.json
# Edit appsettings.Development.Local.json with your local DB connection string
dotnet restore
dotnet build

# Frontend setup
cd ../client
cp .env.example .env.local
# Edit .env.local with local API URL (http://localhost:5000/api)
npm install
npm run dev
```

### Running Backend Locally

```bash
cd Myshop.Api
dotnet run --launch-profile https
# API will be at https://localhost:7000 or http://localhost:5000
```

### Running Frontend Locally

```bash
cd client
npm run dev
# Frontend will be at http://localhost:5173
```

### Using Docker Compose (Recommended for Production-like setup)

```bash
# Copy environment template
cp .env.docker.example .env.docker

# Edit .env.docker with your secrets
nano .env.docker

# Start all services (API + Database + Frontend)
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop services
docker-compose down
```

---

## Deployment Quick Commands

### Deploy to Railway (Backend)

```bash
# Login to Railway
railway login

# Link project
railway link

# Deploy current branch
railway deploy

# Check status
railway status

# View logs
railway logs
```

### Deploy to Vercel (Frontend)

```bash
# Login to Vercel
vercel login

# Deploy to production
vercel --prod

# Or just push to main branch (auto-deploys if configured)
git push origin main
```

### Manual Deployment Steps

1. **Update Code**
   ```bash
   git add .
   git commit -m "feat: your changes"
   git push origin main
   ```

2. **GitHub Actions will automatically**:
   - Run tests
   - Build backend
   - Deploy to Railway
   - Build frontend
   - Deploy to Vercel

3. **Monitor Deployments**
   - Check GitHub Actions tab
   - View Railway dashboard
   - View Vercel dashboard

---

## Environment Variables Reference

### Frontend (.env.production)
```env
VITE_API_URL=https://api.railway.app/api
VITE_API_HOST=https://api.railway.app
```

### Backend (Railway dashboard)
```
ASPNETCORE_ENVIRONMENT=Production
MYSHOP_DB_CONNECTION=postgresql://...
MYSHOP_JWT_KEY=<strong-key>
MYSHOP_ALLOWED_ORIGINS=https://myshop.vercel.app
```

---

## Common Tasks

### Running Database Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName \
  --project Myshop.Infrastructure \
  --startup-project Myshop.Api

# Apply migrations locally
dotnet ef database update \
  --project Myshop.Infrastructure \
  --startup-project Myshop.Api

# Apply migrations on Railway (via dashboard)
# Redeploy or use Railway CLI
```

### Viewing Logs

```bash
# Backend logs on Railway
railway logs --service api

# Frontend logs on Vercel
vercel logs

# Local logs
cd Myshop.Api && dotnet run --verbosity=detailed
```

### Rollback a Deployment

```bash
# Railway: Click "Rollback" in dashboard
# Vercel: Click "Deployments" → Select previous version → "Rollback"

# Or via CLI:
railway deployments --service api  # See history
railway rollback                     # Rollback to previous

vercel deployments                   # See history
vercel rollback                      # Rollback to previous
```

---

## Debugging Issues

### Frontend not loading API correctly
```bash
# Check env vars in Vercel dashboard
# Check browser console (F12) for actual API URL being used
# Verify CORS headers: curl -i https://api.railway.app/api/products
```

### Backend deployment failed
```bash
# Check Railway build logs
railway logs --service api --build

# Verify all environment variables are set
railway variables

# Check database connection
# Can you connect to Supabase from Railway?
```

### Database connection timeout
```bash
# Check Supabase connection string format
# Verify DATABASE_URL environment variable
# Check Railway ← → Supabase network connectivity
# May need to whitelist Railway IP on Supabase
```

---

## Testing Production Before Going Live

```bash
# Full integration test
curl https://your-railway-domain.app/api/products
curl https://your-vercel-domain.app  # Should not 404

# Login test
curl -X POST https://your-railway-domain.app/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"password"}'
```

---

## Useful Commands Cheatsheet

```bash
# Kill a port if needed
# macOS/Linux: lsof -ti:5000 | xargs kill -9
# Windows: netstat -ano | findstr :5000 (then taskkill /PID <pid> /F)

# Clear node_modules and reinstall
rm -rf client/node_modules
npm install --prefix client

# Force rebuild Docker image
docker-compose build --no-cache

# View Docker container stats
docker stats
```

---

## Need Help?

1. **Check logs first** - 90% of issues are visible in logs
2. **Check environment variables** - Most common issue
3. **Run locally** - If it works locally, it's likely an env var issue
4. **Ask team** - Check Slack/Discord for solutions
5. **Read docs** - Railway and Vercel have excellent docs

---

**Questions?** Ask your team lead or DevOps engineer!
