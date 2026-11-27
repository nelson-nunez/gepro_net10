# Tareas — 001 Swagger en la API

| # | Tarea | Criterios | Capas | Estado |
|---|---|---|---|---|
| 1 | Swagger UI solo en Development + perfiles que lo abren | CA-01, CA-02, CA-03, CA-04 | API | ☑ |
| 2 | Esquema Bearer JWT en OpenAPI (botón Authorize) | CA-05 | API | ☑ |

## Notas por tarea

### Tarea 1
- **Test rojo primero**: integración con `WebApplicationFactory` (Development → 200, Production → 404) y test de `launchSettings.json`.
- **Hecho cuando**: F5 abre `/swagger` y los tests pasan.

### Tarea 2
- **Test rojo primero**: `GetOpenApiDocument_Development_DeclaraEsquemaBearerJwt` (en `SwaggerTests`) lee `/openapi/v1.json` y comprueba el esquema `Bearer` y el requisito `security`.
- **Implementación**: `GePro.API/Security/BearerSecurityTransformer.cs` (`IOpenApiDocumentTransformer`), registrado en `Program.cs` con `AddOpenApi(options => options.AddDocumentTransformer<BearerSecurityTransformer>())`.
- **Hecho cuando**: Swagger UI muestra *Authorize* y el test pasa.
