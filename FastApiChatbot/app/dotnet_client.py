from __future__ import annotations

from urllib.parse import urlencode

import httpx

from .models import CreateSaleRequest, ProductVariantSearchResponse, SaleResponse
from .settings import settings


class DotNetApiClient:
    def __init__(self) -> None:
        self._client = httpx.AsyncClient(
            base_url=settings.dotnet_base_url.rstrip("/"),
            timeout=settings.request_timeout_seconds,
            verify=settings.dotnet_verify_ssl,
            follow_redirects=True,
        )

    async def close(self) -> None:
        await self._client.aclose()

    async def search_variants(
        self,
        *,
        query: str | None,
        color: str | None,
        size: str | None,
        only_available: bool = True,
    ) -> list[ProductVariantSearchResponse]:
        params = {
            "query": query,
            "color": color,
            "size": size,
            "onlyAvailable": str(only_available).lower(),
        }
        query_string = urlencode({k: v for k, v in params.items() if v})
        response = await self._client.get(f"/api/products/variants/search?{query_string}")
        response.raise_for_status()
        return [ProductVariantSearchResponse.model_validate(item) for item in response.json()]

    async def create_sale(self, payload: CreateSaleRequest) -> SaleResponse:
        response = await self._client.post("/api/sales", json=payload.model_dump())
        response.raise_for_status()
        return SaleResponse.model_validate(response.json())
