from __future__ import annotations

import re
from dataclasses import dataclass
from typing import Optional

import httpx
from fastapi import FastAPI, HTTPException

from .dotnet_client import DotNetApiClient
from .models import (
    ChatMessageRequest,
    ChatMessageResponse,
    CreateSaleRequest,
    ProductVariantSearchResponse,
    SaleItemRequest,
)

app = FastAPI(title="ProjectNetIa Chatbot", version="1.0.0")


@dataclass
class PendingPurchase:
    variant_id: str
    product_name: str
    sku: str
    color_name: str
    size_name: str
    quantity: int
    unit_price: float


@dataclass
class SessionState:
    pending_purchase: Optional[PendingPurchase] = None


SESSIONS: dict[str, SessionState] = {}
COLOR_HINTS = [
    "negro",
    "blanco",
    "azul",
    "rojo",
    "verde",
    "gris",
    "amarillo",
    "beige",
]
SIZE_HINTS = ["xs", "s", "m", "l", "xl", "xxl", "28", "30", "32", "34", "36", "38", "40"]
CONFIRMATION_HINTS = {
    "si",
    "sí",
    "confirma",
    "confirmar",
    "dale",
    "acepto",
    "comprar",
}
CANCEL_HINTS = {"no", "cancelar", "cancela", "ya no", "mejor no"}

dotnet_client = DotNetApiClient()


@app.on_event("shutdown")
async def shutdown_event() -> None:
    await dotnet_client.close()


@app.get("/")
async def root() -> dict[str, str]:
    return {"message": "FastAPI chatbot running"}


@app.post("/chat/message", response_model=ChatMessageResponse)
async def chat_message(request: ChatMessageRequest) -> ChatMessageResponse:
    session = SESSIONS.setdefault(request.sessionId, SessionState())
    normalized_message = normalize_message(request.message)

    if session.pending_purchase is not None:
        if contains_any(normalized_message, CONFIRMATION_HINTS):
            return await complete_purchase(request.sessionId, session)

        if contains_any(normalized_message, CANCEL_HINTS):
            session.pending_purchase = None
            return ChatMessageResponse(
                response="Compra cancelada. Si quieres, te ayudo a buscar otra prenda.",
                state="SEARCHING_PRODUCT",
            )

        pending = session.pending_purchase
        return ChatMessageResponse(
            response=(
                f"Tengo pendiente {pending.quantity} unidad(es) de {pending.product_name} "
                f"color {pending.color_name}, talla {pending.size_name}. "
                "Responde 'si' para confirmar o 'no' para cancelar."
            ),
            state="WAITING_CONFIRMATION",
        )

    extracted = extract_purchase_intent(request.message)
    if extracted["query"] is None:
        return ChatMessageResponse(
            response=(
                "Puedo ayudarte a comprar una prenda. Dime el producto, y si puedes "
                "tambien incluye color y talla. Ejemplo: camiseta negra talla M."
            ),
            state="SEARCHING_PRODUCT",
        )

    try:
        variants = await dotnet_client.search_variants(
            query=extracted["query"],
            color=extracted["color"],
            size=extracted["size"],
            only_available=True,
        )
    except httpx.HTTPError as exc:
        raise HTTPException(status_code=502, detail=f"No fue posible consultar .NET: {exc}") from exc

    if not variants:
        return ChatMessageResponse(
            response="No encontre productos disponibles con esa descripcion. Intenta con otro color, talla o nombre.",
            state="PRODUCT_NOT_FOUND",
        )

    selected_variant = variants[0]
    session.pending_purchase = PendingPurchase(
        variant_id=selected_variant.productVariantId,
        product_name=selected_variant.productName,
        sku=selected_variant.sku,
        color_name=selected_variant.colorName,
        size_name=selected_variant.sizeName,
        quantity=extracted["quantity"],
        unit_price=selected_variant.price,
    )

    total = selected_variant.price * extracted["quantity"]
    return ChatMessageResponse(
        response=(
            f"Encontre {selected_variant.productName} color {selected_variant.colorName}, "
            f"talla {selected_variant.sizeName}. Hay {selected_variant.quantity} unidades disponibles. "
            f"El total por {extracted['quantity']} unidad(es) es ${total:,.0f}. "
            "Responde 'si' para confirmar la compra."
        ),
        state="WAITING_CONFIRMATION",
    )


async def complete_purchase(session_id: str, session: SessionState) -> ChatMessageResponse:
    pending = session.pending_purchase
    if pending is None:
        return ChatMessageResponse(
            response="No hay una compra pendiente para confirmar.",
            state="SEARCHING_PRODUCT",
        )

    payload = CreateSaleRequest(
        saleOriginId=2,
        items=[
            SaleItemRequest(
                productVariantId=pending.variant_id,
                quantity=pending.quantity,
            )
        ],
    )

    try:
        sale = await dotnet_client.create_sale(payload)
    except httpx.HTTPStatusError as exc:
        session.pending_purchase = None
        backend_message = exc.response.text.strip()
        return ChatMessageResponse(
            response=f"No pude completar la compra. El backend respondio: {backend_message}",
            state="ERROR",
        )
    except httpx.HTTPError as exc:
        raise HTTPException(status_code=502, detail=f"No fue posible registrar la venta en .NET: {exc}") from exc

    session.pending_purchase = None
    SESSIONS[session_id] = session
    return ChatMessageResponse(
        response=(
            f"Compra realizada exitosamente. La venta fue registrada desde el chatbot "
            f"y se genero la factura {sale.invoiceNumber}."
        ),
        state="SALE_COMPLETED",
        invoiceNumber=sale.invoiceNumber,
        saleOrigin="CHATBOT",
    )


def normalize_message(message: str) -> str:
    return re.sub(r"\s+", " ", message.strip().lower())


def contains_any(message: str, hints: set[str]) -> bool:
    return any(hint in message for hint in hints)


def extract_purchase_intent(message: str) -> dict[str, str | int | None]:
    normalized = normalize_message(message)
    quantity = extract_quantity(normalized)
    color = next((color.title() for color in COLOR_HINTS if color in normalized), None)
    size = extract_size(normalized)

    clean_query = normalized
    for token in COLOR_HINTS + SIZE_HINTS + ["talla", "quiero", "comprar", "una", "un", "unos", "unas", "de"]:
        clean_query = re.sub(rf"\b{re.escape(token)}\b", " ", clean_query)

    clean_query = re.sub(r"\b\d+\b", " ", clean_query)
    clean_query = re.sub(r"\s+", " ", clean_query).strip()
    query = clean_query.title() if clean_query else None

    return {
        "query": query,
        "color": color,
        "size": size,
        "quantity": quantity,
    }


def extract_quantity(message: str) -> int:
    number_match = re.search(r"\b(\d+)\b", message)
    if number_match:
        return max(1, int(number_match.group(1)))

    word_quantities = {
        "un": 1,
        "una": 1,
        "uno": 1,
        "dos": 2,
        "tres": 3,
        "cuatro": 4,
        "cinco": 5,
    }
    for word, value in word_quantities.items():
        if re.search(rf"\b{word}\b", message):
            return value

    return 1


def extract_size(message: str) -> str | None:
    talla_match = re.search(r"\btalla\s+([a-z0-9]+)\b", message)
    if talla_match:
        return talla_match.group(1).upper()

    for size in SIZE_HINTS:
        if re.search(rf"\b{re.escape(size)}\b", message):
            return size.upper()

    return None
