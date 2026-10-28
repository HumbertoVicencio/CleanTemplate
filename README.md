# Clean Architecture Template

A pragmatic Clean Architecture starter for **.NET 10**. Batteries included, opinionated where it matters, and easy to extend.

## What's included in the template?

- **SharedKernel** project with common Domain-Driven Design abstractions.
- **Domain** layer with sample entities and domain events.
- **Application** layer with abstractions for:
  - CQRS (lightweight, MediatR-free command/query handlers)
  - Example use cases (Todos and Users)
  - Cross-cutting concerns (logging, validation) implemented as decorators
- **Infrastructure** layer with:
  - JWT authentication with **refresh tokens** (with token rotation)
  - Permission-based authorization
  - EF Core + PostgreSQL (snake_case naming, migrations)
  - **HybridCache** for fast, unified caching with cache invalidation
  - Serilog structured logging
- **Web.Api** layer with:
  - Minimal API endpoints
  - **Rate limiting** (configurable global + authentication policies)
  - **OpenTelemetry** tracing and metrics (ASP.NET Core, HTTP, Npgsql, runtime)
  - Global exception handling and `ProblemDetails`
  - Swagger / OpenAPI with JWT support
- **Seq** for searching and analyzing structured logs
  - Seq is available at http://localhost:8081 by default
- **Testing** projects
  - Architecture testing (`ArchitectureTests`)
  - Unit testing (`Application.UnitTests`)
  - Integration testing with **Testcontainers** (`IntegrationTests`)

## Starting a new project

Rename everything — folders, projects, assemblies, namespaces, EF Core snapshots, Dockerfiles,
compose and the `.claude` skills — in one command:

```bash
./rename-project.sh GestCond            # -> Kcl.Reso.GestCond.*
./rename-project.sh Contoso.Billing     # a dotted name replaces the whole prefix
./rename-project.sh GestCond --dry-run  # preview without touching anything
```

It also updates the infrastructure slug (Postgres database, JWT issuer, container names) and
generates a fresh `UserSecretsId`; pass `--projects-only` to skip that part. The current prefix
is read from the `.slnx`, so the script can be run again on an already-renamed project.

## Getting started

```bash
docker compose up -d        # PostgreSQL + Seq
dotnet run --project src/Kcl.Reso.CleanTemplate.Web.Api
```

Run the full test suite (the integration tests spin up a throwaway PostgreSQL container, so
Docker must be running):

```bash
dotnet test Kcl.Reso.CleanTemplate.slnx
```

To target .NET 8 or .NET 9 instead of .NET 10, see the notes in `Directory.Build.props`.

I'm open to hearing your feedback about the template and what you'd like to see in future iterations.

If you're ready to learn more, check out [**Pragmatic Clean Architecture**](https://www.milanjovanovic.tech/pragmatic-clean-architecture?utm_source=ca-template):

- Domain-Driven Design
- Role-based authorization
- Permission-based authorization
- Distributed caching with Redis
- OpenTelemetry
- Outbox pattern
- API Versioning
- Unit testing
- Functional testing
- Integration testing

Stay awesome!
