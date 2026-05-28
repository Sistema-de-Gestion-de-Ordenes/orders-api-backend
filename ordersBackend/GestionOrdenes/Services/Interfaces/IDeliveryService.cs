using OrderManagement.Models.DTOs.Deliveries;

namespace OrderManagement.Services.Interfaces;

public interface IDeliveryService
{
    Task<IEnumerable<DeliveryDto>> GetAllAsync();
    Task<DeliveryDetailDto> GetDetailByIdAsync(int id);
    Task<DeliveryDto> UploadEvidenceAsync(int orderId, UploadEvidenceDto dto);
    Task UpdateStatusAsync(int id, string newStatus);
}
