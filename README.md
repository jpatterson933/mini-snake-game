# Mini Snake Game

A mini snake game built with C# and ASP.NET Core.

## Prerequisites

- .NET 8.0 SDK

## Local Development

```bash
dotnet run
```

The application will be available at `http://localhost:8080`

## Railway Deployment

This application is configured to deploy to Railway using Docker.

### Deploy Steps:

1. Push code to GitHub
2. Create a new project in Railway
3. Connect your GitHub repository
4. Railway will automatically detect the Dockerfile and deploy

The application listens on the PORT environment variable provided by Railway.
