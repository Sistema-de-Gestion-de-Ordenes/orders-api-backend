using OrderManagement.Models.DTOs.Deliveries;

namespace OrderManagement.Services.Interfaces;

public interface IDeliveryService
{
    Task<IEnumerable<DeliveryDto>> GetAllAsync();
    Task<DeliveryDto> UploadEvidenceAsync(int orderId, UploadEvidenceDto dto);
}
