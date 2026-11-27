# 003 — Validación de requests con FluentValidation

- **Estado**: Aprobada
- **Aprobación**: explícita del usuario (2026-11-27), con las decisiones D-01 a D-05 de la sección 14
- **Fecha**: 2026-11-27
- **Capas afectadas**: API / Tests

## 1. Problema y objetivo

Hoy la única validación de entrada de la API son dos atributos `[Required]` en `SigninRequest`, que aceptan contraseñas o usuarios formados solo por espacios y no tienen límites de largo. Cuando se agreguen endpoints de escritura (clientes, empleados) cada request necesitará reglas propias. El objetivo es dejar listo el mecanismo: validadores FluentValidation por request, ejecutados por un filtro global, con una única forma de respuesta de error (400 `ValidationProblemDetails`), y migrar a él el único request que existe hoy.

## 2. Análisis del estado actual

**Qué entra por los endpoints** (verificado en `GePro.API/Controllers`):

| Endpoint | Entrada | Validación hoy |
|---|---|---|
| `POST /api/Authentication/Authenticate` | Body `SigninRequest` (`UserName`, `Password`) en `GePro.API/Contracts/AuthContracts.cs` | `[Required]` en ambas propiedades. |
| `GET /api/Customer` | Query `name` (string?, opcional) | Ninguna; el repositorio ignora el valor vacío. |
| `GET /api/Customer/{id:int}`, `DELETE /api/Customer/{id:int}` | Ruta `id` | Restricción de ruta `:int`. |
| `GET /api/provinces/{provinceId:int}/cities`, `GET /api/cities/{cityId:int}/neighborhoods` | Ruta | Restricción de ruta `:int`. |
| `GET /api/provinces`, `GET /api/Gender` | Sin entrada | — |

No hay otros requests con cuerpo: los DTOs de `GePro.Model.DTO` (`CustomerDTO`, `EmployeeDTO`, …) solo se usan como respuesta.

**Cómo se devuelven los errores hoy**
- Validación de entrada: los `[Required]` se evalúan en `ModelState`; `[ApiController]` responde solo, con 400 y `ValidationProblemDetails` (`errors` por campo). Está cubierto por `AuthTests.Authenticate_SinCredenciales_Devuelve400`, y `AuthenticationController` declara `ProducesResponseType<ProblemDetails>(400)`.
- Reglas de negocio: `DomainExceptionHandler` convierte `NotFoundException` → 404 y `UnauthorizedException` → 401 en `ProblemDetails`.
- No hay `ModelState` manual, `if` de validación en controllers, ni FluentValidation instalado.

**Validaciones que NO son de entrada y no se tocan**
- `AddDataAccess`: lanza si la cadena de conexión está vacía (configuración de arranque).
- `AddJwtAuthentication`: valida `JwtTokenConfig` al arrancar (`ValidateOnStart`).
- `AuthenticationService`: no valida entrada; compara credenciales y lanza `UnauthorizedException`.
- `[StringLength]`/`[Column]` de las entidades: definen columnas de EF, no validan requests.

## 3. Alcance

**Dentro**
- Paquete `FluentValidation.DependencyInjectionExtensions` en `GePro.API` (ver D-01 para la versión).
- Validador `SigninRequestValidator` (reglas en la sección 5).
- Filtro de acción global `ValidationFilter` que ejecuta los validadores registrados.
- Registro en DI con `AddValidatorsFromAssemblyContaining<SigninRequestValidator>()`.
- Eliminar los atributos `[Required]` de `SigninRequest`.
- Actualizar `ProducesResponseType` del 400 de `Authenticate` a `ValidationProblemDetails`.
- Tests unitarios de validadores (`TestValidate`) y test de integración del formato de error.
- Actualizar documentación (`Docs/architecture/api.md`, `overview.md`, `CLAUDE.md`).

**Fuera**
- **No** se usa `FluentValidation.AspNetCore` (auto-validación integrada con MVC, sin mantenimiento).
- Validadores para `CustomerDTO`, `EmployeeDTO` y demás DTOs de respuesta: no entran como request; se crean con la spec del endpoint de escritura que los use.
- Validación de la query `name` y de ids de ruta (ya acotadas por `:int`; ver D-05).
- Validación de reglas de negocio con acceso a datos (duplicados, existencia): siguen en `Business` y lanzan excepciones de `Model/Exceptions`.
- Nuevas excepciones de dominio (`ValidationException`, `ConflictException`).
- Mensajes localizados / multi-idioma: los mensajes se escriben en español.
- Cambios en `JwtTokenConfig` u opciones de arranque.

## 4. Proyecto de los validadores y convención

- **Proyecto**: `GePro.API`, carpeta `Validators/`, namespace `GePro.API.Validators`. Motivo: hoy los únicos requests (`SigninRequest`) viven en `GePro.API/Contracts`, la validación de formato es una preocupación de transporte y no existen proyectos `Application`/`Contracts` compartidos. Crear uno solo para esto sería añadir una capa sin necesidad (ver D-02).
- **Convención**: un validador por request, `<Request>Validator`, clase pública que hereda de `AbstractValidator<Request>`, reglas declaradas en el constructor. Se registran todos por escaneo del ensamblado.
- `Validators/` contiene solo validadores de requests de `GePro.API/Contracts`. Cuando una spec agregue DTOs de entrada en `GePro.Model.DTO`, su validador se decide en esa spec (y puede obligar a revisar D-02).
- Estilo: reglas en varias líneas, un paso por línea, sin `=>` en miembros, mensajes en español con los términos de `CONTEXT.md`.

## 5. Reglas de validación

| Request | Campo | Regla | Mensaje |
|---|---|---|---|
| `SigninRequest` | `UserName` | No nulo, no vacío ni solo espacios (`NotEmpty`) | `El usuario es obligatorio.` |
| `SigninRequest` | `UserName` | Largo máximo 64 (coincide con `AppUser.Name`, `StringLength(64)`) | `El usuario no puede superar los 64 caracteres.` |
| `SigninRequest` | `Password` | No nulo, no vacío ni solo espacios (`NotEmpty`) | `La contraseña es obligatoria.` |
| `SigninRequest` | `Password` | Largo máximo 128 (D-04) | `La contraseña no puede superar los 128 caracteres.` |

La contraseña no se recorta (`Trim`) ni se registra en logs. El tope evita hashear entradas enormes.

## 6. Registro en DI y ejecución

- **Registro**: en `Program.cs` (o en una extensión `AddRequestValidation()` en `GePro.API/Extensions`, igual que `AddJwtAuthentication()`), `services.AddValidatorsFromAssemblyContaining<SigninRequestValidator>()`. Los validadores se registran como `Scoped` (valor por defecto) sin dependencias propias.
- **Ejecución**: `ValidationFilter` implementa `IAsyncActionFilter` y se agrega globalmente: `AddControllers(options => options.Filters.Add<ValidationFilter>())`.
  1. Recorre `context.ActionArguments`.
  2. Para cada argumento no nulo, resuelve `IValidator<T>` (`T` = tipo en tiempo de ejecución) desde `HttpContext.RequestServices`; si no hay validador registrado, lo ignora.
  3. Ejecuta `ValidateAsync` con el `CancellationToken` de la petición.
  4. Si hay errores, vuelca cada `ValidationFailure` en un `ModelStateDictionary` y responde con `ProblemDetailsFactory.CreateValidationProblemDetails(...)` envuelto en `BadRequestObjectResult`/`ObjectResult` con status 400, **sin** llamar a la acción.
- **Convivencia con `[ApiController]`**: se mantiene el filtro automático de `ModelState` (no se usa `SuppressModelStateInvalidFilter`). Corre antes que `ValidationFilter` y sigue resolviendo los errores de enlace (JSON mal formado, cuerpo ausente, tipo incorrecto). `ValidationFilter` actúa solo cuando el enlace fue correcto.
- Sin llamadas a `AddFluentValidationAutoValidation()` ni paquetes de auto-validación.
- Los validadores no consultan base de datos ni servicios externos: son validaciones de formato, síncronas en la práctica.

## 7. Formato de respuesta de error

Respuesta 400, `Content-Type: application/problem+json`, forma estándar de `ValidationProblemDetails` (la misma que genera hoy `[ApiController]`, para no romper clientes):

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "UserName": ["El usuario es obligatorio."],
    "Password": ["La contraseña es obligatoria."]
  }
}
```

- Las claves de `errors` son los nombres de propiedad del request (`UserName`, `Password`), una lista de mensajes por campo.
- Un campo con varias reglas rotas devuelve todos los mensajes.
- Los demás errores no cambian: 401 y 404 siguen saliendo por `DomainExceptionHandler`; excepciones inesperadas, 500 sin detalle interno.

## 8. Qué validaciones manuales existentes se eliminan o migran

| Hoy | Después |
|---|---|
| `[Required]` en `SigninRequest.UserName` y `Password` | Eliminados; migran a `SigninRequestValidator` (con la mejora de rechazar solo espacios y el tope de 64 en el usuario). |
| `ProducesResponseType<ProblemDetails>(400)` en `AuthenticationController.Authenticate` | Pasa a `ProducesResponseType<ValidationProblemDetails>(400)`. |
| `using System.ComponentModel.DataAnnotations` en `AuthContracts.cs` | Se elimina si queda sin uso. |
| Validaciones de arranque (`connectionString`, `JwtTokenConfig`), `AuthenticationService`, columnas EF | Sin cambios. |

## 9. Contrato de API

Sin cambios de rutas ni de forma de éxito. Cambia solo el 400 de `Authenticate`.

| Método | Ruta | Request | Response | Errores |
|---|---|---|---|---|
| POST | /api/Authentication/Authenticate | `SigninRequest` | `AuthTokenResponse` | 400 (`ValidationProblemDetails`), 401 |

## 10. Modelo de datos y pantallas

No aplica (sin cambios de esquema, sin migración ni Angular).

## 11. Tests

- **Unitarios de validador** (`GePro.API.Tests/Validators/SigninRequestValidatorTests.cs`), con `TestValidate` y `ShouldHaveValidationErrorFor` / `ShouldNotHaveValidationErrorFor`. Nombres `Metodo_Escenario_Resultado`.
- **Integración** (`WebApplicationFactory<Program>`, mismo patrón y configuración ficticia que `AuthTests`): POST con cuerpo inválido → 400 con `errors.UserName` / `errors.Password`; POST válido llega a la acción: el test sustituye `IAuthenticationService` por un doble (`ConfigureTestServices`) para no depender de la base de datos, y comprueba que el doble fue invocado.
- **Registro**: test que verifica que `IValidator<SigninRequest>` se resuelve desde DI.
- Se mantiene `AuthTests.Authenticate_SinCredenciales_Devuelve400`.

## 12. Pasos de implementación (orden)

1. Agregar el paquete `FluentValidation.DependencyInjectionExtensions` a `GePro.API.csproj`; ejecutar `dotnet list package --vulnerable`.
2. Test rojo: `SigninRequestValidatorTests` (CA-01 a CA-04) y el test de integración del formato 400 (CA-06, CA-07).
3. Crear `SigninRequestValidator` en `GePro.API/Validators/`.
4. Crear `ValidationFilter` y registrarlo globalmente junto con `AddValidatorsFromAssemblyContaining`.
5. Quitar `[Required]` de `SigninRequest` y actualizar el `ProducesResponseType` del 400.
6. Verificar que `AuthTests` y `SwaggerTests` siguen en verde y que el documento OpenAPI declara `ValidationProblemDetails` para el 400.
7. Actualizar `Docs/architecture/api.md` (sección de validación y errores), `overview.md` (flujo y errores) y `CLAUDE.md`; correr build y tests completos.

## 13. Criterios de aceptación

Cada criterio se convierte en al menos un test.

- [ ] **CA-01**: Dado un `SigninRequest` con `UserName` nulo, vacío o solo espacios, cuando se valida, entonces falla en `UserName` con `El usuario es obligatorio.`.
- [ ] **CA-02**: Dado un `SigninRequest` con `UserName` de más de 64 caracteres, cuando se valida, entonces falla en `UserName` con el mensaje de largo máximo; con exactamente 64 caracteres es válido.
- [ ] **CA-03**: Dado un `SigninRequest` con `Password` nula, vacía o solo espacios, cuando se valida, entonces falla en `Password` con `La contraseña es obligatoria.`.
- [ ] **CA-03b**: Dado un `SigninRequest` con `Password` de más de 128 caracteres, cuando se valida, entonces falla en `Password` con el mensaje de largo máximo; con exactamente 128 caracteres es válido.
- [ ] **CA-04**: Dado un `SigninRequest` con usuario y contraseña válidos, cuando se valida, entonces no hay errores.
- [ ] **CA-05**: Dado el contenedor de DI de la API, cuando se resuelve `IValidator<SigninRequest>`, entonces devuelve `SigninRequestValidator`.
- [ ] **CA-06**: Dado `POST /api/Authentication/Authenticate` con usuario vacío y contraseña vacía, cuando se envía, entonces responde 400 `application/problem+json` con `status: 400` y `errors` que contiene `UserName` y `Password`.
- [ ] **CA-07**: Dado `POST /api/Authentication/Authenticate` con cuerpo `{}`, cuando se envía, entonces responde 400 (comportamiento existente) y la acción no se ejecuta.
- [ ] **CA-08**: Dado `POST /api/Authentication/Authenticate` con un request válido, cuando se envía, entonces el filtro no devuelve 400 y la petición llega a la acción (con `IAuthenticationService` sustituido por un doble en el test).
- [ ] **CA-09**: Dado `POST /api/Authentication/Authenticate` con JSON mal formado, cuando se envía, entonces responde 400 por el filtro automático de `[ApiController]`, sin error 500.
- [ ] **CA-10**: Dado el código fuente, entonces `SigninRequest` ya no tiene atributos `[Required]` y el proyecto no referencia `FluentValidation.AspNetCore`.
- [ ] **CA-11**: Dado el documento `/openapi/v1.json`, entonces la respuesta 400 de `Authenticate` usa el esquema `ValidationProblemDetails`.
- [ ] **CA-12**: Dados los tests existentes, cuando se ejecuta `dotnet test GePro.slnx`, entonces todos pasan.

## 14. Decisiones y preguntas abiertas

Decisiones tomadas (2026-11-27):

- **D-01** — Paquete `FluentValidation.DependencyInjectionExtensions`, última versión estable (confirmar con `dotnet list package --outdated` al instalar). Sin `FluentValidation.AspNetCore`.
- **D-02** — Los validadores viven en `GePro.API/Validators/`; no se crea un proyecto compartido (Q-01 resuelta: no).
- **D-03** — Ejecución mediante un `IAsyncActionFilter` global (no `IEndpointFilter`, porque la API usa controllers).
- **D-04** — `Password` con tope de 128 caracteres (Q-02 resuelta: sí; el valor 128 es el sugerido en el borrador y queda como valor de la spec).
- **D-05** — La query `name` de `GET /api/Customer` queda fuera de la validación (Q-03 resuelta: fuera).

Pendiente:

- **Q-04** — Al instalar el paquete, ejecutar `dotnet list package --vulnerable`; si NuGet audit reporta un aviso, decidir antes de continuar.
