# Guía de integración FastAPI ↔ ASP.NET Core

## Proyecto

ProjectNetIa - Tienda de ropa con inventario inteligente y chatbot de ventas.

## Rol de la API .NET

La API en ASP.NET Core es el backend principal del negocio. Se encarga de:

- Gestionar productos.
- Gestionar variantes de ropa por talla y color.
- Consultar inventario.
- Validar stock.
- Crear ventas.
- Descontar inventario.
- Registrar ventas hechas por chatbot.
- Generar facturas.
- Exponer endpoints para React.
- Servir como fuente real de datos para FastAPI/LangChain.

FastAPI no debe modificar directamente la base de datos.

---

# URL base

Durante desarrollo local:

```text
https://localhost:PUERTO
```

Ejemplo:

```text
https://localhost:7045
```

El puerto real se debe tomar desde la ejecución de la API .NET o desde `launchSettings.json`.

## 1. Buscar productos/variantes para el chatbot

### Endpoint

`GET /api/products/variants/search`

### Uso

Este endpoint sirve para que el chatbot busque prendas reales en la tienda.

### Query parameters

| Parámetro | Tipo | Obligatorio | Descripción |
| --- | --- | --- | --- |
| `query` | `string` | No | Texto de búsqueda: camiseta, jean, chaqueta, etc. |
| `size` | `string` | No | Talla: S, M, L, XL, 32, 34, etc. |
| `color` | `string` | No | Color: Negro, Blanco, Azul, Rojo, etc. |
| `onlyAvailable` | `bool` | No | Si es `true`, solo devuelve productos con stock disponible. |

### Ejemplos

```text
GET /api/products/variants/search?query=camiseta
GET /api/products/variants/search?query=camiseta&color=Negro
GET /api/products/variants/search?query=camiseta&color=Negro&size=M&onlyAvailable=true
```

### Respuesta esperada

```json
[
  {
    "productVariantId": "11111111-1111-1111-1111-111111111111",
    "productId": "22222222-2222-2222-2222-222222222222",
    "productName": "Camiseta básica cuello redondo",
    "description": "Camiseta en algodón para uso casual",
    "categoryName": "Camisetas",
    "sku": "CAM-BAS-NEG-M",
    "sizeName": "M",
    "colorName": "Negro",
    "price": 45000,
    "quantity": 10,
    "isAvailable": true
  }
]
```

### Uso en LangChain

Este endpoint debe usarse cuando el usuario diga algo como:

```text
Quiero comprar una camiseta negra talla M
```

El chatbot debe extraer:

- `query = camiseta`
- `color = Negro`
- `size = M`
- `onlyAvailable = true`

## 2. Consultar inventario de una variante

### Endpoint

`GET /api/inventory/variant/{productVariantId}`

### Uso

Sirve para validar el stock actual de una variante específica.

### Ejemplo

```text
GET /api/inventory/variant/11111111-1111-1111-1111-111111111111
```

### Respuesta esperada

```json
{
  "inventoryId": "33333333-3333-3333-3333-333333333333",
  "productVariantId": "11111111-1111-1111-1111-111111111111",
  "productName": "Camiseta básica cuello redondo",
  "sku": "CAM-BAS-NEG-M",
  "sizeName": "M",
  "colorName": "Negro",
  "quantity": 10,
  "minimumQuantity": 3,
  "isLowStock": false
}
```

## 3. Crear venta desde chatbot

### Endpoint

`POST /api/sales`

### Uso

Este endpoint crea la venta, descuenta inventario, registra movimiento de inventario y genera factura.

Para ventas realizadas desde chatbot se debe enviar:

```json
"saleOriginId": 2
```

### Request body

```json
{
  "customerId": null,
  "saleOriginId": 2,
  "items": [
    {
      "productVariantId": "11111111-1111-1111-1111-111111111111",
      "quantity": 1
    }
  ]
}
```

### Respuesta esperada

```json
{
  "id": "44444444-4444-4444-4444-444444444444",
  "customerId": null,
  "saleOriginName": "Chatbot",
  "saleStatusName": "Completed",
  "createdAt": "2026-07-05T15:30:00Z",
  "invoiceNumber": "FAC-000001",
  "total": 45000,
  "items": [
    {
      "productVariantId": "11111111-1111-1111-1111-111111111111",
      "productName": "Camiseta básica cuello redondo",
      "sku": "CAM-BAS-NEG-M",
      "sizeName": "M",
      "colorName": "Negro",
      "quantity": 1,
      "unitPrice": 45000,
      "subtotal": 45000
    }
  ]
}
```

### Reglas importantes

La API .NET valida:

- Que la variante exista.
- Que el producto esté activo.
- Que haya stock suficiente.
- Que la cantidad sea mayor que cero.
- Que el inventario no quede negativo.
- Que la venta quede marcada como hecha desde chatbot.
- Que se genere la factura.

## 4. Consultar factura por número

### Endpoint

`GET /api/invoices/number/{invoiceNumber}`

### Ejemplo

```text
GET /api/invoices/number/FAC-000001
```

### Respuesta esperada

```json
{
  "id": "55555555-5555-5555-5555-555555555555",
  "invoiceNumber": "FAC-000001",
  "saleId": "44444444-4444-4444-4444-444444444444",
  "invoiceStatusName": "Issued",
  "saleOriginName": "Chatbot",
  "createdAt": "2026-07-05T15:30:00Z",
  "total": 45000,
  "items": [
    {
      "productName": "Camiseta básica cuello redondo",
      "sku": "CAM-BAS-NEG-M",
      "sizeName": "M",
      "colorName": "Negro",
      "quantity": 1,
      "unitPrice": 45000,
      "subtotal": 45000
    }
  ]
}
```

## 5. Enviar mensaje desde React hacia el chatbot vía .NET

### Endpoint expuesto por .NET

`POST /api/chat/message`

### Uso

React debe consumir este endpoint, no FastAPI directamente.

La API .NET reenvía el mensaje a FastAPI:

```text
POST http://localhost:8000/chat/message
```

### Request body

```json
{
  "sessionId": "abc-123",
  "message": "Quiero comprar una camiseta negra talla M"
}
```

### Respuesta esperada

```json
{
  "response": "Encontré Camiseta básica cuello redondo en color Negro, talla M. Hay 10 unidades disponibles. El total por 1 unidad es $45.000. ¿Deseas confirmar la compra?",
  "state": "WAITING_CONFIRMATION",
  "invoiceNumber": null,
  "saleOrigin": null
}
```

Cuando el usuario confirme:

```json
{
  "sessionId": "abc-123",
  "message": "Sí, confirma la compra"
}
```

Respuesta esperada:

```json
{
  "response": "Compra realizada exitosamente. La venta fue registrada desde el chatbot y se generó la factura FAC-000001.",
  "state": "SALE_COMPLETED",
  "invoiceNumber": "FAC-000001",
  "saleOrigin": "CHATBOT"
}
```

## Flujo recomendado para FastAPI/LangChain

### Caso: usuario quiere comprar una prenda

1. Recibir mensaje del usuario desde .NET.
2. Extraer intención, producto, color, talla y cantidad.
3. Llamar a:
   `GET /api/products/variants/search`
4. Si no hay resultados, responder que no se encontró la prenda.
5. Si hay resultados, revisar stock disponible.
6. Si hay stock, pedir confirmación al usuario.
7. Cuando el usuario confirme, llamar a:
   `POST /api/sales`
8. Tomar `invoiceNumber` de la respuesta.
9. Responder al usuario que la compra fue realizada.
10. Si se necesita detalle de factura, llamar a:
    `GET /api/invoices/number/{invoiceNumber}`

## Estados sugeridos del chatbot

- `SEARCHING_PRODUCT`
- `WAITING_CONFIRMATION`
- `SALE_COMPLETED`
- `PRODUCT_NOT_FOUND`
- `OUT_OF_STOCK`
- `ERROR`

## Ejemplo completo del flujo

### Usuario

```text
Quiero comprar una camiseta negra talla M
```

### FastAPI consulta .NET

```text
GET /api/products/variants/search?query=camiseta&color=Negro&size=M&onlyAvailable=true
```

### .NET responde

```json
[
  {
    "productVariantId": "11111111-1111-1111-1111-111111111111",
    "productName": "Camiseta básica cuello redondo",
    "sku": "CAM-BAS-NEG-M",
    "sizeName": "M",
    "colorName": "Negro",
    "price": 45000,
    "quantity": 10,
    "isAvailable": true
  }
]
```

### Chatbot responde

```text
Encontré Camiseta básica cuello redondo en color Negro, talla M. Hay 10 unidades disponibles. El precio es $45.000. ¿Deseas confirmar la compra?
```

### Usuario

```text
Sí, confirma la compra
```

### FastAPI crea venta en .NET

```json
POST /api/sales
{
  "customerId": null,
  "saleOriginId": 2,
  "items": [
    {
      "productVariantId": "11111111-1111-1111-1111-111111111111",
      "quantity": 1
    }
  ]
}
```

### .NET responde

```json
{
  "id": "44444444-4444-4444-4444-444444444444",
  "saleOriginName": "Chatbot",
  "invoiceNumber": "FAC-000001",
  "total": 45000
}
```

### Chatbot responde al usuario

```text
Compra realizada exitosamente. La venta fue registrada desde el chatbot y se generó la factura FAC-000001.
```
