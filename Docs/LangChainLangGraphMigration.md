# Migracion a smartinventory-ai-core-main

Esta guia deja `ProjectNetIa` consumiendo el microservicio `smartinventory-ai-core-main`, que si usa `FastAPI + LangChain + LangGraph + Gemini`, alineandose mejor con el taller.

## Arquitectura objetivo

- React consume solo `ProjectNetIa`.
- `ProjectNetIa` reenvia el chat a `http://localhost:8000`.
- `smartinventory-ai-core-main` procesa el turno conversacional.
- El microservicio AI consulta catalogo, inventario y ventas a `ProjectNetIa`.

## Requisitos del repo hermano

Ubicacion esperada:

```text
..\smartinventory-ai-core-main
```

Archivos requeridos:

- `..\smartinventory-ai-core-main\.venv`
- `..\smartinventory-ai-core-main\.env`

Contenido minimo esperado para `.env`:

```env
GOOGLE_API_KEY=tu_api_key_de_google_ai_studio
GOOGLE_MODEL=gemini-2.5-flash
NET_BACKEND_URL=http://localhost:5083
NET_BACKEND_TIMEOUT_SECONDS=5.0
```

## Arranque integrado

Desde `ProjectNetIa`:

```powershell
.\Scripts\Start-IntegratedStack-AI.ps1
```

Esto levanta:

- `.NET` en `http://localhost:5083`
- `smartinventory-ai-core-main` en `http://127.0.0.1:8000`

Luego levanta el frontend React por separado desde `smartinventory-client`.

## Notas importantes

- `Api/appsettings.json` ya apunta a `http://localhost:8000`, por lo que `.NET` no necesita cambio adicional si el microservicio AI corre en ese puerto.
- El contrato esperado por `.NET` es `POST /chat/message`, y `smartinventory-ai-core-main` ya expone ese alias.
- Sin `GOOGLE_API_KEY`, el microservicio AI no puede iniciar correctamente.
