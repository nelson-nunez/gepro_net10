# Frontend Angular 19 (`GePro/`)

Proyecto con **NgModules** (no standalone) hasta que un ADR diga lo contrario. Angular 19, tests con Karma + Jasmine (`npm test`), `npm start` sirve en `127.0.0.1`.

**Estado actual**: `GePro/GePro/src/app/` solo contiene el esqueleto generado (`app.module.ts`, `app-routing.module.ts`, `app.component.*`). Las carpetas `core/`, `shared/` y `features/` de abajo son la **estructura objetivo**: se crean con la primera feature de frontend.

## Estructura objetivo

```
src/app/
  core/            # singleton: AuthService, interceptors (JWT, errores), guards, layout
  shared/          # componentes/pipes/directivas reutilizables + SharedModule
  features/
    xxx/           # una carpeta por feature (término de CONTEXT.md)
      xxx.module.ts
      xxx-routing.module.ts
      components/  # presentacionales ("tontos"): @Input/@Output
      pages/       # contenedores enrutados
      services/    # XxxApiService (HTTP) 
      models/      # interfaces TypeScript espejo de los DTOs
  app.module.ts, app-routing.module.ts
```

## Reglas

- **Lazy loading** por feature en `app-routing.module.ts` (`loadChildren`).
- **HTTP solo en servicios** (`XxxApiService`), con `HttpClient` tipado. Los componentes no llaman a `HttpClient`.
- **Sin lógica de negocio en componentes**: formatean, validan formato y delegan.
- **Interceptors** en `core`: añadir el JWT y traducir `ProblemDetails` a un mensaje de usuario.
- **Guards** para rutas autenticadas; el rol/permiso usa los términos de `CONTEXT.md`.
- **Formularios reactivos** (`FormGroup`) con validadores que reflejan las validaciones `VAL-xx` de la spec.
- Cada pantalla gestiona tres estados explícitos: cargando, vacío, error.
- Los modelos TypeScript reflejan los DTOs de la API; si cambia el contrato, cambian juntos.
- Textos de UI con los términos de `CONTEXT.md`.
- `strict: true` en TypeScript; sin `any` salvo justificación en comentario.

## Tests

- `npm test`: un test por criterio de aceptación que afecte a la UI, sobre servicios y componentes de página; el HTTP se simula con `HttpTestingController`.

## Errores típicos

- Llamar a `HttpClient` desde un componente.
- Un módulo gigante en lugar de uno por feature.
- Suscripciones sin cerrar (usar `async` pipe o `takeUntilDestroyed`).
- Duplicar reglas de negocio del backend en el cliente.
