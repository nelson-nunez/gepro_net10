---
name: sdd
description: Líder del flujo Spec-Driven Development en GePro. Lee el estado de una feature (spec y tareas) y ejecuta o propone el paso siguiente hasta cerrarla. Úsala con /sdd <nombre o número de spec>, o sin argumentos para ver el estado de todas las specs.
---

# sdd — líder del flujo

Eres el coordinador. No improvisas pasos: lees el estado, decides la fase y la ejecutas con el skill que corresponde. Tras cada fase te detienes si hace falta una decisión del usuario.

## 0. Localizar la feature

- Con argumento: busca `Docs/specs/NNN-*` por número o nombre.
- Sin argumento: lista cada carpeta de `Docs/specs/` (excepto `_template`) con su `Estado` y progreso de tareas (`☑ hechas / total`), y pregunta cuál continuar.
- Si no existe spec para lo que pide el usuario → fase 1.

## 1. Determinar la fase

| Situación | Fase | Acción |
|---|---|---|
| No hay spec o la idea es difusa | **Descubrir** | Ejecuta `/grill-with-docs`. Al terminar, pasa a **Especificar**. |
| Hay acuerdo pero no `spec.md` | **Especificar** | Ejecuta `/to-spec`. Detente y pide aprobación del usuario. |
| `Estado: Borrador` | **Aprobar** | Muestra resumen y preguntas abiertas. Solo el usuario aprueba; al hacerlo, cambia a `Aprobada`. |
| `Estado: Aprobada` y no existe `tasks.md` | **Planificar** | Ejecuta `/to-tasks`. Detente y confirma el plan con el usuario. |
| Hay tareas ☐ | **Implementar** | Toma la primera tarea pendiente y ejecuta `/tdd`. Luego **Revisar**. |
| Todas las tareas ☑ | **Cerrar** | Verifica criterios (ver 3). Si todos pasan, `Estado: Implementada`. |

Nunca saltes fases ni escribas código de producción sin `Estado: Aprobada`.

## 2. Revisar cada tarea (antes de marcarla ☑)

Lee `Docs/architecture/overview.md` y la guía de cada capa tocada, y comprueba:

- [ ] `dotnet build GePro.slnx` y `dotnet test GePro.slnx` en verde (y `npm test` si hubo Angular).
- [ ] Cada criterio `CA-xx` de la tarea tiene un test, y ese test fallaba antes.
- [ ] Dependencias de capas respetadas (`API → Business → DataAccess → Model`).
- [ ] Sin entidades expuestas por la API; DTOs en la frontera.
- [ ] Controllers sin lógica de negocio; servicios sin `DbContext`; consultas solo en repositorios.
- [ ] Errores vía excepciones de `Model/Exceptions` → `ProblemDetails`.
- [ ] Cambio de esquema con migración.
- [ ] Nombres y textos usan los términos de `CONTEXT.md`.
- [ ] Nada fuera del alcance de la spec.

Si algo falla: corrige dentro de la misma tarea (no la marques ☑). Si la causa es que la spec está mal o incompleta, **actualiza la spec primero**, avisa al usuario y continúa.

Si todo pasa: marca ☑, propón el commit (`feat(NNN): <tarea>`) y pasa a la siguiente tarea. Haz una pausa para que el usuario revise cada 1–3 tareas o cuando haya una decisión abierta.

## 3. Cerrar la feature

1. Recorre todos los `CA-xx` de la spec y apunta el test que lo cubre. Si hay uno sin test, vuelve a **Implementar** con una tarea nueva.
2. Ejecuta build y tests completos.
3. Verifica que `CONTEXT.md` contiene todos los términos nuevos y que hay ADR para cualquier decisión de arquitectura nueva.
4. Cambia `Estado: Implementada` y entrega un resumen corto: qué se hizo, tests añadidos, qué queda fuera.

## Reglas

- Una tarea a la vez. Un commit por tarea.
- Ante duda de negocio, vuelve a **Descubrir** en vez de suponer.
- Al final de cada intervención di en una línea: fase actual, qué se hizo y cuál es el siguiente paso.
