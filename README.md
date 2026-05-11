.NET 6 Web API for a house broker MVP. Built with Clean Architecture
Avoided AutoMapper due to GHSA-rvv3-g6hj-g44x (DoS, all free versions affected, patch requires paid license). 
Mapping library Mapperly used instead.

# House Broker API

Brokers manage property listings; seekers (or anyone) browse and search. Includes a configurable commission engine and JWT auth with role separation.


## Stack

.NET 6 · ASP.NET Core Web API · EF Core 6 (SQL Server) · ASP.NET Core Identity · JWT Bearer · Mapperly · IMemoryCache · xUnit · Swagger / OpenAPI

## Prerequisites

- .NET 6 SDK
- SQL Server (LocalDB that ships with Visual Studio is fine, or any SQL Server instance)
- EF Core CLI tool: `dotnet tool install --global dotnet-ef --version 6.0.36`

## Setup

```bash
git clone <repo-url>
cd HouseBroker-MVP

dotnet restore

# create the database and seed commission tiers
dotnet ef database update --project src/HouseBroker.Infra --startup-project src/HouseBroker.API
```

The default connection string in `appsettings.json` targets `(localdb)\mssqllocaldb`. Update it if you use a different SQL Server instance.

## Run

```bash
dotnet run --project src/HouseBroker.API
```

Swagger UI is served at the root URL once the app is running (typically `https://localhost:7xxx`).

## Quick smoke test

In Swagger:

1. `POST /api/auth/register` — register a Broker:
```json
   {
     "fullName": "Test Broker",
     "email": "broker@test.com",
     "password": "Password1",
     "role": "Broker"
   }
```
2. Copy the `token` from the response.
3. Click **Authorize** at the top right, paste `Bearer <token>`, click Authorize, close.
4. `POST /api/properties` with a sample body. Should return 201 with `commissionAmount` populated.
5. Click **Logout** in the Authorize popup. `GET /api/properties/{id}` should now return 200 *without* the commission field.

If those four things work, the core requirements are functional.

## Project structure

```
src/
├── HouseBroker.Domain/    entities, enums, constants (zero external dependencies)
├── HouseBroker.App/       DTOs, service interfaces, mappers, validators
├── HouseBroker.Infra/     EF Core, Identity, JWT, service implementations
└── HouseBroker.API/       controllers, middleware, Program.cs

tests/
└── HouseBroker.UnitTests/ commission engine, property service, controller tests
```

Dependency direction: `API → App, API → Infra → App → Domain`. Domain has no project references.

## Commission engine

Commission tiers live in the `CommissionRates` table and are seeded on first migration. The rules from the spec:

| Price range (NPR) | Rate |
|---|---|
| less than 5,000,000 | 2.00% |
| 5,000,000 – 10,000,000 | 1.75% |
| greater than 10,000,000 | 1.50% |

Tiers can be edited directly in the database — no code redeploy needed. Values are cached in memory for 30 minutes after first read.

Commission is included in property responses **only when the requesting user is the broker who owns the listing**. Anonymous users and other brokers see the property without it.

## Key decisions and trade-offs

- **Mapperly chosen over AutoMapper.** AutoMapper's free versions are affected by an unpatched DoS vulnerability ([GHSA-rvv3-g6hj-g44x](https://github.com/advisories/GHSA-rvv3-g6hj-g44x)); patched versions require a paid license. Mapperly generates mapping code at compile time and is actively maintained.
- **DbContext used directly in services.** A repository layer was skipped because EF Core's `DbContext` already is a Repository + Unit of Work. Tests use the EF Core InMemory provider for isolation.
- **`ApplicationUser` lives in Infrastructure, not Domain.** `IdentityUser` is a framework type, so it belongs with infrastructure concerns. Domain entities reference users by `BrokerId` (string) only — keeping Domain framework-free.
- **DI consolidated in Infrastructure.** All registrations live in `Infra/DependencyInjection.cs`. A larger project would split into a separate `AddApp()` extension for App-layer services.
- **Image upload not implemented.** Property images are stored as URL strings. Production would integrate Azure Blob, AWS S3, or local filesystem upload.
- **JWT secret committed in `appsettings.json`.** Suitable only for a demo. Production would use `dotnet user-secrets` or environment variables.
- **Built-in exception types over custom domain exceptions.** `InvalidOperationException`, `UnauthorizedAccessException`, and `KeyNotFoundException` are mapped to HTTP status codes by global middleware. A larger codebase would define custom domain exceptions.
- **Data annotations on Auth DTOs.** Simple rules; FluentValidation packages are referenced for future complex property validators.

## What's not in scope

- Image upload to storage (URLs only)
- Refresh tokens
- Pagination on search results
- API key authentication or rate limiting for third-party clients
- Exhaustive test coverage — commission engine has boundary-case tests; service and controller have representative coverage

## Tests

```bash
dotnet test
```

Covers:
- Commission engine across tier boundaries (`[Theory]` with seven cases)
- Property service: create, ownership-based commission visibility, update/delete reject non-owners
- Controller: create returns 201 with the expected DTO

## License

MIT