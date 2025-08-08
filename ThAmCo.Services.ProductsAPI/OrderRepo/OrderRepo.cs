using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThAmCo.ProductsAPI.Data;


namespace ThAmCo.ProductsAPI.OrderRepo
{
    public class OrderRepo : IOrder
    {
        private readonly List<Order> _orders;
        private int _nextOrderId = 1;

        public OrderRepo()
        {
            _orders = new List<Order>();
        }

        public async Task<Order> CreateOrderAsync()
        {
            var order = new Order
            {
                OrderId = _nextOrderId++,
                Products = new List<Product>(),
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending
            };

            _orders.Add(order);
            return await Task.FromResult(order);
        }

        public async Task<Order?> GetOrderAsync(int orderId)
        {
            return await Task.FromResult(_orders.FirstOrDefault(o => o.OrderId == orderId));
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await Task.FromResult(_orders.AsEnumerable());
        }

        public async Task<bool> AddProductToOrderAsync(int orderId, Product product)
        {
            var order = await GetOrderAsync(orderId);
            if (order == null) return false;

            order.Products.Add(product);
            return true;
        }

        public async Task<bool> RemoveProductFromOrderAsync(int orderId, int productId)
        {
            var order = await GetOrderAsync(orderId);
            if (order == null) return false;

            var product = order.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null) return false;

            return order.Products.Remove(product);
        }

        public async Task<bool> UpdateOrderAsync(Order updatedOrder)
        {
            var existingOrder = await GetOrderAsync(updatedOrder.OrderId);
            if (existingOrder == null) return false;

            existingOrder.Products = updatedOrder.Products;
            existingOrder.OrderDate = updatedOrder.OrderDate;
            existingOrder.Status = updatedOrder.Status;

            return true;
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await GetOrderAsync(orderId);
            if (order == null) return false;

            return _orders.Remove(order);
        }
    }
}

