using OrderApi.Domain.Entities;

namespace OrderApi.Application.Dtos.Conversions
{
    public static class OrderConversion
    {
        public static Order ToEntity(OrderDto order)
        {
            return new Order() { 
                Id = order.Id,
                ClientId = order.ClientId,
                OrderDate = order.OrderDate,
                ProductId = order.ProductId,
                PurchaseQuantity = order.PurchaseQuantity
            };
        }

        public static (OrderDto?, IEnumerable<OrderDto>?) FromEntity(Order? order, IEnumerable<Order> orders)
        {
            if (order is not null || orders is null)
            {
                var singleOrder = new OrderDto(
                        order!.Id,
                        order.ProductId,
                        order.ClientId,
                        order.PurchaseQuantity,
                        order.OrderDate
                    );
                return (singleOrder, null);
            }

            if (orders is not null)
            {
                var _orders = orders.Select(o => new OrderDto(
                        o.Id,
                        o.ProductId,
                        o.ClientId,
                        o.PurchaseQuantity,
                        o.OrderDate
                    ));

                return (null, _orders);
            }
            
        }
    }
}
