# 001 — Swagger en la API

- **Estado**: Aprobada
- **Aprobación**: explícita del usuario al pedir crear y ejecutar la feature (2026-11-03)
- **Fecha**: 2026-11-03
- **Capas afectadas**: API

## 1. Problema y objetivo

Explorar y probar los endpoints de `GePro.API` desde el navegador sin herramientas externas. Al depurar (F5 / `dotnet run`) debe abrirse Swagger UI automáticamente, en Chrome.

## 2. Alcance

**Dentro**
- Swagger UI en `/swagger`, servido sobre el documento OpenAPI existente (`/openapi/v1.json`).
- Disponible solo en entorno `Development`.
- Perfiles `http` y `https` de `launchSettings.json` abren `/swagger` al iniciar.
- Abrir en Chrome: se configura en Visual Studio (Browse With → Chrome → Set as Default).

**Fuera**
- Botón "Authorize" / esquema JWT (se añade con la spec de autenticación).
- Comentarios XML, versionado de API, Swagger en producción.
- Perfil `Container (Dockerfile)`.

## 3. Términos del dominio

Ninguno nuevo.

## 4. Reglas de negocio y validaciones

- **RN-01**: Swagger UI nunca se expone fuera de `Development`.
- **RN-02**: El navegador lo elige el entorno del desarrollador, no el repo (`launchSettings.json` no admite elegir navegador).

## 5. Contrato de API

| Método | Ruta | Request | Response | Errores |
|---|---|---|---|---|
| GET | /swagger/index.html | — | HTML de Swagger UI | 404 fuera de Development |
| GET | /openapi/v1.json | — | Documento OpenAPI | 404 fuera de Development |

## 6. Modelo de datos

No aplica.

## 7. Pantallas (Angular)

No aplica.

## 8. Criterios de aceptación

- [x] **CA-01**: Dado entorno Development, cuando se pide `/swagger/index.html`, entonces responde 200 con HTML de Swagger UI.
- [x] **CA-02**: Dado entorno Development, cuando se pide `/openapi/v1.json`, entonces responde 200 y es la fuente de Swagger UI.
- [x] **CA-03**: Dado entorno Production, cuando se pide `/swagger/index.html`, entonces responde 404.
- [x] **CA-04**: Dados los perfiles `http` y `https`, entonces `launchBrowser` es true y `launchUrl` es `swagger`.

## 9. Preguntas abiertas

- Nada pendiente.
