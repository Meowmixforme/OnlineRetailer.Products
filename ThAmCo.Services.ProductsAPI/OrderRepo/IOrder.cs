using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThAmCo.ProductsAPI.Data;

namespace ThAmCo.ProductsAPI.OrderRepo
{
    public interface IOrder
    {
        Task<Order> CreateOrderAsync();
        Task<Order?> GetOrderAsync(int orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<bool> AddProductToOrderAsync(int orderId, Product product);
        Task<bool> RemoveProductFromOrderAsync(int orderId, int productId);
        Task<bool> UpdateOrderAsync(Order updatedOrder);
        Task<bool> DeleteOrderAsync(int orderId);
    }

}

