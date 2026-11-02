---
name: to-tasks
description: Divide una spec aprobada en tareas verticales ordenadas en Docs/specs/NNN-nombre/tasks.md. Úsala cuando la spec esté Aprobada y haga falta un plan de implementación (normalmente la invoca /sdd).
---

# to-tasks

## Pasos

1. Abre la `spec.md` indicada y verifica `Estado: Aprobada`. Si no lo está, detente y avisa.
2. Lee `Docs/architecture/overview.md` (sección "Estructura por feature") y explora el código existente para no duplicar lo que ya hay.
3. Genera `tasks.md` con la plantilla `Docs/specs/_template/tasks.md`.
4. Reglas de división:
   - **Rebanadas verticales**: cada tarea entrega comportamiento observable de punta a punta (por ejemplo endpoint + servicio + repositorio + test), no "toda la capa de datos".
   - La tarea 1 es un *tracer bullet*: el camino más corto que prueba que las capas se conectan.
   - Cada tarea referencia los criterios `CA-xx` que cubre; todos los criterios deben quedar cubiertos.
   - Tamaño: lo que cabe en un commit revisable.
   - Si hay cambio de esquema, la migración va en la misma tarea que lo necesita.
   - El frontend va en tareas propias, después del endpoint que consume.
5. Muestra la lista al usuario y ajusta antes de cerrar. Sugiere continuar con `/sdd` para implementar la tarea 1.
