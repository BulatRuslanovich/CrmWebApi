# PharmaCRM — Web API

REST API for a pharmaceutical CRM system. Manages visits, organizations, doctors, and medications with role-based access control.

## Tech Stack

| | |
|---|---|
| Runtime | .NET 10 / ASP.NET Core |
| Database | PostgreSQL 16+ |
| ORM | Entity Framework Core (Npgsql) |
| Auth | JWT Bearer + Refresh Tokens |
| Password hashing | BCrypt.Net |
| API docs | Scalar (OpenAPI) |

## Architecture

```
CrmWebApi/
├── Controllers/        # HTTP endpoints
├── Services/           # Business logic (interface + impl)
├── Repositories/       # Data access layer (interface + impl)
├── Data/
│   ├── Entities/       # EF Core entity models
│   └── AppDbContext.cs
├── DTOs/               # Request / Response models
├── Middleware/         # Global exception handler
├── Extensions/         # DI service registration
└── sql-scripts/        # DB initialization SQL
```

## API Endpoints

| Resource | Methods |
|---|---|
| `POST /api/auth/login` | Login, returns access + refresh token |
| `POST /api/auth/refresh` | Refresh access token |
| `POST /api/auth/logout` | Revoke refresh token |
| `/api/users` | CRUD, role assignment (`POST/DELETE /{id}/policies/{policyId}`) |
| `/api/orgs` | Organizations CRUD |
| `/api/activs` | Visits CRUD + drug linking |
| `/api/physes` | Physicians CRUD |
| `/api/drugs` | Drugs CRUD |
| `/api/specs` | Specialties CRUD |
| `/api/statuses` | List statuses |
| `/api/policies` | List policies |
| `/api/org-types` | List organization types |

## Access control

| Role | Permissions |
|---|---|
| Admin | Full access, see all visits |
| Director | See all visits |
| Manager / Representative | Own visits only, no delete |

## Getting started

### 1. Database

```bash
psql -U postgres -c "CREATE USER crm_user WITH PASSWORD 'your_password';"
psql -U postgres -c "CREATE DATABASE crm_db OWNER crm_user;"
psql -U crm_user -d crm_db -f sql-scripts/01-init-tables.sql
```

Or with Docker:

```bash
docker-compose up -d
```

### 2. Configure `appsettings.json`

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=crm_db;Username=crm_user;Password=your_password"
  },
  "Jwt": {
    "Secret": "your-secret-key-minimum-32-characters-long",
    "Issuer": "PharmaCrmApi",
    "Audience": "PharmaCrmClient",
    "AccessTokenTtlMinutes": 15,
    "RefreshTokenTtlDays": 7
  }
}
```

### 3. Run

```bash
dotnet run
```

Scalar API docs: `http://localhost:5000/scalar`

## Visit status workflow

```
Запланирован → Открыт → Сохранен
                  ↓
               Закрыт
```

Start time is recorded when a visit is opened; end time when it is closed. Closed visits are read-only for non-admins.
