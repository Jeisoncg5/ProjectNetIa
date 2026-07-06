# FastAPI Chatbot

Servicio FastAPI minimo para integrar el chatbot con la API de .NET.

## 1. Crear entorno virtual

```powershell
cd FastApiChatbot
python -m venv .venv
.venv\Scripts\Activate.ps1
pip install -r requirements.txt
```

## 2. Configurar variables

```powershell
Copy-Item .env.example .env
```

Por defecto el servicio de Python consumira la API de .NET en `https://localhost:7221`.

La verificacion SSL viene desactivada para desarrollo local con el certificado de ASP.NET Core.

## 3. Ejecutar FastAPI

```powershell
uvicorn app.main:app --reload --port 8000
```

## 4. Flujo disponible

- Busca productos reales en `.NET` usando `GET /api/products/variants/search`.
- Mantiene una compra pendiente por `sessionId`.
- Confirma la compra con `POST /api/sales` usando `saleOriginId = 2`.
- Expone `POST /chat/message`, que es el endpoint consumido por `.NET`.

## 5. Ejemplo de prueba

```json
{
  "sessionId": "session-001",
  "message": "Quiero comprar una camiseta negra talla M"
}
```

Luego envia:

```json
{
  "sessionId": "session-001",
  "message": "si"
}
```
