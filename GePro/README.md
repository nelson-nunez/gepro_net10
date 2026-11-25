# GePro

Solución **.NET 10 (Web API) + Angular 19 (NgModules)** desarrollada con **SDD** (Spec-Driven Development): ninguna feature se codifica sin una spec aprobada.

## Estructura

| Proyecto | Contenido |
|---|---|
| `GePro.API` | Controllers, middlewares, DI, JWT, Serilog. |
| `GePro.Business` | Servicios y reglas de negocio. |
| `GePro.DataAccess` | EF Core, repositorios, Unit of Work, migraciones. |
| `GePro.Model` | Entidades, DTOs, enums, excepciones de negocio. |
| `GePro.API.Tests`, `GePro.Business.Tests` | Tests xUnit. |
| `GePro/` | Frontend Angular (`GePro.UI.esproj`). |

Dependencias: `API → Business → DataAccess → Model`.

## Comandos

```bash
dotnet build GePro.slnx
dotnet test GePro.slnx
dotnet run --project GePro.API
cd GePro && npm install && npm start     # Angular
cd GePro && npm test
```

Con la API en `Development`, Swagger UI está en `/swagger`.

## Flujo de trabajo (SDD)

```
/sdd  →  grill-with-docs → to-spec → (apruebas) → to-tasks → tdd (por tarea) → revisión → cierre
```

- Una spec = una rama `feature/NNN-nombre`. Un commit por tarea (`feat(NNN): …`).
- La spec nueva se pushea al crearla; el código se pushea tras build y tests en verde y aprobación del desarrollador.
- Comandos y reglas: [`CLAUDE.md`](CLAUDE.md) · Guía para el desarrollador: [`Personal/Guia-Harness.md`](Personal/Guia-Harness.md).

## Documentación

- [`CONTEXT.md`](CONTEXT.md): glosario del dominio.
- [`Docs/architecture/`](Docs/architecture/overview.md): reglas de código por capa.
- [`Docs/adr/`](Docs/adr/): decisiones de arquitectura.
- [`Docs/specs/`](Docs/specs/): specs y tareas por feature.
