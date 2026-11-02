# GePro — Guía para el agente

Solución **.NET 10 (Web API) + Angular 19 (NgModules)**. Se trabaja con **SDD**: ninguna feature se codifica sin spec aprobada.

## Mapa del repo

| Ruta | Qué es |
|---|---|
| `GePro.API` | Controllers, middlewares, DI, JWT, Serilog. Sin lógica de negocio. |
| `GePro.Business` | Servicios y reglas de negocio. Depende de `Model` y de interfaces de `DataAccess`. |
| `GePro.DataAccess` | EF Core, DbContext, Repository / Unit of Work, migraciones. |
| `GePro.Model` | Entidades, DTOs, enums, excepciones de negocio. Sin dependencias. |
| `GePro.API.Tests`, `GePro.Business.Tests` | xUnit. |
| `GePro/` | Frontend Angular (`.esproj`). |
| `Docs/architecture/` | **Cómo debe ser el código de cada capa** (fuente de verdad). |
| `Docs/adr/` | Decisiones de arquitectura. |
| `Docs/specs/` | Specs y tareas por feature. |
| `.claude/skills/` | Skills del flujo SDD. |

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
| Revisar y cerrar | `/sdd` | Checklist de arquitectura; `Estado: Implementada`. |

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
dotnet run --project GePro.API
cd GePro && npm install && npm start     # Angular
cd GePro && npm test
```

## Convenciones (resumen; el detalle está en `Docs/architecture/`)

- **Backend**: async/await con `CancellationToken`, DTOs en la frontera de la API (AutoMapper), sin exponer entidades. Errores como excepciones de `Model/Exceptions` → `ProblemDetails`. Logging con Serilog, sin datos sensibles.
- **Datos**: acceso solo vía repositorios + Unit of Work. Cambios de esquema = migración EF Core.
- **Tests**: un test por criterio de aceptación. Nombres `Metodo_Escenario_Resultado`.
- **Frontend**: módulos por feature (`NgModule`), servicios para HTTP, sin lógica de negocio en componentes.
- **Git**: commits pequeños, uno por tarea de `tasks.md`.

## Documentos de referencia

- `CONTEXT.md` — glosario del dominio.
- `Docs/Guia-Harness.md` — guía para el usuario: qué hace cada archivo del harness y dónde poner cada cosa.
- `Docs/ProjectScaffold.md` — bitácora de scaffold (EF Core, Repository/UoW, AutoMapper, JWT, Serilog).
