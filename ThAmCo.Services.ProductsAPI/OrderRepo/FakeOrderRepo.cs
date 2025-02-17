using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThAmCo.ProductsAPI.Data;

namespace ThAmCo.ProductsAPI.OrderRepo
{
    public class FakeOrderRepository
    {
        
        private readonly Dictionary<int, Order> _orders;
        private int _nextOrderId = 1;

        public FakeOrderRepository()
        {
            _orders = new Dictionary<int, Order>();
            SeedData();
        }

        private void SeedData()
        {
            // Sample products
            var product1 = new Product { Id = 1, Name = "Hair Brush", Price = 3.00f, Description = "A fashionable product for combing your hair" };
            var product2 = new Product { Id = 2, Name = "Cat Treats", Price = 6.50f, Description = "Five paws up for the best treats" };
            var product3 = new Product { Id = 3, Name = "Headphones", Price = 56.99f, Description = "Award winning headphones" };
            var product4 = new Product { Id = 4, Name = "Notebook", Price = 2.50f, Description = "A handy notebook for your thoughts" };
            var product5 = new Product { Id = 5, Name = "Travel Mug", Price = 15.00f, Description = "Keeps your drinks hot or cold" };
            var product6 = new Product { Id = 6, Name = "Bluetooth Speaker", Price = 29.99f, Description = "Portable sound for any occasion" };
            var product7 = new Product { Id = 7, Name = "Yoga Mat", Price = 25.00f, Description = "Comfortable mat for your yoga practice" };
            var product8 = new Product { Id = 8, Name = "Water Bottle", Price = 10.00f, Description = "Stay hydrated on the go" };
            var product9 = new Product { Id = 9, Name = "Desk Lamp", Price = 18.00f, Description = "A stylish lamp for your workspace" };
            var product10 = new Product { Id = 10, Name = "Sunglasses", Price = 50.00f, Description = "Protect your eyes in style" };
            var product11 = new Product { Id = 11, Name = "Fitness Tracker", Price = 70.00f, Description = "Track your health and fitness goals" };
            var product12 = new Product { Id = 12, Name = "Instant Pot", Price = 80.00f, Description = "Versatile cooker for quick meals" };
            var product13 = new Product { Id = 13, Name = "Running Shoes", Price = 120.00f, Description = "Comfortable shoes for long runs" };
            var product14 = new Product { Id = 14, Name = "Electric Toothbrush", Price = 40.00f, Description = "Keep your teeth clean and healthy" };
            var product15 = new Product { Id = 15, Name = "Wireless Charger", Price = 25.00f, Description = "Conveniently charge your devices" };
            var product16 = new Product { Id = 16, Name = "Portable Power Bank", Price = 20.00f, Description = "Charge your devices on the go" };
            var product17 = new Product { Id = 17, Name = "Smart Thermostat", Price = 150.00f, Description = "Control your home temperature remotely" };
            var product18 = new Product { Id = 18, Name = "Gaming Headset", Price = 60.00f, Description = "Enhance your gaming experience" };
            var product19 = new Product { Id = 19, Name = "Kitchen Scale", Price = 15.00f, Description = "Accurate measurements for your recipes" };
            var product20 = new Product { Id = 20, Name = "Electric Kettle", Price = 30.00f, Description = "Boil water quickly and efficiently" };

            // Sample orders
            var order1 = new Order
            {
                OrderId = _nextOrderId++,
                OrderDate = DateTime.Now.AddDays(-5), //Current date minus x days
                Status = OrderStatus.Delivered,
                Products = new List<Product> { product1, product2, product20, product16 }
            };

            var order2 = new Order
            {
                OrderId = _nextOrderId++,
                OrderDate = DateTime.Now.AddDays(-2),
                Status = OrderStatus.Processing,
                Products = new List<Product> { product3, product4, product10, product18 }
            };

            var order3 = new Order
            {
                OrderId = _nextOrderId++,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                Products = new List<Product> { product2, product3, product17, product11}
            };

            // Add orders to the dictionary
            _orders[order1.OrderId] = order1;
            _orders[order2.OrderId] = order2;
            _orders[order3.OrderId] = order3;
        }

        public async Task<Order> CreateOrderAsync()
        {
            var order = new Order
            {
                OrderId = _nextOrderId++,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                Products = new List<Product>()
            };
            _orders[order.OrderId] = order;
            return await Task.FromResult(order);
        }

        public async Task<Order?> GetOrderAsync(int orderId)
        {
            if (_orders.TryGetValue(orderId, out var order))
            {
                return await Task.FromResult(order);
            }
            return await Task.FromResult<Order?>(null);
        }

        public async Task<bool> AddProductToOrderAsync(int orderId, Product product)
        {
            if (_orders.TryGetValue(orderId, out var order))
            {
                order.Products!.Add(product);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> RemoveProductFromOrderAsync(int orderId, int productId)
        {
            if (_orders.TryGetValue(orderId, out var order))
            {
                var productToRemove = order.Products.FirstOrDefault(p => p.Id == productId);
                if (productToRemove != null)
                {
                    order.Products.Remove(productToRemove);
                    return await Task.FromResult(true);
                }
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            return await Task.FromResult(_orders.Remove(orderId));
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await Task.FromResult(_orders.Values.ToList());
        }

        public async Task<bool> UpdateOrderAsync(Order updatedOrder)
        {
            if (_orders.TryGetValue(updatedOrder.OrderId, out var existingOrder))
            {
                // Update existing order with the new details
                existingOrder.Products = updatedOrder.Products;
                existingOrder.OrderDate = updatedOrder.OrderDate;
                existingOrder.Status = updatedOrder.Status;
                
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
    }

}
