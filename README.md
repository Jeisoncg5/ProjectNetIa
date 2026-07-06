## Ejecucion local

Para levantar el backend `.NET` y el microservicio `smartinventory-ai-core-main` con un solo comando:

```powershell
.\Scripts\Start-IntegratedStack.ps1
```

Para detener ambos servicios:

```powershell
.\Scripts\Stop-IntegratedStack.ps1
```

Requisitos:

- `smartinventory-ai-core-main` debe existir como carpeta hermana de este repo.
- `smartinventory-ai-core-main\.venv` y `smartinventory-ai-core-main\.env` deben estar creados.
