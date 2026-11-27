# Capa Model (`GePro.Model`)

Sin referencias a otros proyectos. Usa Data Annotations (`System.ComponentModel.DataAnnotations`) solo para el tipo y largo de columnas.

## Estructura real

| Carpeta | Namespace | Contenido |
|---|---|---|
| `Base/` | `GePro.Model.Base` | `BaseEntity` (`Id`, auditoría `Created/Updated/Deleted` y `CreatedBy/UpdatedBy/DeletedBy`), `IEntity`, `ISoftDelete`, `PagingSortFilterRequest`, `PagedDataResponse<T>`. |
| `Model/` | `GePro.Model.Model` | Entidades: `Customer`, `Employee`, `Gender`. Subcarpetas `Address/` (`GePro.Model.Model.Address`: `Address`, `City`, `Neighborhood`, `Province`) y `Person/` (`GePro.Model.Model.Person`: `Person`). |
| `ModelUser/` | `GePro.Model.ModelUser` | `AppUser`, `Role`. |
| `BaseDTO/` | `GePro.Model.BaseDTO` | `BaseEntityDTO<TId>` (`Id` + fechas de auditoría), `ActionResultDTO`, `ExceptionDTO`. |
| `DTO/` | `GePro.Model.DTO` | `CustomerDTO`, `EmployeeDTO`, `PersonDTO`, `AddressDTO`, `GenderDTO`, `AuthenticatedUserDTO`, etc. Sufijo `DTO` en mayúsculas. |
| `Exceptions/` | `GePro.Model.Exceptions` | `DomainException` (abstracta), `NotFoundException`, `UnauthorizedException`. |

## Reglas

- **Entidades**: clases POCO que heredan de `BaseEntity`. Nombres en singular, términos de `CONTEXT.md`. Las propiedades de texto se inicializan con `string.Empty`.
- **Borrado lógico**: `BaseEntity` implementa `ISoftDelete`. Nada se borra físicamente (ver `data-access.md`).
- **Auditoría**: la rellena `UnitOfWork`; las entidades no asignan `Created*`/`Updated*`/`Deleted*`.
- **DTOs**: clases con `get; set;`. Las de lectura heredan de `BaseEntityDTO<int>`. Nunca contienen entidades dentro. Los DTO de request que solo existen por el contrato HTTP (por ejemplo `SigninRequest`, `AuthTokenResponse`) viven en `GePro.API/Contracts/`.
- **Excepciones de negocio**: heredan de `DomainException`. Hoy existen `NotFoundException` (404) y `UnauthorizedException` (401). Una excepción nueva (`ValidationException`, `ConflictException`, `ForbiddenException`) se crea cuando una spec la requiera, junto con su traducción en `DomainExceptionHandler`.
- Nullable reference types activados.

## Ejemplo

```csharp
namespace GePro.Model.Model
{
    public class Gender : BaseEntity
    {
        public const int Male = 1;

        [Column(TypeName = "VARCHAR"), StringLength(128)]
        public string Name { get; set; } = string.Empty;
    }
}

namespace GePro.Model.DTO
{
    public class GenderDTO : BaseEntityDTO<int>
    {
        public string Name { get; set; } = string.Empty;
    }
}
```

## Errores típicos

- DTO que reutiliza la entidad "para ir más rápido".
- Lógica de negocio (métodos que necesitan servicios o repositorios) dentro de la entidad.
- Nombres en inglés y español mezclados, o términos que no están en `CONTEXT.md`.
