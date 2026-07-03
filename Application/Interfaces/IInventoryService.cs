using ProjectNetIa.Application.DTOs.Inventory;

namespace ProjectNetIa.Application.Interfaces;

public interface IInventoryService
{
    Task<IReadOnlyList<InventoryResponse>> GetInventoryAsync();

    Task<IReadOnlyList<InventoryResponse>> GetLowStockAsync();

    Task<InventoryResponse?> GetInventoryByVariantIdAsync(Guid productVariantId);

    Task<InventoryResponse> AdjustInventoryAsync(AdjustInventoryRequest request);
}
