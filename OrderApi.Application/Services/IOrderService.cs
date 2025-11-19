using OrderApi.Application.Dtos;

namespace OrderApi.Application.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetOrderByClientId(int clientId);
        Task<OrderDetailsDto> GetOrderDetails(int orderId);
        Task<AppUserDto> GetUser(int userId);
        Task<ProductDto> GetProduct(int productId);
    }
}
