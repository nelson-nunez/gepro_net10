# Capa Business (`GePro.Business`)

Depende de `Model` y de `DataAccess` (usa `IUnitOfWork`; paquete `Microsoft.Extensions.Identity.Core` para el hash de contraseñas). Aquí viven las reglas del negocio.

## Estructura real

```
GePro.Business/
  DependencyInjection.cs         # AddBusiness()
  Services/                      # interfaz + implementación en el mismo archivo
    AuthenticationService.cs, CustomerService.cs, ReferenceDataServices.cs
  Security/IPasswordService.cs, PasswordService.cs
```

`ReferenceDataServices.cs` agrupa los servicios de datos de referencia (`Province`, `City`, `Neighborhood`, `Gender`).

## Reglas

- Un servicio por agregado: `IXxxService` / `XxxService`, registrado como `Scoped` en `AddBusiness()` (`PasswordService` es `Singleton`).
- El servicio recibe `IUnitOfWork` (nunca `DbContext`) y devuelve **DTOs**. No conoce `HttpContext` ni `IActionResult`.
- **Mapeo entidad → DTO a mano** dentro del servicio (método privado `ToDto`). No hay AutoMapper.
- Las reglas rotas se señalan con excepciones de `Model/Exceptions`:
  - no existe → `NotFoundException`
  - credenciales inválidas o sin sesión → `UnauthorizedException`
  - (otras excepciones se crean cuando una spec las pida; ver `model.md`)
- Una operación = una transacción: leer, validar, modificar y llamar **una vez** a `IUnitOfWork.SaveChangesAsync(ct)`.
- Usuario actual: `ICurrentUser` (definida en `DataAccess` para la auditoría, implementada en `API`). Los servicios no la usan directamente salvo que una spec lo requiera.
- Sin `static` con estado. Para fechas, `TimeProvider`.
- Todos los métodos `async` con `CancellationToken`.

## Ejemplo

```csharp
public class CustomerService(IUnitOfWork uow) : ICustomerService
{
    public async Task<CustomerDTO> GetByIdAsync(int id, CancellationToken ct)
    {
        var customer = await uow.Customers.GetDetailAsync(id, ct)
            ?? throw new NotFoundException($"No existe el cliente {id}.");
        return ToDto(customer);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var customer = await uow.Customers.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"No existe el cliente {id}.");

        uow.Customers.Remove(customer);
        await uow.SaveChangesAsync(ct);
    }
}
```

## Tests

- `GePro.Business.Tests` (xUnit). No hay librería de mocks: se usan dobles escritos a mano (`FakeUnitOfWork`, `FakeAppUsers`) que implementan `IUnitOfWork` y los repositorios.
- Un test por criterio de aceptación y uno por cada excepción que el servicio puede lanzar.
- Nombres `Metodo_Escenario_Resultado`.

## Errores típicos

- Devolver entidades al controller.
- Llamar a `SaveChangesAsync` varias veces en una misma operación.
- Capturar excepciones para devolver `null` o `bool` en lugar de dejar que `DomainExceptionHandler` las traduzca.
- Revelar si falló el usuario o la contraseña (el mensaje de `AuthenticationService` es el mismo en ambos casos).
