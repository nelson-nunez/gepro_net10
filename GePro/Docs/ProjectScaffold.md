# Iniciando el Proyecto — SiGeP.API

Bitácora de los pasos seguidos para levantar el proyecto desde cero: Web API, Entity Framework Core, patrón Repository/Unit of Work, AutoMapper, autenticación JWT y logging con Serilog.

## 1. Configuración inicial del proyecto

1. Crear proyecto Web API con .NET 5 (definir un nombre corto para evitar problemas de URL — ver soluciones para URLs largas).
2. Configurar compatibilidad con Docker al crear el proyecto. **Modelo:**
3. Crear biblioteca de clases.
4. Referenciar la biblioteca de clases en el proyecto principal (de inicio).
5. Crear el modelo y los DTOs planos.

## 2. Base de datos y Entity Framework Core

6. Instalar SQL Server y SSMS.
7. Configurar el servidor SQL Server y copiar el nombre.
   Referencia: [Cómo instalar y configurar SQL Server paso a paso](https://www.exceleinfo.com/como-instalar-y-configurar-sql-server-paso-a-paso/)
8. Crear el string de conexión en el proyecto principal (de inicio), en `appsettings.json` (es más seguro y ordenado que hacerlo directo en el `DbContext`).
9. Crear `DbModelContext` en la biblioteca de clases.
10. Definir las propiedades de string y las inicializadoras en `OnConfiguring`.
    - Definir también el tipo de carga: perezosa (*lazy*) o dinámica.
    - Validar que se esté tomando el string de conexión, lanzando una excepción si falta.
11. Definir en `OnModelCreating` las clases y el tipo de borrado.
12. Crear un seed para algunos objetos iniciales requeridos.

### Migraciones

13. Crear migraciones (referenciar el proyecto en los comandos si hay varios):

    ```
    Add-Migration NewMigration -Project SiGeP.Model
    Add-Migration NewMigration -Project SiGeP.Model -Context DbModelContext

    Update-Database -Project SiGeP.Model
    Update-Database -Project SiGeP.Model -Context DbModelContext
    ```

    Referencia: [EF Core — Migrations with multiple projects](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/projects?tabs=vs)

## 3. Capa Data Access (Design Pattern)

14. Crear la capa Unit of Work (patrón de diseño que abstrae el acceso a datos de la lógica de negocio).
15. Inyectar el `DbContext` y el `ContextAccessor` en el `UnitOfWork`.
16. Crear un repositorio genérico con las acciones principales.
17. Crear un repositorio para cada clase, que invoque al repositorio genérico. Ahí se pueden agregar acciones no estándar.

## 4. Capa Business

18. Crear un Business por cada clase a trabajar, con las consultas básicas, invocando las acciones del `UnitOfWork`.
    Los datos se trabajan en base al modelo original; los DTO se implementan recién del lado del Controller.

## 5. AutoMapper

19. Instalar el paquete `AutoMapper.Extensions.Microsoft.DependencyInjection`.
20. Añadir `AutoMapperProfile` y definir en el mismo las clases que se van a mapear.
21. Agregar en el `Startup` de la API las referencias a los servicios agregados:

    ```csharp
    services.AddScoped<CustomerRepository, CustomerRepository>();
    services.AddScoped<UnitOfWork, UnitOfWork>();
    services.AddScoped<CustomerBusiness, CustomerBusiness>();
    ```

## 6. Autenticación (JWT)

23. Instalar `Microsoft.AspNetCore.Authentication.JwtBearer`.
24. Crear el Controller y el Business de autenticación.
25. Crear el repositorio y un seed de un usuario básico.
26. Configurar en el `Startup` la autenticación y el token, y luego el Swagger para que use Bearer Token.
27. Configurar en `Services` para que lea los tokens desde Swagger, y en `Startup -> Configure` agregar:

    ```csharp
    app.UseAuthentication();
    app.UseAuthorization();
    ```

## 7. Logging con Serilog

Referencia: [Logging with Serilog in ASP.NET Core Web API](https://www.codeproject.com/Articles/5344667/Logging-with-Serilog-in-ASP-NET-Core-Web-API)

28. Instalar los paquetes `Serilog.Sinks.MSSqlServer` y `Serilog.AspNetCore` (este último tiene mayor compatibilidad con las versiones más nuevas de .NET).
29. En `Program.cs`, agregar en `Main` la configuración del `appsettings` e inicializar el Logger mediante `CreateLogger`.
30. Crear la clase `SerilogConfiguration`, donde se configuran las opciones de las columnas y la tabla de SQL donde se grabará el log.
31. En `appsettings` grabar las configuraciones comunes, como el link de conexión y funciones que también se pueden definir en `Program.cs`.
32. Crear la clase `ExceptionHandlingMiddleware`, donde se atrapan las excepciones, se las formatea y se las enriquece a través del contexto antes de guardarlas en la base de log.
33. Agregar en la clase `Startup.cs`, en el método `Configure`, el uso del middleware para la captura de excepciones.
