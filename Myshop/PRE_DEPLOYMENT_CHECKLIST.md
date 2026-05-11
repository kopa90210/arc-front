# 📋 PRE-DEPLOYMENT CHECKLIST

Use this checklist to ensure your project is production-ready before deployment.

## ✅ SECURITY

- [ ] **Remove hardcoded secrets** from code
- [ ] **Review .gitignore** - sensitive files are excluded
  - [ ] `appsettings.Production.json`
  - [ ] `appsettings.Development.json`
  - [ ] `.env.local`
  - [ ] Database credentials
- [ ] **JWT key is strong** (minimum 32 characters)
- [ ] **CORS origins are specific** (not using `*`)
- [ ] **HTTPS is enforced** in production
- [ ] **No console.log() debugging statements** left in code
- [ ] **API rate limiting** is configured (if needed)
- [ ] **Input validation** on all API endpoints
- [ ] **SQL injection protection** (using Entity Framework parameterized queries)

## ✅ FRONTEND

- [ ] **Build completes without errors**: `npm run build` ✓
- [ ] **Environment variables configured**:
  - [ ] `VITE_API_URL` set to production backend
  - [ ] `VITE_API_HOST` correct
- [ ] **All dependencies updated**: `npm audit`
- [ ] **No console errors** in browser (F12 → Console)
- [ ] **Mobile responsive** (test on phone/tablet)
- [ ] **Lighthouse score** > 80 in all categories
- [ ] **Images optimized** (WebP format where possible)
- [ ] **Lazy loading** for images/components
- [ ] **Error boundaries** implemented
- [ ] **Loading states** for all async operations
- [ ] **404 page** exists and is reachable

## ✅ BACKEND

- [ ] **.NET project builds**: `dotnet build -c Release` ✓
- [ ] **All tests pass**: `dotnet test` ✓
- [ ] **Environment variables configured**:
  - [ ] `MYSHOP_DB_CONNECTION`
  - [ ] `MYSHOP_JWT_KEY`
  - [ ] `MYSHOP_ALLOWED_ORIGINS`
  - [ ] `MYSHOP_SENDGRID_APIKEY` (or SMTP config)
- [ ] **Logging is configured** (not too verbose in production)
- [ ] **Database migrations** created and tested
- [ ] **Database seeding** works correctly
- [ ] **API documentation** (Swagger) is up-to-date
- [ ] **Error handling** returns meaningful error messages (not stack traces)
- [ ] **Async/await** properly used (no deadlocks)
- [ ] **Connection pooling** configured for database
- [ ] **Health check endpoint** works (`/api/health`)

## ✅ DATABASE

- [ ] **Supabase connection** string is correct
- [ ] **All migrations applied** to production schema
- [ ] **Backup strategy** in place (Supabase auto-backups)
- [ ] **Indexes** created on frequently queried columns
- [ ] **Foreign keys** with proper constraints
- [ ] **Data type sizes** appropriate (no BIGINT where INT sufficient)
- [ ] **Sensitive data** is not logged or exposed in APIs

## ✅ DEPLOYMENT

- [ ] **Docker builds successfully**: `docker build .` ✓
- [ ] **Docker Compose works**: `docker-compose up` ✓
- [ ] **Railway account created** and project set up
- [ ] **Vercel account created** and project set up
- [ ] **GitHub repository** is up-to-date (no uncommitted changes)
- [ ] **GitHub Actions workflows** created:
  - [ ] `deploy-backend.yml`
  - [ ] `deploy-frontend.yml`
  - [ ] `tests.yml`
- [ ] **GitHub Secrets configured**:
  - [ ] `RAILWAY_TOKEN`
  - [ ] `VERCEL_TOKEN`
  - [ ] `VERCEL_ORG_ID`
  - [ ] `VERCEL_PROJECT_ID`
- [ ] **Railway environment variables** set
- [ ] **Vercel environment variables** set
- [ ] **DNS records prepared** (if using custom domains)
- [ ] **SSL certificates** configured (automatic)

## ✅ MONITORING & LOGGING

- [ ] **Structured logging** configured (JSON format)
- [ ] **Log levels** appropriate for production
- [ ] **No sensitive data** in logs
- [ ] **Log retention** policy set
- [ ] **Error tracking** configured (Sentry recommended)
- [ ] **Performance monitoring** enabled (Application Insights)
- [ ] **Uptime monitoring** configured (Uptime Robot)
- [ ] **Email alerts** set up for critical errors

## ✅ PERFORMANCE

- [ ] **API response time** < 200ms (p95)
- [ ] **Frontend initial load** < 3 seconds
- [ ] **Database queries optimized** (no N+1 queries)
- [ ] **Static assets cached** (Vercel does this automatically)
- [ ] **Compression enabled** (gzip/brotli)
- [ ] **Image optimization** (WebP, lazy loading)
- [ ] **Code splitting** implemented (Vite does this)
- [ ] **Bundle size** < 500KB (gzipped)

## ✅ TESTING

- [ ] **Unit tests** pass (minimum 70% coverage)
- [ ] **Integration tests** for API endpoints
- [ ] **E2E tests** for critical workflows (if applicable)
- [ ] **Manual testing checklist**:
  - [ ] User registration
  - [ ] Login/logout
  - [ ] Browse products
  - [ ] Add to cart
  - [ ] Checkout process
  - [ ] Order confirmation email
  - [ ] Admin dashboard
  - [ ] Vendor dashboard

## ✅ DOCUMENTATION

- [ ] **README.md** updated with setup instructions
- [ ] **DEPLOYMENT_GUIDE.md** is complete
- [ ] **API documentation** (Swagger) accessible
- [ ] **Environment variables** documented (.env.example)
- [ ] **Database schema** documented
- [ ] **Architecture** documented
- [ ] **Team has access** to documentation

## ✅ TEAM READINESS

- [ ] **Team trained** on deployment process
- [ ] **Runbooks** created for common issues
- [ ] **On-call schedule** established
- [ ] **Rollback plan** documented
- [ ] **Incident response** plan in place
- [ ] **Communication channels** set up (Slack, email)
- [ ] **Database backup tested** (can restore successfully)

## 🚀 DEPLOYMENT DAY

### Pre-Deployment (1 hour before)

1. [ ] Pull latest `main` branch code
2. [ ] Verify all tests pass locally
3. [ ] Check database backup is recent
4. [ ] Notify team of deployment
5. [ ] Close support tickets that require deployment

### Deployment

1. [ ] Backend deploys to Railway successfully
2. [ ] Frontend deploys to Vercel successfully
3. [ ] All environment variables loaded correctly
4. [ ] Database migrations completed
5. [ ] No errors in deployment logs

### Post-Deployment (30 minutes after)

1. [ ] Verify API responds at new URL
2. [ ] Verify frontend loads at new URL
3. [ ] Test critical user flows
4. [ ] Check monitoring dashboards
5. [ ] Monitor error logs
6. [ ] Verify emails working
7. [ ] Test from different browsers/devices

### Verification

- [ ] No 404 errors in monitoring
- [ ] No 500 errors in logs
- [ ] Response times normal
- [ ] Database connections stable
- [ ] All services up and running

### If Issues Occur

1. [ ] Check deployment logs first
2. [ ] Verify environment variables
3. [ ] Rollback if critical issue (can revert on Railway/Vercel)
4. [ ] Document issue for post-mortem
5. [ ] Communicate status to team

---

## 📞 CONTACT & ESCALATION

- **Frontend Issues**: Frontend Lead
- **Backend Issues**: Backend Lead
- **Database Issues**: Database Admin / DBA
- **Deployment Issues**: DevOps / Tech Lead
- **Emergency**: Notify entire team immediately

---

## NOTES

Use this section to track any blockers or notes:

```
- [ ] Issue: _______________
  Solution: _______________
  Status: _______________

- [ ] Issue: _______________
  Solution: _______________
  Status: _______________
```

---

**Last Updated**: May 4, 2026
**Next Review**: After first production deployment
