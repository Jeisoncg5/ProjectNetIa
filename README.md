## Ejecucion local

Tienes dos opciones de chatbot:

- `FastApiChatbot`: version simple basada en reglas para integracion local rapida.
- `smartinventory-ai-core-main`: version alineada con el taller, usando `FastAPI + LangChain + LangGraph + Gemini`.

### Opcion 1: FastApiChatbot

Para levantar el backend `.NET` y el microservicio `FastApiChatbot` con un solo comando:

```powershell
.\Scripts\Start-IntegratedStack.ps1
```

Para detener ambos servicios:

```powershell
.\Scripts\Stop-IntegratedStack.ps1
```

Requisitos:

- Tener `dotnet` instalado.
- Tener Python disponible en el sistema.
- Crear `FastApiChatbot\.venv`.
- Crear `FastApiChatbot\.env` a partir de `FastApiChatbot\.env.example`.

### Opcion 2: smartinventory-ai-core-main

Para levantar el backend `.NET` y el microservicio AI del taller:

```powershell
.\Scripts\Start-IntegratedStack-AI.ps1
```

Requisitos:

- Tener `dotnet` instalado.
- Tener Python disponible en el sistema.
- Tener el repo hermano `..\smartinventory-ai-core-main`.
- Crear `..\smartinventory-ai-core-main\.venv`.
- Crear `..\smartinventory-ai-core-main\.env` con `GOOGLE_API_KEY`.

Mas detalle en [Docs/LangChainLangGraphMigration.md](/C:/Users/Jeiso/OneDrive/Escritorio/ProjectNetIa/Docs/LangChainLangGraphMigration.md).
