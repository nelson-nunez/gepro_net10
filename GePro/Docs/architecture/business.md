# Capa Business (`GePro.Business`)

Depende de `Model` y de las **interfaces** de `DataAccess`. Aquí viven las reglas del negocio.

## Reglas

- Un servicio por agregado: `IXxxService` / `XxxService`, registrado como `Scoped`.
- El servicio recibe y devuelve **DTOs** (o tipos de `Model`). No conoce `HttpContext`, `IActionResult` ni `DbContext`.
- Usa AutoMapper solo si el mapeo es trivial; las transformaciones con reglas se escriben a mano en el servicio.
- Las reglas rotas se señalan con excepciones de `Model/Exceptions`:
  - no existe → `NotFoundException`
  - dato inválido → `ValidationException` (con la lista de errores por campo)
  - duplicado o estado incompatible → `ConflictException`
  - sin permiso → `ForbiddenException`
- Validación de entrada: primero formato (FluentValidation o validaciones explícitas), luego reglas de negocio que consultan datos.
- Una operación = una transacción: leer, validar, modificar y llamar **una vez** a `IUnitOfWork.SaveChangesAsync(ct)`.
- Usuario actual: mediante `ICurrentUser` (interfaz definida en `Business`, implementada en `API`).
- Sin `static` con estado, sin acceso al reloj directo: usar `TimeProvider`.
- Todos los métodos `async` con `CancellationToken`.

## Ejemplo

```csharp
public class ProyectoService(IUnitOfWork uow, IMapper mapper, TimeProvider clock) : IProyectoService
{
    public async Task<ProyectoDto> CrearAsync(CreateProyectoDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            throw new ValidationException(nameof(dto.Nombre), "El nombre es obligatorio.");

        if (await uow.Proyectos.ExistePorNombreAsync(dto.Nombre, ct))
            throw new ConflictException($"Ya existe un proyecto llamado '{dto.Nombre}'.");

        var proyecto = new Proyecto { Nombre = dto.Nombre, Estado = EstadoProyecto.Borrador, CreatedAt = clock.GetUtcNow() };
        await uow.Proyectos.AddAsync(proyecto, ct);
        await uow.SaveChangesAsync(ct);

        return mapper.Map<ProyectoDto>(proyecto);
    }
}
```

## Tests

- `GePro.Business.Tests`, xUnit, repositorios/UoW mockeados (NSubstitute o Moq, el que ya use el proyecto).
- Un test por criterio de aceptación, y uno por cada excepción que el servicio puede lanzar.
- Nombres `Metodo_Escenario_Resultado`: `CrearAsync_NombreDuplicado_LanzaConflictException`.

## Errores típicos

- Devolver entidades al controller.
- Llamar a `SaveChangesAsync` varias veces en una misma operación.
- Capturar excepciones para devolver `null` o `bool` en lugar de dejar que el middleware las traduzca.
