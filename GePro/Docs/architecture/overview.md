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
| `DataAccess` | `DbModelContext`, configuración EF (Fluent API), repositorios, Unit of Work, auditoría, borrado lógico, seed, migraciones. | Reglas de negocio, DTOs, validaciones de negocio. |
| `Business` | Servicios con reglas de negocio, mapeo entidad → DTO, orquestación y transacciones. | `HttpContext`, EF Core directo, detalles de transporte. |
| `API` | Controllers finos, manejo de errores, DI, JWT, Serilog, OpenAPI/Swagger. | Lógica de negocio, acceso a datos. |

Regla de oro: **cada capa solo conoce a la de su derecha**. `API` no usa `DataAccess` salvo en el registro de DI (`Program.cs` / extensiones de DI).

## Estructura por feature

Una feature `Xxx` (singular, término de `CONTEXT.md`) toca estos ficheros (rutas reales del repo):

```
GePro.Model/
  Model/Xxx.cs                     (entidad; subcarpeta si es de un grupo: Model/Address/, Model/Person/)
  DTO/XxxDTO.cs
GePro.DataAccess/
  DbModelContext.cs                (DbSet, claves/índices en OnModelCreating, seed si aplica)
  Repositories/SpecificRepositories.cs   (IXxxRepository + XxxRepository, solo si hay consultas propias)
  IUnitOfWork.cs, UnitOfWork.cs    (propiedad del repositorio)
  Migrations/…                     (local; ver data-access.md)
GePro.Business/
  Mapping/XxxProfile.cs            (Xxx → XxxDTO con AutoMapper)
  Services/XxxService.cs           (IXxxService + XxxService en el mismo archivo)
  DependencyInjection.cs           (registro Scoped)
GePro.API/
  Controllers/XxxController.cs
GePro.Business.Tests/XxxServiceTests.cs
GePro.API.Tests/<Tema>/XxxTests.cs
GePro/src/app/features/xxx/        (ver angular.md; el frontend aún es el esqueleto de Angular)
```

## Flujo de una petición

```
HTTP → Serilog request logging / DomainExceptionHandler → [Authorize] Controller
     → Service (reglas) → IUnitOfWork / IXxxRepository → DbModelContext → BD
     ← entidad → el servicio la mapea a DTO ← ProblemDetails si hay error
```

## Transversales

| Tema | Decisión |
|---|---|
| **Errores** | Las reglas rotas lanzan excepciones de `Model/Exceptions`. Hoy existen `NotFoundException` (404) y `UnauthorizedException` (401); `DomainExceptionHandler` (`IExceptionHandler` en `API`) las convierte en `ProblemDetails`. Cualquier otra excepción → 500 sin detalle interno. Las demás (400, 409, 403) se agregan cuando una spec las requiera. |
| **Autenticación** | JWT Bearer (`POST /api/Authentication/Authenticate`). `[Authorize]` en los controllers; `[AllowAnonymous]` solo donde la spec lo pida. El usuario actual llega a `Business` mediante una abstracción (`ICurrentUser`), nunca con `HttpContext`. |
| **Logging** | Serilog configurado desde `appsettings` (consola + sink SQL Server). Sin contraseñas, tokens ni datos personales. Mensajes con plantilla estructurada. |
| **Borrado y auditoría** | Borrado lógico (`ISoftDelete`) con filtro global; `UnitOfWork` rellena `Created/Updated/Deleted` y `*By` con `ICurrentUser`. |
| **Transacciones** | Una operación de negocio = un `IUnitOfWork.SaveChangesAsync()` al final del servicio. |
| **Configuración** | `appsettings*.json` + user-secrets + variables de entorno. Cadena de conexión con contraseña, `JwtTokenConfig:Secret` y credenciales fuera del repo. |
| **Async** | `async/await` en todo I/O, con `CancellationToken` desde el controller hasta el repositorio. |

## Reglas de revisión (resumen)

Una tarea no está terminada si: el código es difícil de leer (cuerpos con flecha, líneas que encadenan varios pasos, nombres abreviados), hay entidades expuestas por la API, un controller con `if` de negocio, un servicio con `DbContext`, una consulta fuera de un repositorio, un cambio de esquema sin migración, o un término de dominio que no está en `CONTEXT.md`.
