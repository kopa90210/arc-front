# 🚀 Railway & Vercel Deployment Guide

## Overview
This guide walks you through deploying MyShop to Railway (backend) and Vercel (frontend).

---

## PREREQUISITES

### Required Accounts & Tools
- [ ] GitHub account with your code pushed
- [ ] [Railway account](https://railway.app) (sign up with GitHub)
- [ ] [Vercel account](https://vercel.com) (sign up with GitHub)
- [ ] [Railway CLI](https://docs.railway.app/cli/installation) installed locally
- [ ] [Vercel CLI](https://vercel.com/docs/cli) installed locally (optional)

### Environment Variables Checklist
- [ ] Database connection string (Supabase)
- [ ] JWT secret key (generate a strong one)
- [ ] SendGrid API key (for emails)
- [ ] SMTP credentials (for email fallback)
- [ ] Frontend URL (for CORS)
- [ ] Backend URL (for frontend API calls)

---

## PART 1: RAILWAY DEPLOYMENT (Backend)

### Step 1.1: Connect GitHub to Railway

1. Go to [railway.app](https://railway.app)
2. Click **"Start a New Project"**
3. Click **"Import from GitHub"**
4. Authorize Railway to access your GitHub repositories
5. Select the **Myshop** repository
6. Click **"Deploy"**

### Step 1.2: Configure PostgreSQL Database

1. In Railway dashboard, click **"New"** → **"Database"** → **"PostgreSQL"**
2. Name it: `myshop-db`
3. Add it to your project
4. Railway will automatically generate `DATABASE_URL` environment variable

### Step 1.3: Set Environment Variables

In Railway dashboard, go to your API service and add these variables:

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
MYSHOP_DB_CONNECTION=${{DATABASE_URL}}
MYSHOP_JWT_KEY=<generate-a-strong-secret-key>
MYSHOP_JWT_ISSUER=Myshop.Api
MYSHOP_JWT_AUDIENCE=Myshop.Client
MYSHOP_ALLOWED_ORIGINS=https://myshop.vercel.app,https://www.myshop.com
MYSHOP_SENDGRID_APIKEY=<your-sendgrid-key>
MYSHOP_SMTP_HOST=smtp.gmail.com
MYSHOP_SMTP_PORT=587
MYSHOP_SMTP_USER=<your-email@gmail.com>
MYSHOP_SMTP_PASS=<your-app-password>
```

### Step 1.4: Run Database Migrations

1. In Railway, open your API service
2. Go to **Settings** → **Build** → **Build command**
3. Set to: `dotnet publish -c Release`
4. Go to **Deploy** → **Deployments**
5. Click **"Redeploy"** to trigger a new build
6. Check the deployment logs

### Step 1.5: Verify Backend Deployment

1. Get your Railway domain:
   - Go to API service → **Settings** → **Public Networking**
   - Copy the domain (e.g., `myshop-api-production.up.railway.app`)

2. Test the API:
   ```bash
   curl https://myshop-api-production.up.railway.app/api/health
   ```

3. You should see a 200 OK response

### Step 1.6: Configure Custom Domain (Optional)

1. Go to API service → **Settings** → **Domains**
2. Add custom domain: `api.myshop.com`
3. Update DNS records at your domain provider
4. Enable HTTPS (automatic via Let's Encrypt)

---

## PART 2: VERCEL DEPLOYMENT (Frontend)

### Step 2.1: Connect GitHub to Vercel

1. Go to [vercel.com](https://vercel.com)
2. Click **"New Project"**
3. Click **"Import Git Repository"**
4. Authorize Vercel to access GitHub
5. Select **Myshop** repository
6. Click **"Import"**

### Step 2.2: Configure Build Settings

1. **Root Directory**: `client`
2. **Build Command**: `npm run build`
3. **Output Directory**: `dist`
4. **Install Command**: `npm install` (default)

### Step 2.3: Set Environment Variables

In Vercel dashboard, go to **Settings** → **Environment Variables** and add:

```
VITE_API_URL=https://myshop-api-production.up.railway.app/api
VITE_API_HOST=https://myshop-api-production.up.railway.app
```

(Replace with your actual Railway domain)

### Step 2.4: Deploy

1. Click **"Deploy"**
2. Wait for build to complete (usually 2-3 minutes)
3. Visit your deployment URL
4. Verify frontend loads correctly

### Step 2.5: Configure Custom Domain (Optional)

1. Go to **Settings** → **Domains**
2. Add your domain: `myshop.com`
3. Update DNS records at your domain provider
4. Vercel provides DNS instructions

### Step 2.6: Enable Auto-Deployments

1. Go to **Settings** → **Git**
2. Enable **"Automatic Deployments"** for `main` branch
3. Future pushes to `main` will auto-deploy

---

## PART 3: GITHUB ACTIONS SETUP (CI/CD Automation)

The workflows in `.github/workflows/` will automatically:
- ✅ Build and test on every PR
- ✅ Deploy backend to Railway on `main` push
- ✅ Deploy frontend to Vercel on `main` push

### Step 3.1: Configure GitHub Secrets

Go to **GitHub** → **Settings** → **Secrets and variables** → **Actions** and add:

**For Railway:**
```
RAILWAY_TOKEN=<get-from-railway-dashboard>
```

**For Vercel:**
```
VERCEL_TOKEN=<get-from-vercel-settings>
VERCEL_ORG_ID=<your-org-id>
VERCEL_PROJECT_ID=<your-project-id>
```

### Step 3.2: Get Railway Token

1. Go to [Railway account settings](https://railway.app/account/tokens)
2. Click **"Create New Token"**
3. Name it: `GitHub Actions`
4. Copy the token and add to GitHub Secrets as `RAILWAY_TOKEN`

### Step 3.3: Get Vercel Credentials

1. Go to [Vercel account settings](https://vercel.com/account/tokens)
2. Create a new token
3. Add to GitHub as `VERCEL_TOKEN`
4. Get `VERCEL_ORG_ID` and `VERCEL_PROJECT_ID` from Vercel dashboard

### Step 3.4: Test Workflows

1. Make a test commit to `main` branch
2. Go to **GitHub** → **Actions**
3. Watch the workflows run
4. Check that both deployments complete successfully

---

## PART 4: POST-DEPLOYMENT TESTING

### Checklist

- [ ] Frontend loads at Vercel URL
- [ ] API responds at Railway domain
- [ ] Login/registration works
- [ ] Can browse products
- [ ] Can add to cart
- [ ] Can view orders
- [ ] SignalR notifications work
- [ ] Emails are sent (if configured)
- [ ] Mobile responsive ✓
- [ ] Performance is acceptable (Lighthouse score > 80)

### Test Commands

```bash
# Test API connectivity
curl https://myshop-api-production.up.railway.app/api/products

# Test frontend build
cd client
npm run build
npm run preview
```

---

## PART 5: MONITORING & MAINTENANCE

### Enable Railway Monitoring

1. Go to Railway dashboard
2. Enable **Metrics** for your services
3. Set up **Alerts** for:
   - High CPU usage (> 80%)
   - High memory usage (> 80%)
   - Deployment failures
   - Database connection issues

### Enable Vercel Analytics

1. Go to Vercel dashboard
2. Click **"Analytics"**
3. View real-time performance metrics
4. Check Core Web Vitals

### Set Up Error Tracking (Recommended)

Install Sentry for better error monitoring:

1. Sign up at [sentry.io](https://sentry.io)
2. Create new projects for backend and frontend
3. Add Sentry SDKs to your code
4. Get automatic error alerts

### Database Backups

Supabase automatically backs up your database:
1. Go to Supabase dashboard
2. Check **Backups** section
3. Enable **Point-in-time recovery** (PRO plan)

---

## TROUBLESHOOTING

### Backend Deployment Issues

**Issue**: "Deployment failed - Build error"
- **Solution**: Check build logs in Railway dashboard, ensure `Myshop.Api.csproj` is in root

**Issue**: "Database connection error"
- **Solution**: Verify `DATABASE_URL` environment variable is set correctly

**Issue**: "CORS errors in frontend"
- **Solution**: Update `MYSHOP_ALLOWED_ORIGINS` to include your Vercel URL

### Frontend Deployment Issues

**Issue**: "API 404 errors"
- **Solution**: Ensure `VITE_API_URL` environment variable points to correct Railway domain

**Issue**: "Build timeout"
- **Solution**: Clear Vercel cache: Settings → Git → Clear Cache & Redeploy

**Issue**: "Environmental variable not loading"
- **Solution**: Redeploy after updating Vercel env vars (Settings → Deployments → Redeploy)

---

## USEFUL LINKS

- 📚 [Railway Docs](https://docs.railway.app)
- 📚 [Vercel Docs](https://vercel.com/docs)
- 📚 [Supabase Docs](https://supabase.com/docs)
- 📚 [.NET Deployment](https://learn.microsoft.com/aspnet/core/host-and-deploy)
- 📚 [Vite Deployment](https://vitejs.dev/guide/static-deploy.html)

---

## NEXT STEPS

1. **Add custom domains** for professional URLs
2. **Set up CI/CD pipeline** for automated testing
3. **Implement monitoring** with Sentry/New Relic
4. **Configure email notifications** for orders
5. **Enable caching** for better performance
6. **Set up CDN** for static assets
7. **Implement analytics** to track user behavior

---

## SUPPORT

For issues:
- 📧 Check deployment logs in Railway/Vercel dashboards
- 🔗 Consult official documentation
- 💬 Ask in Railway/Vercel community forums
