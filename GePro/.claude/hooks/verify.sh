#!/usr/bin/env bash
# Hook Stop: compila y corre los tests al terminar. Si fallan, devuelve el error
# al modelo (exit 2) para que lo corrija antes de dar la tarea por terminada.
input=$(cat)
# Evita bucle infinito: si ya estamos continuando por un fallo anterior, no reintentar.
echo "$input" | grep -Eq '"stop_hook_active"[[:space:]]*:[[:space:]]*true' && exit 0

cd "$CLAUDE_PROJECT_DIR" || exit 0

out=$(dotnet build GePro.slnx --nologo -v q 2>&1) || {
  echo "dotnet build falló:" >&2; echo "$out" | tail -n 40 >&2; exit 2; }
out=$(dotnet test GePro.slnx --nologo --no-build 2>&1) || {
  echo "dotnet test falló:" >&2; echo "$out" | tail -n 40 >&2; exit 2; }
exit 0
