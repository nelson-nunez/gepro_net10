# Guía Harness

Esta guía explica **para qué sirve cada archivo del harness SDD de GePro y dónde poner cada cosa**. Está escrita para ti (la persona que desarrolla), no para el agente.

## 1. Qué es el harness

El harness es el conjunto de documentos, skills y automatizaciones que le dicen a Claude **qué construir, con qué palabras, con qué estructura y en qué orden**. Sin él, el agente improvisa y cada feature sale distinta. Con él:

- Nada se programa sin una **spec aprobada** (SDD: Spec-Driven Development).
- El código de cada capa sigue **las mismas reglas**.
- Hay **un líder (`/sdd`)** que sabe en qué paso va cada feature.
- Cada spec vive en **su propia rama** y los commits los gestiona `/commit-sdd`.

## 2. Mapa de archivos

La raíz de git es la carpeta que contiene `.gitignore` y la carpeta `GePro/`; todo lo siguiente está dentro de esta última (`GePro.slnx` en la raíz de trabajo).

```
GePro/                              ← raíz de trabajo (GePro.slnx)
├── CLAUDE.md                       ← Lo primero que lee el agente
├── CONTEXT.md                      ← Glosario del dominio
├── README.md                       ← Presentación del proyecto
├── .claude/
│   ├── settings.json               ← Permisos y registro del hook
│   ├── hooks/verify.sh             ← Hook Stop: build + tests al terminar
│   └── skills/                     ← Los comandos del flujo
│       └── sdd/ · grill-with-docs/ · to-spec/ · to-tasks/ · tdd/ · commit-sdd/
├── Docs/
│   ├── Readme.md                   ← Índice de la documentación
│   ├── architecture/               ← Cómo debe ser el código
│   │   ├── overview.md
│   │   └── model.md · data-access.md · business.md · api.md · angular.md
│   ├── adr/                        ← Decisiones de arquitectura (el porqué)
│   │   └── 0001-arquitectura-en-capas.md
│   └── specs/                      ← Qué se construye, feature por feature
│       ├── _template/{spec.md, tasks.md}
│       └── 001-swagger-api/{spec.md, tasks.md}
├── Personal/                       ← Notas y bitácora tuyas (no son reglas)
│   ├── Guia-Harness.md             ← Este documento
│   ├── ProjectScaffold.md          ← Bitácora de scaffold
│   └── Angular.md                  ← Notas de frontend
├── GePro.API · GePro.Business · GePro.DataAccess · GePro.Model
├── GePro.API.Tests · GePro.Business.Tests
└── GePro/                          ← Frontend Angular (GePro.UI.esproj)
```

En Visual Studio, `GePro.slnx` agrupa estos archivos en carpetas de solución: `Docs/…`, `.claude/…`, `Elementos de la solución` (`CLAUDE.md`, `CONTEXT.md`, `README.md`), `Personal` y `Test`.

> Claude Code solo carga `CLAUDE.md` automáticamente si está en la raíz de trabajo del proyecto.

## 3. Qué hace cada archivo

### Raíz

| Archivo | Función | Quién lo edita |
|---|---|---|
| `CLAUDE.md` | Punto de entrada del agente: mapa del repo, reglas de dependencias, flujo SDD, comandos, convenciones y a qué guía ir para cada capa. Se carga solo en cada sesión. | Tú, cuando cambien comandos, proyectos o reglas generales. Mantenlo corto. |
| `CONTEXT.md` | Glosario del dominio: actores, entidades, reglas de negocio transversales y términos prohibidos. Es el lenguaje común: clases, DTOs, endpoints y textos de pantalla usan estas palabras. | `/grill-with-docs` lo actualiza; tú lo revisas. |
| `README.md` | Presentación del proyecto para quien llega al repo. | Tú. |

### `Docs/architecture/` — cómo debe ser el código

| Archivo | Función |
|---|---|
| `overview.md` | Las capas, qué va y qué NO va en cada una, estructura de ficheros por feature, flujo de una petición y temas transversales (errores, auth, logging, transacciones). |
| `model.md` | Entidades, DTOs, enums y excepciones de negocio (`GePro.Model`). |
| `data-access.md` | `DbModelContext`, Fluent API, repositorios, Unit of Work, borrado lógico y migraciones (`GePro.DataAccess`). |
| `business.md` | Servicios, mapeo a DTO, reglas de negocio y tests de servicio (`GePro.Business`). |
| `api.md` | Controllers finos, `ProblemDetails`, JWT, Swagger/OpenAPI, DI (`GePro.API`). |
| `angular.md` | Estructura de módulos por feature, servicios HTTP, formularios y estados de pantalla (`GePro/`). |

Cada guía tiene reglas, un ejemplo y errores típicos. El agente las lee **antes** de escribir código en esa capa. Son la fuente de verdad: si el código y la guía discrepan, se corrige uno de los dos.

### `Docs/adr/` — el porqué

Un ADR (Architecture Decision Record) es un documento corto por decisión difícil de revertir: contexto, decisión, consecuencias y alternativas descartadas. `0001` explica por qué hay cuatro capas. Se numeran de forma correlativa (`0002-...`). Si no hay ADR, la decisión no está tomada.

### `Docs/specs/` — qué se construye

| Archivo | Función |
|---|---|
| `_template/spec.md` | Plantilla de la especificación funcional: problema, alcance (dentro/fuera), reglas, contrato de API, modelo de datos, pantallas y **criterios de aceptación (CA-xx)**. |
| `_template/tasks.md` | Plantilla del plan: tareas verticales ordenadas, qué criterios cubre cada una y su estado (☐/☑). |
| `NNN-nombre/spec.md` | La spec de una feature. Campo `Estado`: `Borrador` → `Aprobada` → `Implementada`. |
| `NNN-nombre/tasks.md` | Las tareas de esa feature. Una tarea = un commit. |

Los criterios de aceptación son la definición de "terminado": cada uno acaba en al menos un test.

### `.claude/skills/` — los comandos

| Comando | Función | Cuándo usarlo |
|---|---|---|
| `/sdd` | **El líder.** Lee el estado de la feature y ejecuta o propone el paso siguiente; revisa cada tarea contra las guías de arquitectura y cierra la spec. Sin argumentos lista el estado de todas las specs. | Casi siempre: es tu comando principal. |
| `/grill-with-docs` | Te entrevista, de una pregunta a la vez, hasta que idea y vocabulario quedan claros. Actualiza `CONTEXT.md` y crea ADRs. | Al empezar una feature o cuando el plan es difuso. |
| `/to-spec` | Convierte lo acordado en `spec.md` (estado `Borrador`) y llama a `/commit-sdd` para crear la rama, commitear y pushear la spec. | Tras la entrevista. |
| `/to-tasks` | Divide una spec aprobada en tareas verticales (`tasks.md`). | Con la spec en `Aprobada`. |
| `/tdd` | Implementa **una** tarea: test rojo, código mínimo, refactor; luego llama a `/commit-sdd`. | Lo invoca `/sdd`; también lo puedes lanzar tú. |
| `/commit-sdd` | Gestiona ramas y commits: rama `feature/NNN-nombre` por spec, push automático de la spec nueva, un commit por tarea (sin push) y push del código solo con tests en verde y tu aprobación. | Lo invocan las demás skills; también lo puedes lanzar tú. |

### `.claude/settings.json` y `.claude/hooks/verify.sh` — automatización

- `settings.json` permite sin preguntar `dotnet build`, `dotnet test` y `npm test`, y prohíbe leer `appsettings.*.json`.
- `verify.sh` es un hook `Stop`: cuando el agente termina, compila y corre los tests. Si fallan, le devuelve el error para que lo corrija antes de dar la tarea por terminada.

## 4. Dónde poner cada cosa

| Quiero… | Lo pongo en… |
|---|---|
| Definir una palabra nueva del dominio | `CONTEXT.md` (antes de usarla en código) |
| Escribir lo que hará una feature | `Docs/specs/NNN-nombre/spec.md` (`/to-spec` también la agrega a `GePro.slnx`) |
| Planificar cómo implementarla | `Docs/specs/NNN-nombre/tasks.md` (`/to-tasks` también la agrega a `GePro.slnx`) |
| Registrar una decisión técnica importante | Nuevo `Docs/adr/NNNN-titulo.md` |
| Cambiar cómo se escribe el código de una capa | La guía de esa capa en `Docs/architecture/` (y un ADR si es un cambio de fondo) |
| Cambiar comandos o reglas generales | `CLAUDE.md` |
| Añadir un paso o comando al flujo | Nueva carpeta en `.claude/skills/<nombre>/SKILL.md` (y agrégala a `GePro.slnx`) |
| Cambiar permisos o automatizaciones | `.claude/settings.json` / `.claude/hooks/` |
| Guardar el historial de cómo se montó el proyecto | `Personal/ProjectScaffold.md` (bitácora) |
| Notas específicas del frontend que no son reglas | `Personal/Angular.md` |

Regla práctica: **reglas permanentes** van en `architecture/`; **decisiones y su razón** en `adr/`; **lo que se construye ahora** en `specs/`; **lenguaje** en `CONTEXT.md`.

## 5. Cómo trabajar una feature, paso a paso

1. **Arranca con `/sdd`** (o `/sdd <número>` si ya existe la spec). Te dirá en qué fase estás.
2. **Descubrir**: responde las preguntas de `/grill-with-docs`. Revisa los cambios en `CONTEXT.md`.
3. **Especificar**: `/to-spec` crea `spec.md` en `Borrador`. `/commit-sdd` crea la rama `feature/NNN-nombre`, commitea la spec y la pushea. Léela completa, sobre todo alcance "Fuera" y criterios.
4. **Aprobar**: solo tú. Cambia `Estado: Aprobada` (o dile al agente "aprobada"). Ese cambio también se commitea y pushea en la rama. Sin esto no se escribe código.
5. **Planificar**: `/to-tasks` crea `tasks.md`. Comprueba que cada criterio CA-xx está cubierto.
6. **Implementar**: `/sdd` toma la primera tarea pendiente y usa `/tdd`. Cada tarea termina en un commit `feat(NNN): …` en la rama de la spec, **sin push**. Tras cada tarea revisa el diff.
7. **Cerrar**: cuando todas las tareas están ☑, `/sdd` verifica los criterios, los términos y los ADR, y marca `Implementada`. Luego `/commit-sdd` te muestra commits y diff y **te pide aprobación** antes de pushear la rama.
8. **PR / merge a `main`**: solo si lo pides, con `/ship`.

Si algo cambia a mitad de camino (un requisito, una regla), **se edita la spec primero**; el código sigue a la spec, no al revés.

## 6. Reglas de oro

- Sin spec `Aprobada` no hay código de producción.
- Una spec, una rama `feature/NNN-nombre`; nunca se implementa en `main`.
- Una tarea a la vez, un commit por tarea.
- La spec nueva se pushea sola; el código solo con build y tests en verde y tu aprobación.
- Los nombres en código salen de `CONTEXT.md`.
- Las capas dependen en un solo sentido: `API → Business → DataAccess → Model`.
- Si dudas del negocio, vuelve a `/grill-with-docs`; no supongas.
- Credenciales (tokens, contraseñas) nunca en archivos del repo ni en la URL del remote: van en el Administrador de credenciales de Windows.

## 7. Preguntas frecuentes

**¿Dónde están `ProjectScaffold.md` y `Angular.md`?** En `Personal/`. Son notas tuyas, no reglas del agente.

**¿Puedo cambiar una regla de arquitectura?** Sí: edita la guía de la capa y, si es una decisión de fondo, añade un ADR. No lo hagas dentro de una tarea de feature sin avisar.

**¿Y para un bug o un cambio pequeño?** Si cambia comportamiento, actualiza la spec de la feature afectada y añade una tarea. Si es un arreglo sin cambio de comportamiento, basta un test que lo reproduzca y el arreglo.

**¿Qué pasa si el agente propone código que rompe una guía?** Es un fallo del proceso: pide que lo corrija o revisa si la guía está desactualizada.

**¿Los archivos `*.user` van al repo?** No: son configuración local de Visual Studio y están en `.gitignore`.
