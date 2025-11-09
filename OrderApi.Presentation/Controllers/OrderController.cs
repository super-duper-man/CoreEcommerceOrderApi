using Microsoft.AspNetCore.Http;
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
    public class OrderController(IOrder orderInterface, IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await orderInterface.GetAllAsync();

            if (!orders.Any())
                return NotFound("No rder is in saerver");

            var (_, _orders) = OrderConversion.FromEntity(null, orders);

            return _orders!.Any() ? Ok(_orders) : NotFound();
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
    }
}
