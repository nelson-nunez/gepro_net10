# Tareas — 002 Mapeo con AutoMapper

> Cada tarea es una **rebanada vertical** (atraviesa las capas necesarias, se puede probar y commitear sola).
> Se ejecutan en orden con `/tdd`. Un commit por tarea.

| # | Tarea | Criterios | Capas | Estado |
|---|---|---|---|---|
| 1 | Tracer bullet: paquete AutoMapper 14.x + registro en `AddBusiness()` + `GenderProfile` + `GenderService` migrado + test de validación de configuración | CA-01, CA-06 (parcial) | Business, Tests | ☑ |
| 2 | `ProvinceProfile`, `CityProfile`, `NeighborhoodProfile` y servicios de referencia migrados | CA-04, CA-06 | Business, Tests | ☑ |
| 3 | `CustomerProfile` y `CustomerService` sin `ToDto` | CA-02, CA-03 | Business, Tests | ☑ |
| 4 | `EmployeeProfile` (`Employee` → `EmployeeDTO`) | CA-02b | Business, Tests | ☐ |
| 5 | `AppUserProfile`, `AuthenticationService` migrado y `AuthenticationServiceTests` actualizados | CA-05 | Business, Tests | ☐ |
| 6 | Cierre: verificación de que no quedan mapeos manuales, API sin referencia a AutoMapper, documentación actualizada | CA-07, CA-08, CA-09 | Business, API, Docs | ☐ |

## Notas por tarea

### Tarea 1
- **Test rojo primero**: `MappingConfigurationTests.AssertConfigurationIsValid_ProfilesDeBusiness_NoLanza` y `GenderProfileTests` (mapea `Gender` → `GenderDTO`). Falla porque no existe el paquete ni el Profile.
- **Q-04**: tras agregar el paquete, ejecutar `dotnet list package --vulnerable` e informar el resultado antes de seguir.
- **Hecho cuando**: `GenderService.ListAsync` devuelve `IReadOnlyList<GenderDTO>` vía `IMapper` y `dotnet test GePro.slnx` pasa.

### Tarea 2
- **Test rojo primero**: un test por Profile (`City` con su `Province` → `ProvinceName`; `Neighborhood` con su `City` → `CityName`; `Province` → `ProvinceDTO`) y un test de servicio que conserve el orden por `Name` de las provincias.
- **Hecho cuando**: `ReferenceDataServices.cs` no construye ningún `new XxxDTO { … }`.

### Tarea 3
- **Test rojo primero**: `CustomerProfileTests` (todos los campos de `CustomerDTO`, `PersonAge`, barrio nulo). Comparar contra los valores que producía `CustomerService.ToDto`.
- **Hecho cuando**: `CustomerService` inyecta `IMapper` y no existe `ToDto`.

### Tarea 4
- **Test rojo primero**: `EmployeeProfileTests` (persona y dirección aplanadas, campos propios, nulos de `Department`, `TerminationDate`, `AppUserId`, `TaxId`).
- **Hecho cuando**: `AssertConfigurationIsValid()` sigue en verde con el nuevo Profile.

### Tarea 5
- **Test rojo primero**: `AppUserProfileTests` (`RoleName` y `Permissions`) y `AuthenticationServiceTests` construyendo el servicio con un `IMapper` real.
- **Hecho cuando**: `AuthenticationService` ya no construye `AuthenticatedUserDTO` a mano.

### Tarea 6
- **Verificación**: búsqueda en `GePro.Business/Services` de `ToDto` y `new .*DTO {`; `GePro.API.csproj` sin `AutoMapper`; `dotnet test GePro.slnx` completo.
- **Documentación**: actualizar `Docs/architecture/business.md`, `api.md`, `overview.md`, `CLAUDE.md` y la nota del ADR-0001 (hoy dicen "mapeo manual, sin AutoMapper").
- **Hecho cuando**: todos los CA de la spec tienen test y la spec pasa a `Implementada` (lo hace `/sdd` al cerrar).
