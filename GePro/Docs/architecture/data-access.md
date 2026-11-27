# Capa DataAccess (`GePro.DataAccess`)

Depende solo de `Model`. Aquí vive todo lo relacionado con EF Core (SQL Server, `UseLazyLoadingProxies`).

## Estructura real

```
GePro.DataAccess/
  DbModelContext.cs          # DbContext único, Fluent API en OnModelCreating, Seed (ModelBuilderExtensions)
  DbContextExtension.cs      # filtro global de borrado lógico (interno)
  DependencyInjection.cs     # AddDataAccess(connectionString)
  IUnitOfWork.cs, UnitOfWork.cs
  ICurrentUser.cs            # interfaz; la implementa la API
  Repositories/IRepository.cs, Repository.cs, SpecificRepositories.cs
  Extensions/EntityFrameworkExtension.cs, ExpressionBuilder.cs
  Migrations/                # local; hoy ignorada por Git (ver nota)
```

## Reglas

- Un único `DbModelContext`. Los `DbSet<>` exponen solo entidades de `Model`.
- **Configuración EF con Fluent API dentro de `OnModelCreating`** (claves, índices únicos, cascadas a `Restrict`). No hay clases `IEntityTypeConfiguration` por entidad.
- **Borrado lógico**: toda entidad con `ISoftDelete` recibe un filtro global (`Deleted == null`) y un índice sobre `Deleted`. `IRepository.Remove` marca la entidad; `UnitOfWork.SaveChangesAsync` la convierte en `Modified` y asigna `Deleted`/`DeletedBy`.
- **Auditoría**: `UnitOfWork.ApplyAudit()` rellena `Created/Updated/Deleted` con `TimeProvider` y los `*By` con `ICurrentUser.UserName`.
- **Repositorios**: `IRepository<T>` genérico (`GetByIdAsync`, `FirstOrDefaultAsync`, `AnyAsync`, `ListAsync`, `GetPagedAsync`, `AddAsync`, `Remove`). Las consultas específicas van en `IXxxRepository` dentro de `Repositories/SpecificRepositories.cs`, con nombre de dominio (`ListByProvinceAsync`). Nunca exponen `IQueryable`.
- **Unit of Work**: `IUnitOfWork` expone un repositorio por entidad y `SaveChangesAsync(CancellationToken)`. Es el único sitio que persiste. Un repositorio nuevo se agrega a `IUnitOfWork` y a `UnitOfWork`.
- Lecturas: `AsNoTracking()`. Los repositorios específicos usan `Include` explícito solo donde el servicio lo necesita.
- Todos los métodos son `async` y reciben `CancellationToken`.
- Los repositorios no validan reglas de negocio ni lanzan excepciones de negocio (devuelven `null` o colecciones vacías).
- **Seed**: datos iniciales en `ModelBuilderExtensions.Seed` (`HasData`), con fecha fija (`SeedDate`) para que el modelo no cambie en cada compilación.
- `AddDataAccess` falla al arrancar si la cadena de conexión está vacía. La API debe registrar además `ICurrentUser`.

## Migraciones

Proyecto de inicio = `GePro.API` (tiene el host, la configuración y `Microsoft.EntityFrameworkCore.Design`). Desde la Package Manager Console:

```
Add-Migration NombreDescriptivo -Project GePro.DataAccess -StartupProject GePro.API
Update-Database -Project GePro.DataAccess -StartupProject GePro.API
```

Con la CLI: `dotnet ef migrations add NombreDescriptivo --project GePro.DataAccess --startup-project GePro.API`.

Requiere `ConnectionStrings:DefaultConnection` (user-secrets o `appsettings.Development.json`). Se revisa el SQL generado antes de dar la tarea por terminada.

> Nota: `.gitignore` excluye `GePro/GePro.DataAccess/Migrations/`, así que las migraciones no se versionan. Ver "Dudas" del informe de documentación.

## Ejemplo

```csharp
public interface ICityRepository : IRepository<City>
{
    Task<IReadOnlyList<City>> ListByProvinceAsync(int provinceId, CancellationToken ct);
}

public class CityRepository(DbModelContext context) : Repository<City>(context), ICityRepository
{
    public async Task<IReadOnlyList<City>> ListByProvinceAsync(int provinceId, CancellationToken ct)
    {
        return await DbSet.AsNoTracking()
            .Include(c => c.Province)
            .Where(c => c.ProvinceId == provinceId)
            .OrderBy(c => c.Name)
            .ToListAsync(ct);
    }
}
```

## Errores típicos

- Devolver `IQueryable` para que el servicio componga consultas.
- Llamar a `SaveChangesAsync` del `DbContext` dentro de un repositorio.
- Cambiar una entidad sin crear la migración.
- Borrar físicamente en lugar de usar `Remove` (borrado lógico).
