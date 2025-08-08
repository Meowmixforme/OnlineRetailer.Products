using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThAmCo.ProductsAPI.Data;
using ThAmCo.ProductsAPI.OrderRepo;
using Xunit;

namespace ThAmCo.Orders.Tests
{
    public class UpdateOrderAsyncTests
    {
        private readonly FakeOrderRepository _repository;

        public UpdateOrderAsyncTests()
        {
            _repository = new FakeOrderRepository();
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldUpdateExistingOrder()
        {
            // Arrange
            var orderId = 1;
            var initialOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(initialOrder);

            var updatedOrder = new Order
            {
                OrderId = orderId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Processing,
                Products = new List<Product> { new Product { Id = 100, Name = "New Product", Price = 10.0f } }
            };

            // Act
            var result = await _repository.UpdateOrderAsync(updatedOrder);

            // Assert
            Assert.True(result);
            var retrievedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(retrievedOrder);
            Assert.Equal(updatedOrder.OrderDate, retrievedOrder.OrderDate);
            Assert.Equal(updatedOrder.Status, retrievedOrder.Status);
            Assert.Equal(updatedOrder.Products.Count, retrievedOrder.Products.Count);
            Assert.Equal(updatedOrder.Products.First().Id, retrievedOrder.Products.First().Id);
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldReturnFalseForNonExistentOrder()
        {
            // Arrange
            var nonExistentOrderId = 9999;
            var updatedOrder = new Order
            {
                OrderId = nonExistentOrderId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Processing,
                Products = new List<Product>()
            };

            // Act
            var result = await _repository.UpdateOrderAsync(updatedOrder);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldUpdateOrderStatus()
        {
            // Arrange
            var orderId = 1;
            var initialOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(initialOrder);

            var updatedOrder = new Order
            {
                OrderId = orderId,
                OrderDate = initialOrder.OrderDate,
                Status = OrderStatus.Delivered,
                Products = initialOrder.Products
            };

            // Act
            var result = await _repository.UpdateOrderAsync(updatedOrder);

            // Assert
            Assert.True(result);
            var retrievedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(retrievedOrder);
            Assert.Equal(OrderStatus.Delivered, retrievedOrder.Status);
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldUpdateOrderDate()
        {
            // Arrange
            var orderId = 1;
            var initialOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(initialOrder);

            var newOrderDate = DateTime.Now.AddDays(1);
            var updatedOrder = new Order
            {
                OrderId = orderId,
                OrderDate = newOrderDate,
                Status = initialOrder.Status,
                Products = initialOrder.Products
            };

            // Act
            var result = await _repository.UpdateOrderAsync(updatedOrder);

            // Assert
            Assert.True(result);
            var retrievedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(retrievedOrder);
            Assert.Equal(newOrderDate, retrievedOrder.OrderDate);
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldUpdateProducts()
        {
            // Arrange
            var orderId = 1;
            var initialOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(initialOrder);

            var newProducts = new List<Product>
            {
                new Product { Id = 101, Name = "New Product 1", Price = 10.0f },
                new Product { Id = 102, Name = "New Product 2", Price = 20.0f }
            };

            var updatedOrder = new Order
            {
                OrderId = orderId,
                OrderDate = initialOrder.OrderDate,
                Status = initialOrder.Status,
                Products = newProducts
            };

            // Act
            var result = await _repository.UpdateOrderAsync(updatedOrder);

            // Assert
            Assert.True(result);
            var retrievedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(retrievedOrder);
            Assert.Equal(newProducts.Count, retrievedOrder.Products.Count);
            Assert.All(newProducts, product => Assert.Contains(retrievedOrder.Products, p => p.Id == product.Id));
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldNotAffectOtherOrders()
        {
            // Arrange
            var orderId1 = 1;
            var orderId2 = 2;
            var initialOrder1 = await _repository.GetOrderAsync(orderId1);
            var initialOrder2 = await _repository.GetOrderAsync(orderId2);
            Assert.NotNull(initialOrder1);
            Assert.NotNull(initialOrder2);

            var updatedOrder = new Order
            {
                OrderId = orderId1,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Delivered,
                Products = new List<Product> { new Product { Id = 100, Name = "New Product", Price = 10.0f } }
            };

            // Act
            var result = await _repository.UpdateOrderAsync(updatedOrder);

            // Assert
            Assert.True(result);
            var retrievedOrder2 = await _repository.GetOrderAsync(orderId2);
            Assert.NotNull(retrievedOrder2);
            Assert.Equal(initialOrder2.OrderDate, retrievedOrder2.OrderDate);
            Assert.Equal(initialOrder2.Status, retrievedOrder2.Status);
            Assert.Equal(initialOrder2.Products.Count, retrievedOrder2.Products.Count);
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldHandleEmptyProductList()
        {
            // Arrange
            var orderId = 1;
            var initialOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(initialOrder);

            var updatedOrder = new Order
            {
                OrderId = orderId,
                OrderDate = initialOrder.OrderDate,
                Status = initialOrder.Status,
                Products = new List<Product>()
            };

            // Act
            var result = await _repository.UpdateOrderAsync(updatedOrder);

            // Assert
            Assert.True(result);
            var retrievedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(retrievedOrder);
            Assert.Empty(retrievedOrder.Products);
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldHandleLargeNumberOfProducts()
        {
            // Arrange
            var orderId = 1;
            var initialOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(initialOrder);

            var largeProductList = Enumerable.Range(1, 1000).Select(i => new Product { Id = i, Name = $"Product {i}", Price = i }).ToList();
            var updatedOrder = new Order
            {
                OrderId = orderId,
                OrderDate = initialOrder.OrderDate,
                Status = initialOrder.Status,
                Products = largeProductList
            };

            // Act
            var result = await _repository.UpdateOrderAsync(updatedOrder);

            // Assert
            Assert.True(result);
            var retrievedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(retrievedOrder);
            Assert.Equal(1000, retrievedOrder.Products.Count);
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldHandleNullProducts()
        {
            // Arrange
            var orderId = 1;
            var initialOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(initialOrder);

            var updatedOrder = new Order
            {
                OrderId = orderId,
                OrderDate = default(DateTime),
                Status = default(OrderStatus),
                Products = new List<Product>()
            };

            // Act
            var result = await _repository.UpdateOrderAsync(updatedOrder);

            // Assert
            Assert.True(result);
            var retrievedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(retrievedOrder);
            Assert.Equal(default(DateTime), retrievedOrder.OrderDate);
            Assert.Equal(default(OrderStatus), retrievedOrder.Status);
            Assert.NotNull(retrievedOrder.Products);
            Assert.Empty(retrievedOrder.Products);   
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldHandleEmptyProducts()
        {
            // Arrange
            var orderId = 1; 
            var initialOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(initialOrder);

            var updatedOrder = new Order
            {
                OrderId = orderId,
                OrderDate = default(DateTime),
                Status = default(OrderStatus),
                Products = new List<Product>() 
            };

            // Act
            var result = await _repository.UpdateOrderAsync(updatedOrder);

            // Assert
            Assert.True(result);
            var retrievedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(retrievedOrder);
            Assert.Equal(default(DateTime), retrievedOrder.OrderDate);
            Assert.Equal(default(OrderStatus), retrievedOrder.Status);
            Assert.Empty(retrievedOrder.Products);
        }
    }
}