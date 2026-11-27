# Capa API (`GePro.API`)

Transporte HTTP y composición. Sin lógica de negocio.

## Estructura real

```
GePro.API/
  Program.cs                      # composición: Serilog, OpenAPI, DI, JWT, pipeline
  Controllers/                    # Address, Authentication, Customer, Gender
  Contracts/AuthContracts.cs      # SigninRequest, AuthTokenResponse
  Extensions/AuthenticationExtensions.cs   # AddJwtAuthentication()
  Middleware/DomainExceptionHandler.cs     # IExceptionHandler -> ProblemDetails
  Security/                       # JwtTokenConfig, JwtTokenService, HttpCurrentUser, BearerSecurityTransformer
  Properties/launchSettings.json
  appsettings.json, appsettings.Development.json
  Dockerfile, GePro.API.http
```

## Reglas

- **Controllers finos**: `[ApiController]`, ruta `api/[controller]` (`AddressController` usa `api` con rutas `provinces`, `provinces/{id}/cities`, `cities/{id}/neighborhoods`). Cada acción: recibir, llamar al servicio, devolver. Nada de `if` de negocio ni acceso a repositorios.
- Códigos explícitos con `[ProducesResponseType]` (`ProducesResponseType<T>`); errores como `ProblemDetails`.
- **Errores**: `DomainExceptionHandler` (`IExceptionHandler`, registrado con `AddExceptionHandler` y `UseExceptionHandler()`) traduce `NotFoundException` → 404 y `UnauthorizedException` → 401. Cualquier otra excepción sigue al 500 genérico. Los controllers no usan `try/catch`.
- **Auth**: JWT Bearer. `[Authorize]` en los controllers, `[AllowAnonymous]` solo en `AuthenticationController`. `AddJwtAuthentication()` valida al arrancar `JwtTokenConfig` (`Secret` ≥ 32 caracteres, `Issuer`, `Audience`) y registra `ICurrentUser` (`HttpCurrentUser`) y `IJwtTokenService`.
- **DTOs en la frontera**: los controllers devuelven DTOs de `GePro.Model.DTO` o contratos de `Contracts/`. El mapeo entidad → DTO ocurre en `Business`. No hay AutoMapper.
- **DI**: `AddDataAccess(connectionString)`, `AddBusiness()` y `AddJwtAuthentication()` se llaman desde `Program.cs`. `API` referencia `DataAccess` solo para eso y para `ICurrentUser`.
- **Serilog**: `UseSerilog` con `ReadFrom.Configuration` + consola; `UseSerilogRequestLogging()`. El sink SQL (`Serilog.Sinks.MSSqlServer`) se declara en `appsettings`. Sin datos sensibles.
- **OpenAPI/Swagger**: `AddOpenApi` con `BearerSecurityTransformer` (esquema Bearer JWT, botón *Authorize*). `MapOpenApi()` (`/openapi/v1.json`) y `UseSwaggerUI` (`/swagger`) solo en `Development`.
- **Configuración**: `ConnectionStrings:DefaultConnection`, `JwtTokenConfig` (`Secret`, `Issuer`, `Audience`, `AccessTokenExpiration`) y `Serilog` en `appsettings*.json`. El `Secret` y las contraseñas de base van en user-secrets (`UserSecretsId` ya definido) o variables de entorno, nunca en un archivo versionado.
- **CORS**: aún no configurado; se agrega cuando el frontend consuma la API.

## Ejemplo

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    [HttpGet("{id:int}")]
    [ProducesResponseType<CustomerDTO>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDTO>> GetById(int id, CancellationToken ct)
    {
        var customer = await customerService.GetByIdAsync(id, ct);
        return Ok(customer);
    }
}
```

## Tests

- `GePro.API.Tests` (xUnit + `Microsoft.AspNetCore.Mvc.Testing`). Se levanta la app con `WebApplicationFactory<Program>` y `UseSetting` para `ConnectionStrings:DefaultConnection` y `JwtTokenConfig:Secret` ficticios. Cubren auth (401 sin token), Swagger y el documento OpenAPI.
- `Program` es `public partial class` para poder usarlo en los tests.

## Errores típicos

- Lógica de validación de negocio en el controller.
- Devolver entidades en lugar de DTOs.
- `try/catch` en cada acción.
- Endpoint sin `[Authorize]` y sin que la spec lo diga.
- Subir credenciales a `appsettings*.json`.
