# Capa Model (`GePro.Model`)

Sin dependencias de otros proyectos ni de EF Core.

## Reglas

- **Entidades** (`Entities/`): clases POCO con `Id`, propiedades y relaciones de navegación. Nombres en singular, términos de `CONTEXT.md`.
- Las invariantes simples (campos obligatorios, longitudes) se expresan en la configuración EF o en el servicio, no con atributos de transporte.
- **DTOs** (`Dtos/<Feature>/`): `XxxDto` (lectura), `CreateXxxDto`, `UpdateXxxDto`. Usar `record` o clases con `init`. Nunca contienen entidades dentro.
- **Enums** (`Enums/`): valores explícitos; se persisten como string o int según el ADR vigente.
- **Excepciones** (`Exceptions/`): `NotFoundException`, `ValidationException`, `ConflictException`, `ForbiddenException`, todas heredan de `DomainException`.
- Auditoría: si la spec la pide, `CreatedAt` / `UpdatedAt` en una clase base `AuditableEntity`.
- Nullable reference types activados; `string` no nulo por defecto.

## Ejemplo

```csharp
namespace GePro.Model.Entities;

public class Proyecto : AuditableEntity
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public EstadoProyecto Estado { get; set; }
    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}

namespace GePro.Model.Dtos.Proyecto;

public record ProyectoDto(int Id, string Nombre, string Estado);
public record CreateProyectoDto(string Nombre);
```

## Errores típicos

- DTO que reutiliza la entidad "para ir más rápido".
- Lógica (métodos de negocio) dentro de la entidad que necesita servicios o repositorios.
- Nombres en inglés/español mezclados o términos que no están en `CONTEXT.md`.
