---
name: grill-with-docs
description: Interroga al usuario sobre una idea o feature hasta alcanzar entendimiento compartido, y actualiza CONTEXT.md y Docs/adr/ a medida que se toman decisiones. Úsala al empezar cualquier feature nueva o cuando el plan sea difuso.
---

# grill-with-docs

Objetivo: que usuario y agente digan lo mismo con las mismas palabras **antes** de escribir una spec.

## Pasos

1. Lee `CONTEXT.md`, `Docs/adr/` y, si existe, la spec relacionada en `Docs/specs/`.
2. Explora el código relevante en vez de preguntar lo que se puede averiguar leyendo.
3. Entrevista al usuario **de a una pregunta por vez**, siguiendo las ramas de decisión: actor, objetivo, reglas, casos límite, errores, datos, pantallas.
   - Para cada pregunta propone tu respuesta recomendada.
   - Si un término es ambiguo o contradice `CONTEXT.md`, señálalo y pide que se resuelva.
4. Cuando se cristaliza una decisión, actualízala en el momento:
   - Término nuevo o cambiado → `CONTEXT.md`.
   - Decisión de arquitectura difícil de revertir → nuevo `Docs/adr/NNNN-titulo.md` (formato de `0001`).
5. Termina con un resumen de lo acordado y las preguntas abiertas. Sugiere `/to-spec`.

## Reglas

- No escribas código ni la spec en esta fase.
- No inventes términos de dominio: si no están en `CONTEXT.md`, se acuerdan primero.
