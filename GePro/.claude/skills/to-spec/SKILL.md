---
name: to-spec
description: Convierte lo acordado en la conversación en una spec funcional en Docs/specs/NNN-nombre/spec.md a partir de la plantilla. Úsala tras grill-with-docs, cuando el usuario pida una spec o definición funcional.
---

# to-spec

## Pasos

1. Lee `CONTEXT.md` y `Docs/specs/_template/spec.md`.
2. Calcula el siguiente número `NNN` mirando las carpetas de `Docs/specs/` y crea `Docs/specs/NNN-nombre-en-kebab/spec.md`.
3. Rellena todas las secciones de la plantilla con lo conversado. No hagas entrevista: sintetiza. Si falta información crítica, anótala en "Preguntas abiertas" o sugiere volver a `/grill-with-docs`.
4. Reglas de calidad:
   - Usa solo términos de `CONTEXT.md`.
   - Cada criterio de aceptación es verificable y en formato Dado / Cuando / Entonces.
   - Alcance con lista explícita de "Fuera".
   - Estado inicial: `Borrador`.
5. **Agrega la spec a la solución**: en `GePro.slnx` añade (si no existe) una carpeta de solución con la ruta del archivo, justo antes de la carpeta `/Docs/specs/_template/`, siguiendo el formato de las existentes:

   ```xml
   <Folder Name="/Docs/specs/NNN-nombre/">
     <File Path="Docs/specs/NNN-nombre/spec.md" />
   </Folder>
   ```

   Si el `.slnx` quedó modificado, entra en el mismo commit de la spec.
6. Ejecuta `/commit-sdd` (caso A): crea la rama `feature/NNN-nombre`, commitea la spec y la pushea.
7. Muestra al usuario un resumen corto y pide aprobación. Al aprobar, cambia `Estado` a `Aprobada` y vuelve a ejecutar `/commit-sdd` (caso A) para commitear y pushear el cambio.

## Reglas

- No continúes a `/to-tasks` ni a código mientras el estado no sea `Aprobada`.
