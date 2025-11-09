using OrderApi.Application.Dtos;
using OrderApi.Application.Dtos.Conversions;
using OrderApi.Application.Interfaces;
using Polly.Registry;
using System.Net.Http.Json;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderInterface,HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipline) : IOrderService
    {
        public async Task<ProductDto> GetProduct(int productId)
        {
            //Call product api  using HttpClient.
            //Redirect this call to the Api GateWay since product api is not responded.
            var getproduct = await httpClient.GetAsync($"/api/products/{productId}");

            if (!getproduct.IsSuccessStatusCode)
               return null!;

            var product = await getproduct.Content.ReadFromJsonAsync<ProductDto>();
            return product!;
        }

        public async Task<AppUserDto> GetUser(int userId)
        {
            var getuser = await httpClient.GetAsync($"/api/product/{userId}");

            if (!getuser.IsSuccessStatusCode)
                return null!;

            var user = await getuser.Content.ReadFromJsonAsync<AppUserDto>();
            return user!;
        }


        public async Task<IEnumerable<OrderDto>> GetOrderByClientId(int clientId)
        {
            //Get all client's orders
            var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);

            if (!orders.Any())
                return null!;

            //Convert from entoty to dto
            var(_, _orders) = OrderConversion.FromEntity(null, orders);
            return _orders!;
        }

        public async Task<OrderDetailsDto> GetOrderDetails(int orderId)
        {
            var order = await orderInterface.FindByIdAsync(orderId);

            if (order is null || order.Id <= 0)
                return null!;

            //Get retry pipline
            var retryPipline = resiliencePipline.GetPipeline("retry-pipline");

            //Prepare product
            var productDto = await retryPipline.ExecuteAsync(async (token) => await GetProduct(order.ProductId));

            //Prepare Client
            var appUserDto = await retryPipline.ExecuteAsync(async token => await GetUser(order.ClientId));

            //Populate order details
            return new OrderDetailsDto(
                order.Id,
                order.ProductId,
                order.ClientId,
                appUserDto.Name,
                appUserDto.Email,
                appUserDto.Address,
                appUserDto.Phone,
                productDto.Name,
                order.PurchaseQuantity,
                productDto.Price,
                (productDto.ProductQuantity * order.PurchaseQuantity),
                order.OrderDate
                );
        }
    }
}
