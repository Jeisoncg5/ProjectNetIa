from __future__ import annotations

from typing import Optional

from pydantic import BaseModel, Field


class ChatMessageRequest(BaseModel):
    sessionId: str = Field(min_length=1)
    message: str = Field(min_length=1)


class ChatMessageResponse(BaseModel):
    response: str
    state: str
    invoiceNumber: Optional[str] = None
    saleOrigin: Optional[str] = None


class ProductVariantSearchResponse(BaseModel):
    productVariantId: str
    productId: str
    productName: str
    description: Optional[str] = None
    categoryName: str
    sku: str
    sizeName: str
    colorName: str
    price: float
    quantity: int
    isAvailable: bool


class SaleItemRequest(BaseModel):
    productVariantId: str
    quantity: int


class CreateSaleRequest(BaseModel):
    customerId: Optional[str] = None
    saleOriginId: int = 2
    items: list[SaleItemRequest]


class SaleResponse(BaseModel):
    id: str
    customerId: Optional[str] = None
    saleOriginName: str
    saleStatusName: str
    createdAt: str
    invoiceNumber: str
    total: float
