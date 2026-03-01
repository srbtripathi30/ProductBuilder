# CLAUDE.md — ProductBuilder Project Instructions

## Project Overview
Insurance Product Builder — a full-stack underwriting platform.
Configure hierarchical insurance products (LOB → Product → Coverage → Cover → Limit/Deductible/Premium/Modifier) and generate quotes with automated premium calculation.

## Repository Layout
```
ProductBuilder/
├── ProductBuilder.API/      ← .NET 10 backend (Clean Architecture)
│   ├── src/
│   │   ├── ProductBuilder.Domain/         # Entities, enums
│   │   ├── ProductBuilder.Application/    # DTOs, interfaces
│   │   ├── ProductBuilder.Infrastructure/ # EF Core, services, migrations
│   │   └── ProductBuilder.API/           # Controllers, Program.cs
│   └── tests/
│       └── ProductBuilder.Tests/          # xUnit tests
└── ProductBuilder.UI/       ← React 19 + TypeScript + Vite 7 frontend
    └── src/
        ├── api/          # Axios client + per-resource API functions
        ├── components/   # Reusable UI (Button, Input, Modal, Badge…)
        ├── features/     # Page-level components per feature
        ├── store/        # AuthContext
        ├── test/         # Vitest setup (setup.ts)
        ├── types/        # TypeScript interfaces
        └── utils/        # formatters.ts, cn.ts
```

## Tech Stack
- **Backend**: .NET 10 (`net10.0`), EF Core 10 + Npgsql, JWT auth, Swashbuckle 6.x, FluentValidation, BCrypt
- **Frontend**: React 19, TypeScript, Vite 7, Tailwind CSS 3, TanStack Query, React Router v6, Axios
- **Database**: PostgreSQL (Docker container `productbuilder-postgres` on port 5433)
- **Testing**: xUnit (backend), Vitest + @testing-library/react (frontend)

---

## Critical Rules

### Do NOT change these versions — they are pinned for compatibility
- `Swashbuckle.AspNetCore` must stay at **6.x** (v10 breaks `Microsoft.OpenApi.Models` namespace)
- `tailwindcss` must stay at **v3** (v4 breaks PostCSS config / `tailwind.config.js`)
- Target framework must be **`net10.0`** (machine has dotnet 10.0.102)

### Seed data — NEVER use dynamic values in HasData()
- `BCrypt.HashPassword()` inside `HasData()` causes `PendingModelChangesWarning` and crashes startup
- Always pre-compute a static BCrypt hash and hardcode it. The current admin hash is already static.
- Use `new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)` style static dates, not `DateTime.UtcNow`

### Backend conventions
- All EF Core table/column names are **snake_case** via `IEntityTypeConfiguration<T>`
- All PKs are UUIDs (`gen_random_uuid()`); set them explicitly in tests
- Request DTOs: `CreateXxxRequest` / `UpdateXxxRequest`; Response DTOs: `XxxDto`
- Controllers use `[ApiController]` + `[Route("api/[controller]")]`
- Do not skip `[Authorize]` on protected endpoints

### Frontend conventions
- API functions live in `src/api/<resource>.api.ts`, all use the shared Axios client
- UI primitives live in `src/components/ui/` — reuse before creating new ones
- Use `primary-*` Tailwind classes for brand color (defined in `tailwind.config.js`)
- Auth state comes from `AuthContext` — do not duplicate auth logic elsewhere
- Use `AmountInput` (not `<Input type="number">`) for all monetary amount fields — it provides k/m/l shortcut expansion (thousands / millions / lakhs) on Tab or blur; its `onChange` receives a `number` directly

### Testing conventions
- Backend: use `Microsoft.EntityFrameworkCore.InMemory` for EF Core tests; InMemory does NOT enforce FK constraints — set GUIDs manually
- Frontend: test files use `.test.ts` / `.test.tsx` suffix alongside their source files
- Run backend tests: `dotnet test` from solution root
- Run frontend tests: `npm run test:run` (single pass) or `npm run test` (watch)

---

## Build & Run Commands
```bash
# ── Backend ────────────────────────────────────────────────────────────────
cd ProductBuilder.API && dotnet build
cd ProductBuilder.API/src/ProductBuilder.API && dotnet run --urls "http://localhost:5000"

# ── Frontend ───────────────────────────────────────────────────────────────
cd ProductBuilder.UI && npm run dev       # http://localhost:5173
cd ProductBuilder.UI && npm run build

# ── Tests ─────────────────────────────────────────────────────────────────
cd ProductBuilder.API && dotnet test --logger "console;verbosity=normal"
cd ProductBuilder.UI  && npm run test:run

# ── EF Core Migrations ────────────────────────────────────────────────────
dotnet ef migrations add <Name> \
  --project src/ProductBuilder.Infrastructure \
  --startup-project src/ProductBuilder.API

dotnet ef database update \
  --project src/ProductBuilder.Infrastructure \
  --startup-project src/ProductBuilder.API

# ── Database (Docker) ─────────────────────────────────────────────────────
docker run -d --name productbuilder-postgres \
  -e POSTGRES_DB=productbuilder \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5433:5432 postgres:17-alpine
```

---

## Default Dev Credentials
| Field | Value |
|---|---|
| Admin email | `admin@productbuilder.com` |
| Admin password | `Admin@123` |
| DB connection | `Host=localhost;Port=5433;Database=productbuilder;Username=postgres;Password=postgres` |
| Docker container | `productbuilder-postgres` (port 5433) |

---

## Key Files
| File | Purpose |
|---|---|
| `ProductBuilder.API/src/ProductBuilder.API/Program.cs` | DI, Swagger, JWT, CORS, auto-migrate |
| `ProductBuilder.API/src/ProductBuilder.API/appsettings.json` | Connection string, JWT config |
| `ProductBuilder.API/src/ProductBuilder.Infrastructure/Data/AppDbContext.cs` | DbContext + static seed data |
| `ProductBuilder.API/src/ProductBuilder.Infrastructure/Services/PremiumCalculationService.cs` | Pricing logic |
| `ProductBuilder.API/src/ProductBuilder.Infrastructure/Migrations/` | EF Core migrations |
| `ProductBuilder.API/tests/ProductBuilder.Tests/Services/` | xUnit service tests |
| `ProductBuilder.UI/src/api/client.ts` | Axios instance + JWT interceptor + 401 refresh |
| `ProductBuilder.UI/src/store/AuthContext.tsx` | Auth state, localStorage persistence |
| `ProductBuilder.UI/src/App.tsx` | Full route tree |
| `ProductBuilder.UI/src/types/index.ts` | All TypeScript interfaces |
| `ProductBuilder.UI/vitest.config.ts` | Vitest + jsdom config |

---

## Test Coverage Summary
| Suite | Tests | What is covered |
|---|---|---|
| `PasswordServiceTests` | 6 | BCrypt hash format, uniqueness, verify correct / wrong / empty |
| `TokenServiceTests` | 11 | JWT generation, email/role claims, user ID extraction, refresh token |
| `PremiumCalculationServiceTests` | 14 | Flat/RateBased/PerUnit, min premium floor, Loading/Discount modifiers, unselected covers, not-found |
| `formatters.test.ts` | 22 | Currency (null, zero, EUR, large), date formatting, all status colors |
| `cn.test.ts` | 11 | Class merging, falsy filtering, Tailwind conflict resolution |
| `AmountInput.test.tsx` | 17 | parseAmountString (k/m/l/decimals/invalid), Tab/blur conversion, case-insensitivity, value sync, error state |
| **Total** | **81** | |
