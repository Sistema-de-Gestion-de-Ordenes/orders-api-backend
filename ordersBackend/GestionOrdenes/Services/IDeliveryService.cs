using OrderManagement.Models.DTOs.Deliveries;

namespace OrderManagement.Services;

public interface IDeliveryService
{
    Task<IEnumerable<DeliverySummaryResponse>> GetAllAsync();
    Task<DeliveryDetailResponse> GetByIdAsync(int id);
    Task<DeliveryResponse> CreateAsync(CreateDeliveryRequest dto);
    Task<DeliveryResponse> UpdateAsync(int id, UpdateDeliveryRequest dto);
    Task DeleteAsync(int id);
    Task<DeliveryStatusResponse> UpdateStatusAsync(int id, UpdateStatusRequest dto, string performedBy);
}
