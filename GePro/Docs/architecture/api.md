# Capa API (`GePro.API`)

Transporte HTTP y composición. Sin lógica de negocio.

## Reglas

- **Controllers finos**: `[ApiController]`, ruta `api/[controller]`, un método por caso de uso. Cada acción hace tres cosas: recibir DTO, llamar al servicio, devolver resultado. Nada de `if` de negocio ni acceso a repositorios.
- Códigos de respuesta explícitos con `[ProducesResponseType]`: 200/201/204 en éxito; 400, 401, 403, 404, 409 según la spec. Los `POST` que crean devuelven `201` con `CreatedAtAction`.
- **Errores**: un middleware (`ExceptionHandlingMiddleware` o `IExceptionHandler`) traduce las excepciones de `Model/Exceptions` a `ProblemDetails`. Los controllers no usan `try/catch` para esto.
- **Auth**: JWT Bearer configurado en `Program.cs`/extensiones. `[Authorize]` en todos los controllers salvo excepción en la spec. Roles/políticas con nombres de `CONTEXT.md`.
- **AutoMapper**: perfiles en `Mapping/`, uno por feature. `AssertConfigurationIsValid` en un test.
- **DI**: un método de extensión por capa (`AddBusiness()`, `AddDataAccess(config)`) llamado desde `Program.cs`. Es el único sitio donde `API` referencia `DataAccess`.
- **Serilog**: configurado desde `appsettings`; `UseSerilogRequestLogging()`. Sin datos sensibles.
- **CORS**: solo el origen del frontend configurado.
- **OpenAPI**: cada endpoint documentado; el contrato publicado debe coincidir con la sección "Contrato de API" de la spec.
- Eliminar el código de plantilla (`WeatherForecast`) cuando se implemente la primera feature.

## Ejemplo

```csharp
[ApiController, Route("api/[controller]"), Authorize]
public class ProyectosController(IProyectoService service) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<ProyectoDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProyectoDto>> Crear(CreateProyectoDto dto, CancellationToken ct)
    {
        var creado = await service.CrearAsync(dto, ct);
        return CreatedAtAction(nameof(Obtener), new { id = creado.Id }, creado);
    }
}
```

## Tests

- `GePro.API.Tests`: pruebas de controller con servicio mockeado, y de integración con `WebApplicationFactory` para el contrato (códigos y forma del `ProblemDetails`).

## Errores típicos

- Lógica de validación de negocio en el controller.
- Devolver entidades o anónimos en lugar de DTOs.
- `try/catch` en cada acción.
- Endpoint sin `[Authorize]` y sin que la spec lo diga.
