# Insurance Product Builder

A full-stack application for underwriting teams to configure hierarchical insurance products and generate quotes.

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | React 19 + TypeScript + Vite 7 + Tailwind CSS 3 |
| Backend | .NET 10 Web API (Clean Architecture) |
| Database | PostgreSQL |
| Auth | JWT Bearer + Refresh Tokens |
| ORM | Entity Framework Core 10 (Npgsql) |
| API Docs | Swagger UI (Swashbuckle 6.x) |

---

## Project Structure

```
ProductBuilder/
├── ProductBuilder.API/          ← .NET 10 backend
│   ├── src/
│   │   ├── ProductBuilder.Domain/         # Entities, enums
│   │   ├── ProductBuilder.Application/    # DTOs, interfaces
│   │   ├── ProductBuilder.Infrastructure/ # EF Core, services, migrations
│   │   └── ProductBuilder.API/           # Controllers, Program.cs
│   └── tests/
│       └── ProductBuilder.Tests/
└── ProductBuilder.UI/           ← React frontend
    └── src/
        ├── api/          # Axios client + per-resource functions
        ├── components/   # Reusable UI (Button, Input, Modal, Table…)
        ├── features/     # Page-level components per feature
        ├── store/        # AuthContext
        ├── types/        # TypeScript interfaces
        └── utils/        # formatters, cn()
```

---

## Domain Model

```
LineOfBusiness (LOB)
  └── Product
        └── Coverage
              └── Cover
                    ├── Limit
                    ├── Deductible
                    ├── Premium (Flat | RateBased | PerUnit)
                    └── Modifier (Loading | Discount)

Quote
  ├── QuoteCover  (selected covers + basis value + calculated premium)
  └── QuoteModifier (applied modifiers + premium impact)

Stakeholders: Insurer · Underwriter · Broker
```

---

## API Endpoints

| Controller | Base Path | Key Operations |
|---|---|---|
| Auth | `/api/auth` | login, refresh, logout |
| Users | `/api/users` | CRUD (Admin only) |
| Lines of Business | `/api/lob` | CRUD |
| Insurers | `/api/insurers` | CRUD |
| Products | `/api/products` | CRUD + status transitions |
| Coverages | `/api/products/{id}/coverages` | CRUD |
| Covers | `/api/coverages/{id}/covers` | CRUD |
| Limits | `/api/covers/{id}/limits` | CRUD |
| Deductibles | `/api/covers/{id}/deductibles` | CRUD |
| Premiums | `/api/covers/{id}/premiums` | CRUD |
| Modifiers | `/api/modifiers` | CRUD (cover or product scope) |
| Underwriters | `/api/underwriters` | CRUD |
| Brokers | `/api/brokers` | CRUD |
| Quotes | `/api/quotes` | CRUD + calculate + submit |

---

## Roles

| Role | Access |
|---|---|
| Admin | Full access, user management |
| Underwriter | Product configuration, quote review/approval |
| Broker | Create quotes, view own quotes |
| Insurer | View products and quotes for their insurer |

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- Node.js 20+
- PostgreSQL (running locally or via Docker)

### 1. Database

```bash
# Quick start with Docker
docker run -d --name pg \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 postgres:16
```

Or update the connection string in `ProductBuilder.API/src/ProductBuilder.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=productbuilder;Username=postgres;Password=postgres"
  }
}
```

### 2. Backend

```bash
cd ProductBuilder.API/src/ProductBuilder.API
dotnet run
```

- API base: `http://localhost:5000/api`
- Swagger UI: `http://localhost:5000/swagger`
- Migrations run automatically on startup
- Seed data is applied on first run

**Default admin account (seeded):**

| Field | Value |
|---|---|
| Email | `admin@productbuilder.com` |
| Password | `Admin@123` |

### 3. Frontend

```bash
cd ProductBuilder.UI
npm install
npm run dev
```

- App: `http://localhost:5173`
- API requests are proxied to `http://localhost:5000` via Vite config

---

## Frontend Pages

| Route | Page |
|---|---|
| `/login` | Login |
| `/dashboard` | Overview stats + recent quotes |
| `/lob` | Lines of Business CRUD |
| `/products` | Product list |
| `/products/:id` | Product detail — Coverage/Cover accordion tree + financial config |
| `/quotes` | Quote list |
| `/quotes/new` | 4-step Quote Wizard |
| `/quotes/:id` | Quote detail + premium breakdown |
| `/insurers` | Insurer management |

---

## Configuration

### Backend (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=productbuilder;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "SecretKey": "ProductBuilder-Super-Secret-Key-At-Least-32-Chars!!",
    "Issuer": "ProductBuilder.API",
    "Audience": "ProductBuilder.UI",
    "AccessTokenExpiryMinutes": "60",
    "RefreshTokenExpiryDays": "7"
  }
}
```

### Frontend (`.env`)

```
VITE_API_BASE_URL=http://localhost:5000/api
```

---

## Key Design Decisions

- **Clean Architecture**: Domain has zero external dependencies; Application defines interfaces; Infrastructure implements them.
- **EF Core snake_case**: All table/column names use snake_case via `IEntityTypeConfiguration<T>` files.
- **Premium Calculation**: `PremiumCalculationService` supports Flat, RateBased (rate × basis value), and PerUnit premium types, with Loading/Discount modifiers applied as percentage or fixed amounts.
- **JWT + Refresh**: Access tokens (60 min) + refresh tokens (7 days) with rotation on refresh. Stored in localStorage on the frontend.
- **Tailwind CSS 3**: Uses `tailwind.config.js` with a `primary` color extension. Kept at v3 for PostCSS plugin compatibility.
- **Swashbuckle 6.x**: Used instead of v10 because Swashbuckle v10 moved to `Microsoft.OpenApi` v2 which reorganized the `Models` namespace.

---

## EF Core Migrations

```bash
# Add a new migration (from solution root)
dotnet ef migrations add <MigrationName> \
  --project src/ProductBuilder.Infrastructure \
  --startup-project src/ProductBuilder.API

# Apply migrations manually
dotnet ef database update \
  --project src/ProductBuilder.Infrastructure \
  --startup-project src/ProductBuilder.API
```
