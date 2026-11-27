# GePro — Guía para el agente

Solución **.NET 10 (Web API) + Angular 19 (NgModules)**. Se trabaja con **SDD**: ninguna feature se codifica sin spec aprobada.

## Mapa del repo

| Ruta | Qué es |
|---|---|
| `GePro.API` | Controllers, `DomainExceptionHandler`, DI, JWT, Serilog, OpenAPI/Swagger. Sin lógica de negocio. |
| `GePro.Business` | Servicios y reglas de negocio; mapean entidad → DTO con AutoMapper (`Mapping/*Profile`). Usa `IUnitOfWork` de `DataAccess`. |
| `GePro.DataAccess` | EF Core (SQL Server), `DbModelContext`, Repository / Unit of Work, borrado lógico y auditoría, migraciones. |
| `GePro.Model` | Entidades (`Model/`), DTOs (`DTO/`), `Base/`, excepciones de negocio. Sin dependencias. |
| `GePro.API.Tests`, `GePro.Business.Tests` | xUnit. |
| `GePro/` | Frontend Angular (`GePro.UI.esproj`). |
| `Docs/architecture/` | **Cómo debe ser el código de cada capa** (fuente de verdad). |
| `Docs/adr/` | Decisiones de arquitectura. |
| `Docs/specs/` | Specs y tareas por feature. |
| `.claude/skills/` | Skills del flujo SDD (`sdd`, `grill-with-docs`, `to-spec`, `to-tasks`, `tdd`, `commit-sdd`). |
| `.claude/settings.json`, `.claude/hooks/verify.sh` | Permisos (`dotnet build/test` y `npm test` permitidos; **lectura de `appsettings.*.json` denegada**) y hook `Stop`: al terminar corre `dotnet build` y `dotnet test`; si fallan, devuelve el error para corregirlo. |
| `Personal/` | Notas y bitácora del usuario (guía del harness, scaffold, Angular). No son reglas. |

Dependencias permitidas: `API → Business → DataAccess → Model`. Nunca al revés. `API` no toca `DataAccess` directamente salvo en el registro de DI.

## Flujo SDD (obligatorio)

**El líder es `/sdd <spec>`**: lee el estado de la feature y ejecuta o propone el paso siguiente. Sin argumentos lista el estado de todas las specs.

```
/sdd  →  grill-with-docs → to-spec → (apruebas) → to-tasks → tdd (por tarea) → revisión → cierre
```

| Fase | Skill | Resultado |
|---|---|---|
| Descubrir | `/grill-with-docs` | Acuerdo; `CONTEXT.md` y ADRs actualizados. |
| Especificar | `/to-spec` | `Docs/specs/NNN-nombre/spec.md` (`Borrador`). |
| Aprobar | el usuario | `Estado: Aprobada`. |
| Planificar | `/to-tasks` | `Docs/specs/NNN-nombre/tasks.md`. |
| Implementar | `/tdd` | Una tarea: red → green → refactor. |
| Revisar y cerrar | `/sdd` | Checklist de arquitectura; `Estado: Implementada`; push con aprobación. |

Ramas y commits en cada fase los hace `/commit-sdd` (invocado por `to-spec`, `tdd` y `sdd`).

Reglas:
- No escribir código de producción si no existe una spec con `Estado: Aprobada`.
- Si el código y la spec divergen, se actualiza la spec primero y se avisa.
- Los criterios de aceptación de la spec son la definición de "terminado".
- Usar los términos de `CONTEXT.md` en clases, métodos, DTOs y textos de UI.

## Arquitectura: leer antes de escribir código

Antes de tocar una capa, lee su guía en `Docs/architecture/`:

| Capa | Guía |
|---|---|
| Visión general, flujo, errores, auth, logging | `overview.md` |
| `GePro.Model` | `model.md` |
| `GePro.DataAccess` | `data-access.md` |
| `GePro.Business` | `business.md` |
| `GePro.API` | `api.md` |
| `GePro/` (Angular) | `angular.md` |

Si una guía y el código discrepan, se corrige uno de los dos en el momento. Una decisión nueva de arquitectura = ADR nuevo en `Docs/adr/`.

## Comandos

```bash
dotnet build GePro.slnx
dotnet test GePro.slnx
dotnet run --project GePro.API --launch-profile https   # Swagger en /swagger (solo Development)
cd GePro && npm install && npm start     # Angular (GePro/GePro)
cd GePro && npm test
```

Migraciones (Package Manager Console; el proyecto de inicio es siempre la API):

```
Add-Migration NombreDescriptivo -Project GePro.DataAccess -StartupProject GePro.API
Update-Database -Project GePro.DataAccess -StartupProject GePro.API
```

Configuración local: la API necesita `ConnectionStrings:DefaultConnection` y `JwtTokenConfig:Secret` (≥ 32 caracteres) en user-secrets (`dotnet user-secrets set … --project GePro.API`) o `appsettings.Development.json`. Nunca commitear contraseñas ni secretos. La raíz de Git es la carpeta superior a `GePro/`; las rutas de `git` salen con prefijo `GePro/`.

## Convenciones (resumen; el detalle está en `Docs/architecture/`)

- **Legibilidad primero**: se prioriza que el código se lea fácil sobre que sea corto o ingenioso. Cuerpos de método en bloque (sin `=>` en miembros), un paso por línea con variables de nombre claro, métodos pequeños con nombre que explica su intención, sin abreviaturas ni código muerto.
- **Backend**: async/await con `CancellationToken`, DTOs en la frontera de la API (AutoMapper 14 en `GePro.Business/Mapping`, un `<Entidad>Profile` por entidad; `API` no referencia AutoMapper), sin exponer entidades. Errores como excepciones de `Model/Exceptions` → `ProblemDetails`. Logging con Serilog, sin datos sensibles.
- **Datos**: acceso solo vía repositorios + Unit of Work. Borrado lógico (`ISoftDelete`) y auditoría en `UnitOfWork`. Cambios de esquema = migración EF Core.
- **Tests**: xUnit; un test por criterio de aceptación. Nombres `Metodo_Escenario_Resultado`. Sin librería de mocks: dobles escritos a mano en `Business.Tests`, `WebApplicationFactory<Program>` en `API.Tests`.
- **Frontend**: módulos por feature (`NgModule`), servicios para HTTP, sin lógica de negocio en componentes.
- **Git** (lo gestiona `/commit-sdd`): una rama por spec, `feature/NNN-nombre`; commits pequeños, uno por tarea de `tasks.md`, formato `tipo(NNN): descripción`. Spec nueva → rama + commit + push. Código → push solo con build/tests en verde y aprobación del desarrollador. Nunca implementar en `main`.

## Documentos de referencia

- `CONTEXT.md` — glosario del dominio.
- `Personal/Guia-Harness.md` — guía para el usuario: qué hace cada archivo del harness y dónde poner cada cosa.
- `Personal/ProjectScaffold.md` — bitácora histórica del scaffold (EF Core, Repository/UoW, JWT, Serilog). Parte de ella describe pasos antiguos (por ejemplo AutoMapper, `Startup`); manda el código y `Docs/architecture/`.
