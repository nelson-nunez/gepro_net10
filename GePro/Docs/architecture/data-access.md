# Capa DataAccess (`GePro.DataAccess`)

Depende solo de `Model`. Aquí vive todo lo relacionado con EF Core.

## Reglas

- Un único `GeProDbContext`. Los `DbSet<>` solo exponen entidades de `Model`.
- **Configuración con Fluent API** en una clase `IEntityTypeConfiguration<T>` por entidad (`Configurations/`), cargadas con `ApplyConfigurationsFromAssembly`. Sin Data Annotations de EF en las entidades.
- **Repositorios**: una interfaz `IXxxRepository` y su implementación por agregado. Heredan de `IRepository<T>` genérico (`GetByIdAsync`, `ListAsync`, `AddAsync`, `Remove`). Las consultas específicas van como métodos con nombre del dominio (`ListarPorEstadoAsync`), nunca exponen `IQueryable`.
- **Unit of Work**: `IUnitOfWork` expone los repositorios y `SaveChangesAsync(CancellationToken)`. Es el único sitio que persiste.
- Lecturas sin cambios: `AsNoTracking()`.
- Todos los métodos son `async` y reciben `CancellationToken`.
- **Migraciones**: todo cambio de esquema = migración EF Core con nombre descriptivo (`AddProyectoEstado`). Se revisa el SQL generado antes de dar la tarea por terminada.
- Los repositorios no hacen validaciones de negocio ni lanzan excepciones de negocio (devuelven `null` / colecciones vacías).

## Ejemplo

```csharp
public interface IProyectoRepository : IRepository<Proyecto>
{
    Task<IReadOnlyList<Proyecto>> ListarPorEstadoAsync(EstadoProyecto estado, CancellationToken ct);
}

public class ProyectoRepository(GeProDbContext db) : Repository<Proyecto>(db), IProyectoRepository
{
    public async Task<IReadOnlyList<Proyecto>> ListarPorEstadoAsync(EstadoProyecto estado, CancellationToken ct) =>
        await db.Proyectos.AsNoTracking().Where(p => p.Estado == estado).ToListAsync(ct);
}

public class ProyectoConfiguration : IEntityTypeConfiguration<Proyecto>
{
    public void Configure(EntityTypeBuilder<Proyecto> b)
    {
        b.HasKey(p => p.Id);
        b.Property(p => p.Nombre).IsRequired().HasMaxLength(200);
        b.HasIndex(p => p.Nombre).IsUnique();
    }
}
```

## Errores típicos

- Devolver `IQueryable` para que el servicio componga consultas.
- Llamar a `SaveChangesAsync` dentro del repositorio.
- Cambiar una entidad sin crear la migración.
- Cargar relaciones con `Include` por defecto "por si acaso".
