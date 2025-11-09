using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;
using Resource.Share.Lib.Logs;
using Resource.Share.Lib.Responses;
using System.Linq.Expressions;

namespace OrderApi.Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext dbContext) : IOrder
    {
        public async Task<Response> CreateAsync(Order entity)
        {
            try
            {
                var order = dbContext.Orders.Add(entity).Entity;
                await dbContext.SaveChangesAsync();
                return (order.Id > 0) ? new Response(true, "Order placed successfuly") : new Response(false, "Error occurred while placing order");
            }
            catch(Exception ex)
            {
                LogException.LogExceptions(ex);

                return new Response(false, "Error occured while placing order");
            }
        }

        public async Task<Response> DeleteAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if(order == null)
                {
                    return new Response(false, "Order not found");
                }

                 dbContext.Orders.Remove(entity);
                await dbContext.SaveChangesAsync();
                return new Response(true, "Order deleted successfully");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                return new Response(false, "Error occurred while placing order");
            }
        }

        public async Task<Order> FindByIdAsync(int id)
        {
            try
            {
                var order = await dbContext.Orders.FindAsync(id);
                if (order is null)
                    return null!;

                return order;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("Error occurred while placing order");
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                var orders = await dbContext.Orders.AsNoTracking().ToListAsync();
                if(orders is null || orders.Count == 0)
                    return Enumerable.Empty<Order>();

                return orders;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("Error occurred while placing order");
            }
        }

        public async Task<Order> GetAsync(Expression<Func<Order, bool>> predicte)
        {
            try
            {
                var order = await dbContext.Orders.FindAsync(predicte);

                return order is not null ? order : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("Error occurred while placing order");
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var orders = await dbContext.Orders.Where(predicate).AsNoTracking().ToListAsync();

                return orders is not null ? orders : Enumerable.Empty<Order>();
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("Error occurred while placing order");
            }
        }

        public async Task<Response> UpdateAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if(order is null)
                    return new Response(false, "Order not found");

                dbContext.Entry(order).State = EntityState.Detached;
                dbContext.Orders.Update(entity);
                await dbContext.SaveChangesAsync();

                return new Response(true, "Order updated successfully");

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("Error occurred while placing order");
            }
        }
    }
}
