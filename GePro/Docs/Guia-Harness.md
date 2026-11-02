# Guía Harness

Esta guía explica **para qué sirve cada archivo del harness SDD de GePro y dónde poner cada cosa**. Está escrita para ti (la persona que desarrolla), no para el agente.

## 1. Qué es el harness

El harness es el conjunto de documentos y skills que le dicen a Claude **qué construir, con qué palabras, con qué estructura y en qué orden**. Sin él, el agente improvisa y cada feature sale distinta. Con él:

- Nada se programa sin una **spec aprobada** (SDD: Spec-Driven Development).
- El código de cada capa sigue **las mismas reglas**.
- Hay **un líder (`/sdd`)** que sabe en qué paso va cada feature.

## 2. Mapa de archivos

```
GePro/
├── CLAUDE.md                       ← Lo primero que lee el agente
├── CONTEXT.md                      ← Glosario del dominio
├── Docs/
│   ├── Guia-Harness.md             ← Este documento
│   ├── architecture/               ← Cómo debe ser el código
│   │   ├── overview.md
│   │   ├── model.md · data-access.md · business.md · api.md · angular.md
│   ├── adr/                        ← Decisiones de arquitectura (el porqué)
│   │   └── 0001-arquitectura-en-capas.md
│   └── specs/                      ← Qué se construye, feature por feature
│       ├── _template/{spec.md, tasks.md}
│       └── NNN-nombre/{spec.md, tasks.md}
└── .claude/skills/                 ← Los comandos del flujo
    ├── sdd/ · grill-with-docs/ · to-spec/ · to-tasks/ · tdd/
```

## 3. Qué hace cada archivo

### Raíz

| Archivo | Función | Quién lo edita |
|---|---|---|
| `CLAUDE.md` | Punto de entrada del agente: mapa del repo, reglas de dependencias, flujo SDD, comandos, convenciones y a qué guía ir para cada capa. Se carga solo en cada sesión. | Tú, cuando cambien comandos, proyectos o reglas generales. Mantenlo corto. |
| `CONTEXT.md` | Glosario del dominio: actores, entidades, reglas de negocio transversales y términos prohibidos. Es el lenguaje común: clases, DTOs, endpoints y textos de pantalla usan estas palabras. | `/grill-with-docs` lo actualiza; tú lo revisas. |

### `Docs/architecture/` — cómo debe ser el código

| Archivo | Función |
|---|---|
| `overview.md` | Las capas, qué va y qué NO va en cada una, estructura de ficheros por feature, flujo de una petición y temas transversales (errores, auth, logging, transacciones). |
| `model.md` | Entidades, DTOs, enums y excepciones de negocio (`GePro.Model`). |
| `data-access.md` | DbContext, Fluent API, repositorios, Unit of Work y migraciones (`GePro.DataAccess`). |
| `business.md` | Servicios, validaciones, reglas de negocio y tests de servicio (`GePro.Business`). |
| `api.md` | Controllers finos, `ProblemDetails`, JWT, AutoMapper, DI (`GePro.API`). |
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
| `/to-spec` | Convierte lo acordado en `spec.md` (estado `Borrador`). | Tras la entrevista. |
| `/to-tasks` | Divide una spec aprobada en tareas verticales (`tasks.md`). | Con la spec en `Aprobada`. |
| `/tdd` | Implementa **una** tarea: test rojo, código mínimo, refactor. | Lo invoca `/sdd`; también lo puedes lanzar tú. |

## 4. Dónde poner cada cosa

| Quiero… | Lo pongo en… |
|---|---|
| Definir una palabra nueva del dominio | `CONTEXT.md` (antes de usarla en código) |
| Escribir lo que hará una feature | `Docs/specs/NNN-nombre/spec.md` |
| Planificar cómo implementarla | `Docs/specs/NNN-nombre/tasks.md` |
| Registrar una decisión técnica importante | Nuevo `Docs/adr/NNNN-titulo.md` |
| Cambiar cómo se escribe el código de una capa | La guía de esa capa en `Docs/architecture/` (y un ADR si es un cambio de fondo) |
| Cambiar comandos o reglas generales | `CLAUDE.md` |
| Añadir un paso o comando al flujo | Nueva carpeta en `.claude/skills/<nombre>/SKILL.md` |
| Guardar el historial de cómo se montó el proyecto | `Docs/ProjectScaffold.md` (bitácora) |
| Notas específicas del frontend que no son reglas | `Docs/Angular.md` |

Regla práctica: **reglas permanentes** van en `architecture/`; **decisiones y su razón** en `adr/`; **lo que se construye ahora** en `specs/`; **lenguaje** en `CONTEXT.md`.

## 5. Cómo trabajar una feature, paso a paso

1. **Arranca con `/sdd`** (o `/sdd <número>` si ya existe la spec). Te dirá en qué fase estás.
2. **Descubrir**: responde las preguntas de `/grill-with-docs`. Revisa los cambios en `CONTEXT.md`.
3. **Especificar**: `/to-spec` crea `spec.md` en `Borrador`. Léela completa, sobre todo alcance "Fuera" y criterios.
4. **Aprobar**: solo tú. Cambia `Estado: Aprobada` (o dile al agente "aprobada"). Sin esto no se escribe código.
5. **Planificar**: `/to-tasks` crea `tasks.md`. Comprueba que cada criterio CA-xx está cubierto.
6. **Implementar**: `/sdd` toma la primera tarea pendiente y usa `/tdd`. Tras cada tarea revisa el diff y el commit propuesto.
7. **Cerrar**: cuando todas las tareas están ☑, `/sdd` verifica los criterios, los términos y los ADR, y marca `Implementada`.

Si algo cambia a mitad de camino (un requisito, una regla), **se edita la spec primero**; el código sigue a la spec, no al revés.

## 6. Reglas de oro

- Sin spec `Aprobada` no hay código de producción.
- Una tarea a la vez, un commit por tarea.
- Los nombres en código salen de `CONTEXT.md`.
- Las capas dependen en un solo sentido: `API → Business → DataAccess → Model`.
- Si dudas del negocio, vuelve a `/grill-with-docs`; no supongas.

## 7. Preguntas frecuentes

**¿Qué hago con los documentos que ya tenía en `Docs/`?** Se conservan. `ProjectScaffold.md` sigue siendo la bitácora. `ProjectStructure.md` puede limitarse a apuntar a `architecture/overview.md`. `Readme.md` debería enlazar esta guía.

**¿Puedo cambiar una regla de arquitectura?** Sí: edita la guía de la capa y, si es una decisión de fondo, añade un ADR. No lo hagas dentro de una tarea de feature sin avisar.

**¿Y para un bug o un cambio pequeño?** Si cambia comportamiento, actualiza la spec de la feature afectada y añade una tarea. Si es un arreglo sin cambio de comportamiento, basta un test que lo reproduzca y el arreglo.

**¿Qué pasa si el agente propone código que rompe una guía?** Es un fallo del proceso: pide que lo corrija o revisa si la guía está desactualizada.
