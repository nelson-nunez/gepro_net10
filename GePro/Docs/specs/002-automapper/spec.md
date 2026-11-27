# 002 — Mapeo con AutoMapper

- **Estado**: Implementada
- **Aprobación**: explícita del usuario (2026-11-27), con las decisiones D-01 a D-03 de la sección 13
- **Fecha**: 2026-11-27
- **Capas afectadas**: Business / API (registro en DI) / Tests

## 1. Problema y objetivo

Hoy el paso entidad → DTO está escrito a mano y repetido en los servicios de `Business` (un método `ToDto` de 23 asignaciones en `CustomerService` y cuatro `new XxxDTO { … }` en `ReferenceDataServices`). Añadir un campo a un DTO obliga a recordar todos los sitios donde se construye. El objetivo es centralizar el mapeo en `Profile`s de AutoMapper, con un test que falle si un DTO queda con propiedades sin origen.

## 2. Análisis del estado actual

Mapeos entre entidades y DTOs que existen hoy (verificado en el código):

| Archivo | Clase / método | Origen → destino | Observaciones |
|---|---|---|---|
| `GePro.Business/Services/CustomerService.cs` | `CustomerService.ToDto` (estático, con `=>`) | `Customer` → `CustomerDTO` | 23 asignaciones; aplana `Person` y `Person.Address`. Se usa en `SearchAsync` (`Select(ToDto)`) y `GetByIdAsync`. |
| `GePro.Business/Services/ReferenceDataServices.cs` | `ProvinceService.ListAsync` | `Province` → `ProvinceDTO` | Lambda `Select(p => new ProvinceDTO { … })` con `OrderBy` previo. |
| ídem | `CityService.ListByProvinceAsync` | `City` → `CityDTO` | `ProvinceName = c.Province.Name`. |
| ídem | `NeighborhoodService.ListByCityAsync` | `Neighborhood` → `NeighborhoodDTO` | `CityName = n.City.Name`. |
| ídem | `GenderService.ListAsync` | `Gender` → `GenderDTO` | Solo `Id` y `Name`. |
| `GePro.Business/Services/AuthenticationService.cs` | `AuthenticateAsync` | `AppUser` → `AuthenticatedUserDTO` | `RoleName = Role.Name`, `Permissions = Role.Permissions`. |

Sin mapeo en controllers: los controllers de `GePro.API` devuelven lo que entrega el servicio. No hay view models, extensiones de mapeo ni `AutoMapper` instalado.

No son mapeos de entidad y quedan como están: `JwtTokenService.CreateAccessToken` (construye `AuthTokenResponse` con el token y la expiración) y `AuthenticationController` (usa `SigninRequest.UserName/Password`).

Sin uso hoy: `EmployeeDTO` (no existe servicio ni endpoint de `Employee`). Por decisión del usuario se le crea igualmente su Profile (ver sección 4). `PersonDTO` solo es base de `CustomerDTO` y `EmployeeDTO`.

## 3. Alcance

**Dentro**
- Paquete `AutoMapper` **versión 14.x** en `GePro.Business` (decisión del usuario).
- Profiles en `GePro.Business/Mapping/` que reemplazan los 6 mapeos de la tabla anterior, más un Profile nuevo para `Employee` → `EmployeeDTO` (sin servicio ni endpoint todavía).
- Eliminar `CustomerService.ToDto` y las construcciones `new XxxDTO { … }` de los servicios; los servicios reciben `IMapper` por constructor.
- Registro en DI (ver sección 5).
- Test de validación de configuración y tests de mapeo por Profile.
- Actualizar los servicios existentes y sus tests (`AuthenticationServiceTests`) para recibir `IMapper`.
- Actualizar documentación: `Docs/architecture/business.md`, `api.md`, `overview.md`, `CLAUDE.md` (hoy dicen "mapeo manual, sin AutoMapper") y la nota del ADR-0001.

**Fuera**
- Mapeo DTO → entidad (altas y modificaciones): hoy no existen endpoints de escritura salvo `DELETE`.
- `AuthTokenResponse` y los contratos de `GePro.API/Contracts` (no son entidades).
- `ProjectTo` / proyección en consulta (ver sección 6.5).
- Cambios en entidades, DTOs, esquema de base de datos o contrato de API: el JSON de respuesta debe quedar idéntico.
- Reglas de ordenamiento: siguen en los servicios (`OrderBy` antes de mapear) y en los repositorios.
- Servicio, controller o endpoint de `Employee`: solo se crea el Profile, que queda cubierto por test.

## 4. Proyecto de los Profiles y convención de nombres

- **Proyecto**: `GePro.Business`, carpeta `Mapping/`, namespace `GePro.Business.Mapping`. Motivo: el mapeo entidad → DTO ya vive en `Business` y la arquitectura prohíbe que `API` conozca entidades en la lógica; así `Business.Tests` puede validarlos sin levantar la API.
- **Convención**: un Profile por entidad de origen, `<Entidad>Profile` (clase pública que hereda de `AutoMapper.Profile`), mapeos declarados en el constructor:

| Profile | Mapeos |
|---|---|
| `CustomerProfile` | `Customer` → `CustomerDTO` |
| `EmployeeProfile` | `Employee` → `EmployeeDTO` (mismo aplanado de `Person` y `Address` que `CustomerProfile`; los campos propios `FileNumber`, `Position`, `Department`, `HireDate`, `TerminationDate`, `AppUserId`, `TaxId` coinciden por nombre) |
| `ProvinceProfile` | `Province` → `ProvinceDTO` |
| `CityProfile` | `City` → `CityDTO` |
| `NeighborhoodProfile` | `Neighborhood` → `NeighborhoodDTO` |
| `GenderProfile` | `Gender` → `GenderDTO` |
| `AppUserProfile` | `AppUser` → `AuthenticatedUserDTO` |

- Los lambdas de `ForMember` se escriben en varias líneas, un paso por línea, según la regla de legibilidad de `CLAUDE.md`; sin `=>` en miembros (el constructor del Profile usa bloque).

## 5. Registro en DI

- `GePro.Business.DependencyInjection.AddBusiness()` registra AutoMapper con el ensamblado de Business: `services.AddAutoMapper(cfg => { }, typeof(CustomerProfile).Assembly)` (AutoMapper 14 incluye la integración con DI; no hace falta el paquete `AutoMapper.Extensions.Microsoft.DependencyInjection`). Decisión del usuario: el registro va en `AddBusiness()`.
- `Program.cs` de `GePro.API` (proyecto de inicio) no cambia: ya llama a `AddBusiness()`. Así `API` no referencia AutoMapper ni los Profiles.
- Ciclo de vida: `IMapper` es el registrado por AutoMapper (scoped); los servicios siguen `Scoped`.

## 6. Reglas de mapeo y casos especiales

1. **Convención y aplanado**: se aprovecha el aplanado por nombre de AutoMapper donde coincide (`Customer.Person.Name` → `PersonName`, `Person.LastName` → `PersonLastName`, `Person.DNI` → `PersonDNI`, `Person.BirthDate` → `PersonBirthDate`, `Person.Phone` → `PersonPhone`, `Person.Email` → `PersonEmail`; `City.Province.Name` → `ProvinceName`; `Neighborhood.City.Name` → `CityName`; `AppUser.Role.Name` → `RoleName`). Las propiedades de auditoría (`Created`, `Updated`, `Deleted`) coinciden por nombre.
2. **`ForMember` explícito** donde el nombre del DTO no coincide con el camino de la entidad:
   - `CustomerDTO.GenderId` ← `Person.GenderId`; `AddressId` ← `Person.AddressId`.
   - `CustomerDTO.ProvinceId`, `CityId`, `NeighborhoodId`, `NeighborhoodName`, `StreetNumber` ← `Person.Address.*`.
   - `AuthenticatedUserDTO.Permissions` ← `Role.Permissions`.
3. **Propiedad calculada**: `PersonAge` ← `Person.Age` (propiedad `[NotMapped]` de la entidad, calculada desde `BirthDate`). La lógica del cálculo no se mueve ni se duplica en el Profile.
4. **Anidados y nulos**: `NeighborhoodId`/`NeighborhoodName` son opcionales; el mapeo conserva `null`. No se mapean entidades anidadas dentro de DTOs (los DTOs son planos).
5. **Colecciones**: los servicios devuelven `IReadOnlyList<TDto>` mediante `mapper.Map<IReadOnlyList<TDto>>(entities)` (o `List<TDto>`); no se declaran mapeos de colección en los Profiles. El orden lo define el servicio antes de mapear.
6. **`ProjectTo` con EF Core**: **no se usa**. Los repositorios devuelven entidades (`IReadOnlyList<T>`) y no exponen `IQueryable` (regla de `data-access.md`), por lo que `ProjectTo` obligaría a romper esa regla. Además las consultas actuales cargan relaciones con `Include`. Queda anotado como posible optimización futura (ADR propio).
7. **Ignorados**: no se espera `Ignore()` en los mapeos de esta spec; cada propiedad del DTO tiene origen. Si el test de validación (sección 7) revela una propiedad sin origen, se decide caso a caso y se documenta en el Profile.
8. **Proxies de lazy loading**: la API usa `UseLazyLoadingProxies`; los mapeos leen propiedades `virtual` ya cargadas por `Include`. El comportamiento (consultas ejecutadas) no debe cambiar.

## 7. Validación de configuración

- Un test en `GePro.Business.Tests` (`MappingConfigurationTests`) construye `MapperConfiguration` con los Profiles de `GePro.Business` y llama a `AssertConfigurationIsValid()`.
- Un test por Profile verifica el mapeo con una entidad de ejemplo, incluidos `CustomerDTO` (aplanado, `PersonAge`, `NeighborhoodId` nulo) y `AuthenticatedUserDTO`.
- Los tests de servicios que hoy construyen el servicio a mano (`AuthenticationServiceTests`) crean un `IMapper` real desde la misma configuración.

## 8. Contrato de API

Sin cambios: mismas rutas, mismo JSON.

| Método | Ruta | Response | Errores |
|---|---|---|---|
| GET | /api/Customer | `IReadOnlyList<CustomerDTO>` | 401 |
| GET | /api/Customer/{id} | `CustomerDTO` | 401, 404 |
| GET | /api/provinces · /api/provinces/{id}/cities · /api/cities/{id}/neighborhoods · /api/Gender | listas de DTOs de referencia | 401 |
| POST | /api/Authentication/Authenticate | `AuthTokenResponse` | 400, 401 |

## 9. Modelo de datos

No aplica (sin cambios de esquema ni migración).

## 10. Pantallas (Angular)

No aplica.

## 11. Pasos de implementación (orden)

1. Agregar el paquete `AutoMapper` 14.x a `GePro.Business.csproj` y revisar el aviso de seguridad de NuGet (ver sección 13).
2. Test rojo: `MappingConfigurationTests` (`AssertConfigurationIsValid`) y un test por Profile.
3. Crear los Profiles de la sección 4 y registrarlos en `AddBusiness()`.
4. Migrar `CustomerService` (inyectar `IMapper`, eliminar `ToDto`) y verificar que `GET /api/Customer` y `/{id}` devuelven el mismo JSON.
5. Migrar `ReferenceDataServices` (`Province`, `City`, `Neighborhood`, `Gender`), conservando el orden actual.
6. Migrar `AuthenticationService` (`AppUser` → `AuthenticatedUserDTO`) y actualizar `AuthenticationServiceTests`.
7. Actualizar `Docs/architecture/*`, `CLAUDE.md` y la nota del ADR-0001; correr build y tests completos.

## 12. Criterios de aceptación

Cada criterio se convierte en al menos un test.

- [ ] **CA-01**: Dados todos los Profiles de `GePro.Business`, cuando se llama a `AssertConfigurationIsValid()`, entonces no lanza excepción.
- [ ] **CA-02**: Dado un `Customer` con `Person` y `Address`, cuando se mapea a `CustomerDTO`, entonces cada campo del DTO (incluidos `PersonName`, `PersonAge`, `GenderId`, `AddressId`, `ProvinceId`, `CityId`, `StreetNumber` y las fechas de auditoría) tiene el mismo valor que producía `CustomerService.ToDto`.
- [ ] **CA-02b**: Dado un `Employee` con `Person` y `Address`, cuando se mapea a `EmployeeDTO`, entonces los campos de persona y dirección se completan igual que en `CustomerDTO` y `FileNumber`, `Position`, `Department`, `HireDate`, `TerminationDate`, `TaxId` y `AppUserId` coinciden con la entidad (incluidos los valores nulos).
- [ ] **CA-03**: Dado un `Customer` cuya `Address` no tiene barrio, cuando se mapea a `CustomerDTO`, entonces `NeighborhoodId` y `NeighborhoodName` son `null`.
- [ ] **CA-04**: Dados una `City` con su `Province` y un `Neighborhood` con su `City`, cuando se mapean, entonces `ProvinceName` y `CityName` se completan.
- [ ] **CA-05**: Dado un `AppUser` con `Role`, cuando se mapea a `AuthenticatedUserDTO`, entonces `Id`, `Name`, `RoleName` y `Permissions` coinciden.
- [ ] **CA-06**: Dadas varias `Province`/`Gender`, cuando los servicios de referencia las listan, entonces se devuelven como `IReadOnlyList` de DTOs con el mismo orden que antes.
- [ ] **CA-07**: Dado el código fuente, entonces ya no existe `CustomerService.ToDto` ni construcciones `new XxxDTO { … }` de entidades en `GePro.Business/Services`.
- [ ] **CA-08**: Dados los tests existentes (`AuthTests`, `SwaggerTests`, `AuthenticationServiceTests`, `PasswordServiceTests`), cuando se ejecuta `dotnet test GePro.slnx`, entonces todos pasan sin cambio de contrato de API.
- [ ] **CA-09**: Dado `GePro.API`, entonces no referencia el paquete `AutoMapper` ni los Profiles directamente (solo llama a `AddBusiness()`).

## 13. Decisiones y preguntas abiertas

Decisiones tomadas (2026-11-27); sin preguntas abiertas:

- **D-01** — Versión: AutoMapper **14.x** (sin clave de licencia).
- **D-02** — El registro (`AddAutoMapper`) va en `AddBusiness()`.
- **D-03** — Se crea ya el Profile de `Employee` → `EmployeeDTO` (con test, sin servicio ni endpoint).

- **D-06** — Q-04 resuelta (2026-11-27): `dotnet list package --vulnerable` reporta para AutoMapper 14.0.0 tres avisos de severidad Alta (GHSA-rvv3-g6hj-g44x, GHSA-gpwp-5797-mjff, GHSA-6vhm-j9h7-f9gh). El usuario acepta el riesgo y se usa 14.0.0 con los tres avisos suprimidos de forma explícita (`NuGetAuditSuppress` centralizado en `Directory.Build.props`, con comentario, porque los proyectos que referencian a `GePro.Business` reciben el aviso de forma transitiva). El detalle de cada aviso no se revisó. Revisar esta excepción si AutoMapper empieza a recibir datos no confiables o si se decide actualizar de versión.
