#!/bin/bash
# Railway startup script to run migrations and start the application

echo "Running database migrations..."
dotnet Myshop.Infrastructure.dll migrate

echo "Starting MyShop API..."
exec dotnet Myshop.Api.dll
