---
name: tdd
description: Implementa una tarea de tasks.md con ciclo red-green-refactor, un test por criterio de aceptación, respetando Docs/architecture/. Úsala para escribir código de una feature con spec aprobada (normalmente la invoca /sdd).
---

# tdd

## Antes de empezar

1. Confirma que la spec está `Aprobada` y localiza la tarea en `tasks.md`. Trabaja **una sola tarea**.
2. Lee `CONTEXT.md`, `Docs/architecture/overview.md` y la guía de **cada capa que vas a tocar** (`model.md`, `data-access.md`, `business.md`, `api.md`, `angular.md`).

## Ciclo

1. **Red**: escribe un test que exprese un criterio de aceptación (`Metodo_Escenario_Resultado`). Ejecútalo (`dotnet test` o `npm test`) y comprueba que falla por la razón correcta.
2. **Green**: escribe el código mínimo que lo hace pasar, siguiendo las guías de arquitectura y las dependencias `API → Business → DataAccess → Model`.
3. **Refactor**: limpia con los tests en verde. No cambies comportamiento.
4. Repite por cada criterio de la tarea.

## Reglas

- Testea comportamiento por la interfaz pública, no detalles internos.
- Mocks solo en los límites (repositorios, servicios externos).
- No añadas nada que la spec no pida. Si surge algo, anótalo en la spec o en "Preguntas abiertas".
- Si el código diverge de la spec o de una guía de arquitectura, actualiza el documento primero y avisa.

## Al terminar la tarea

1. Ejecuta `dotnet build GePro.slnx` y `dotnet test GePro.slnx` (y `npm test` si tocaste Angular).
2. Pasa la checklist de revisión de `/sdd` (sección 2). No marques ☑ si algo falla.
3. Marca la tarea como hecha en `tasks.md` (☑).
4. Propón el commit: uno por tarea, con mensaje que cite la spec (`feat(NNN): …`).
