using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.Dtos;
using OrderApi.Application.Dtos.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using Resource.Share.Lib.Responses;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController(IOrder orderInterface, IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await orderInterface.GetAllAsync();

            if (!orders.Any())
                return NotFound("No rder is in server");

            var (_, _orders) = OrderConversion.FromEntity(null, orders);

            return _orders!.Any() ? Ok(_orders) : NotFound();
        }

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<OrderDto>> GetClientOrders(int clientId)
        {
            if (clientId <= 0)
                return BadRequest("Invalid Data Provided!");

            var orders = await orderService.GetOrderByClientId(clientId);

            return orders!.Any() ? Ok(orders) : NotFound("No order created by this client");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var orderEntity = await orderInterface.FindByIdAsync(id);
            var (order ,_ ) = OrderConversion.FromEntity(orderEntity, null!);

            if (order == null)
                return NotFound();

            return order;
        }

        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<IEnumerable<OrderDetailsDto>>> GetOrderDetails(int orderId)
        {
            if (orderId <= 0)
                return BadRequest("Invalid Data Provided!");

            var orderDetail = await orderService.GetOrderDetails(orderId);

            return orderDetail is not null ? Ok(orderDetail) : NotFound();

        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDto orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Submitted Data!");

            var order = OrderConversion.ToEntity(orderDto);

            var response = await orderInterface.CreateAsync(order);

            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder([FromBody] OrderDto orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invlaid Submitted Data!");

            var orderEntity = OrderConversion.ToEntity(orderDto);

            var response  = await orderInterface.UpdateAsync(orderEntity);

            return !response.Flag ? BadRequest(response) : Ok(response);

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Response>> DeleteOrder(int id)
        {
            var order = await orderInterface.FindByIdAsync(id);
            var response = await orderInterface.DeleteAsync(order);

            return response.Flag ? Ok(response) : BadRequest(response);
        }
    }
}
