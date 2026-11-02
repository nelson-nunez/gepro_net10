# Arquitectura — Visión general

Fuente de verdad de la arquitectura de GePro. Si el código y este documento divergen, se corrige uno de los dos de inmediato (y si es una decisión nueva, se registra un ADR en `Docs/adr/`).

Guías por capa: [model](model.md) · [data-access](data-access.md) · [business](business.md) · [api](api.md) · [angular](angular.md)

## Capas y dependencias

```
GePro.API ──► GePro.Business ──► GePro.DataAccess ──► GePro.Model
    │                                                     ▲
    └─────────────────────────────────────────────────────┘  (API también ve Model: DTOs y excepciones)
```

| Capa | Responsabilidad | NO va aquí |
|---|---|---|
| `Model` | Entidades, DTOs, enums, excepciones de negocio. | Lógica, EF Core, referencias a otros proyectos. |
| `DataAccess` | `DbContext`, configuración EF, repositorios, Unit of Work, migraciones. | Reglas de negocio, DTOs, validaciones de negocio. |
| `Business` | Servicios con reglas de negocio, validaciones, orquestación y transacciones. | `HttpContext`, EF Core directo, detalles de transporte. |
| `API` | Controllers finos, middlewares, DI, JWT, Serilog, perfiles AutoMapper. | Lógica de negocio, acceso a datos. |

Regla de oro: **cada capa solo conoce a la de su derecha**. `API` no usa `DataAccess` salvo en el registro de DI (`Program.cs` / extensiones de DI).

## Estructura por feature

Una feature `Xxx` (singular, término de `CONTEXT.md`) toca estos ficheros:

```
GePro.Model/
  Entities/Xxx.cs
  Dtos/Xxx/XxxDto.cs, CreateXxxDto.cs, UpdateXxxDto.cs
GePro.DataAccess/
  Configurations/XxxConfiguration.cs
  Repositories/IXxxRepository.cs, XxxRepository.cs
  Migrations/…
GePro.Business/
  Services/IXxxService.cs, XxxService.cs
  Validators/…            (si aplica)
GePro.API/
  Controllers/XxxController.cs
  Mapping/XxxProfile.cs
GePro.Business.Tests/Services/XxxServiceTests.cs
GePro.API.Tests/Controllers/XxxControllerTests.cs
GePro/src/app/features/xxx/   (ver angular.md)
```

## Flujo de una petición

```
HTTP → Middleware (Serilog, errores) → [Authorize] Controller
     → mapea DTO ↔ servicio → Service (reglas, validación)
     → IUnitOfWork / IXxxRepository → DbContext → BD
     ← entidad ← AutoMapper → DTO ← ProblemDetails si hay error
```

## Transversales

| Tema | Decisión |
|---|---|
| **Errores** | Las reglas rotas lanzan excepciones de `Model/Exceptions` (`NotFoundException`, `ValidationException`, `ConflictException`, `ForbiddenException`). Un middleware en `API` las convierte en `ProblemDetails` (404, 400, 409, 403). Cualquier otra excepción → 500 sin detalle interno. |
| **Autenticación** | JWT Bearer. `[Authorize]` por defecto en controllers; `[AllowAnonymous]` solo donde la spec lo pida. El usuario actual llega a `Business` mediante una abstracción (`ICurrentUser`), nunca con `HttpContext`. |
| **Logging** | Serilog. Sin contraseñas, tokens ni datos personales. Mensajes con plantilla estructurada. |
| **Transacciones** | Una operación de negocio = un `IUnitOfWork.SaveChangesAsync()` al final del servicio. |
| **Configuración** | `appsettings*.json` + variables de entorno. Secretos fuera del repo. |
| **Async** | `async/await` en todo I/O, con `CancellationToken` desde el controller hasta el repositorio. |

## Reglas de revisión (resumen)

Una tarea no está terminada si: hay entidades expuestas por la API, un controller con `if` de negocio, un servicio con `DbContext`, una consulta fuera de un repositorio, un cambio de esquema sin migración, o un término de dominio que no está en `CONTEXT.md`.
