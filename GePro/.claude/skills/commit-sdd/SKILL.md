---
name: commit-sdd
description: Crea commits limpios en GePro dentro del flujo SDD. Gestiona la rama de cada spec (feature/NNN-nombre), hace commit y push al agregar una spec nueva, un commit por tarea al implementar, y pushea solo tras build/tests en verde y aprobación del desarrollador. Úsala al commitear, o cuando /sdd, /to-spec o /tdd lo indiquen.
---

# commit-sdd

Commits y ramas del flujo SDD. Una spec = una rama. Una tarea = un commit.

## Convenciones

- **Rama**: `feature/NNN-nombre`, igual que la carpeta `Docs/specs/NNN-nombre/`. Ej.: `feature/001-swagger-api`.
- **Mensaje**: `tipo(NNN): descripción en minúsculas, imperativo, ≤ 72 caracteres`, en español.
  - Tipos: `docs` (spec, ADR, CONTEXT), `feat`, `fix`, `test`, `refactor`, `chore`.
  - Cambios sin spec (harness, solución, README): `chore: …` o `docs: …` sin `(NNN)`.
- Nunca commitear: `.env`, credenciales, `*.user`, `bin/`, `obj/`, `node_modules/`, `.vs/`, ni `appsettings*.json` que contengan contraseñas, cadenas de conexión con usuario/clave o `JwtTokenConfig:Secret` (van en user-secrets). Si aparece algo así en `git status`, avisa y exclúyelo. No leas `appsettings.*.json`: `settings.json` lo deniega; revisa con `git diff --stat` y pregunta.
- La raíz de Git es la carpeta superior a `GePro/`: las rutas de `git status` salen con prefijo `GePro/`.
- No uses `--no-verify`, `--force` ni `push --force`.

## Caso A — Spec nueva (se agrega `Docs/specs/NNN-nombre/`)

Se ejecuta justo después de que `/to-spec` crea el `spec.md` (y `CONTEXT.md` / ADRs que hayan cambiado en el descubrimiento).

1. `git status` y `git branch --show-current`. Si hay cambios ajenos a la spec, sepáralos: solo entran en este commit los de la spec (`Docs/specs/NNN-*`, `CONTEXT.md`, `Docs/adr/`).
2. Crea la rama desde la rama base (`main`) actualizada: `git switch main && git pull --ff-only && git switch -c feature/NNN-nombre`. Si la rama ya existe, cámbiate a ella.
3. Commit: `docs(NNN): agregar spec <nombre>`.
4. **Push**: `git push -u origin feature/NNN-nombre`. Es autorización permanente para este caso: el push de una spec nueva no requiere confirmar de nuevo.
5. Informa rama, hash y que quedó pusheada.

Si la spec pasa de `Borrador` a `Aprobada` o cambia después, repite 3–4 en la misma rama: `docs(NNN): aprobar spec` / `docs(NNN): actualizar spec <motivo>`, y push.

## Caso B — Implementación (por tarea)

Lo invoca `/tdd` al terminar una tarea con checklist de `/sdd` en verde.

1. Verifica que estás en `feature/NNN-nombre`. Si no, cámbiate (o créala desde `main` si no existe). Nunca implementes en `main`.
2. Confirma `dotnet build GePro.slnx` y `dotnet test GePro.slnx` (y `npm test` si hubo Angular) en verde. Si algo falla, no commitees.
3. Agrega solo los archivos de la tarea (`git add <rutas>`, no `git add .`), incluido `tasks.md` con la tarea en ☑.
4. Commit: `feat(NNN): <tarea>` (o `test`/`fix`/`refactor` si corresponde). **Un commit por tarea.**
5. **No hagas push todavía.** El push de código lo decide el desarrollador (Caso C).

## Caso C — Push de la feature (requiere aprobación)

Se ejecuta cuando `/sdd` llega a **Cerrar** (todas las tareas ☑) o cuando el desarrollador pide pushear antes.

1. Ejecuta build y tests completos; muestra el resultado.
2. Muestra al desarrollador: rama, `git log main..HEAD --oneline` y `git diff main --stat`.
3. **Pide aprobación explícita** ("¿Pusheo `feature/NNN-nombre`?"). Sin un sí claro, no pushees.
4. Con aprobación: `git push -u origin feature/NNN-nombre` (o `git push` si ya tiene upstream). Informa el resultado.
5. Pull request o merge a `main`: solo si el desarrollador lo pide; se hace con `/ship`, no desde aquí.

## Reglas

- Antes de cualquier commit, mira el `git status`/`git diff` de lo que entra; no commitees a ciegas.
- Commits pequeños y por tema; si el diff mezcla temas, divídelo en varios commits.
- Si el repo no tiene `origin` o el push falla (auth, rama protegida, rechazo), informa el error textual y detente; no cambies remotos ni credenciales por tu cuenta.
- Termina con una línea: rama, commits creados y si se pusheó o está pendiente de aprobación.
