# ADR-0001: Arquitectura en capas con Repository / Unit of Work

- **Estado**: Aceptada
- **Fecha**: 2026-10-31

## Contexto

GePro es una Web API .NET 10 con frontend Angular 19. Se necesita una separación clara entre transporte, negocio y persistencia para poder testear la lógica sin base de datos.

## Decisión

Cuatro proyectos con dependencia unidireccional: `API → Business → DataAccess → Model`.

- `Model`: entidades, DTOs, excepciones de negocio.
- `DataAccess`: EF Core, repositorios, Unit of Work.
- `Business`: servicios con reglas de negocio; usan `IUnitOfWork` (interfaz de `DataAccess`) y mapean entidad → DTO con AutoMapper.
- `API`: controllers finos, JWT, Serilog, OpenAPI/Swagger.

> Actualización (documentación alineada con el código): el borrador original citaba AutoMapper en `API`; el mapeo a DTO se hace en los servicios de `Business` con AutoMapper (Profiles en `GePro.Business/Mapping`, spec 002); `API` no lo referencia.

## Consecuencias

- La lógica de negocio se prueba en `GePro.Business.Tests` con repositorios mockeados.
- Más ficheros por feature (entidad, DTO, repo, servicio, controller), a cambio de límites claros.
- Cualquier atajo que salte una capa requiere un ADR nuevo.

## Alternativas descartadas

- Usar `DbContext` directamente en servicios: más rápido al inicio, pero acopla negocio a EF Core.
