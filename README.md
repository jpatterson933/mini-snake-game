# Mini Snake Game

A mini snake game built with C# and ASP.NET Core with real-time multiplayer support using SignalR.

## Features

- Real-time snake game with smooth gameplay
- Multiple power-ups (Speed Boost, Score Multiplier, Invincibility, etc.)
- Particle effects and visual feedback
- **High Score Leaderboard** - Top 5 scores saved to PostgreSQL database
- Input validation and security features to prevent malicious attacks

## Prerequisites

- .NET 9.0 SDK
- PostgreSQL database (for high scores feature)

## Local Development

```bash
dotnet run
```

The application will be available at `http://localhost:8080`

### Local Database Setup

For local development with the high scores feature, set the `DATABASE_URL` environment variable:

```bash
export DATABASE_URL="postgres://user:password@localhost:5432/snakegame"
dotnet run
```

## Railway Deployment

This application is configured to deploy to Railway using Docker.

### Deploy Steps:

1. Push code to GitHub
2. Create a new project in Railway
3. Connect your GitHub repository
4. Railway will automatically detect the Dockerfile and deploy
5. **Add PostgreSQL database** - See [RAILWAY_SETUP.md](RAILWAY_SETUP.md) for detailed instructions

The application listens on the PORT environment variable provided by Railway.

## Database & High Scores

The game includes a high score system that saves the top 5 scores to a PostgreSQL database.

### Features:

- Automatic prompt when achieving a top 5 score
- Input validation to prevent offensive language and malicious attacks
- SQL injection and XSS protection
- Leaderboard accessible from the main menu

### Setup:

See [RAILWAY_SETUP.md](RAILWAY_SETUP.md) for complete PostgreSQL setup instructions on Railway.

## Security

The application includes multiple security layers:

- Input sanitization and validation
- Parameterized database queries (via Entity Framework Core)
- Offensive language filtering
- SQL injection pattern detection
- XSS attack prevention
