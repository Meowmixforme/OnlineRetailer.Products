using System;
using System.Linq;
using System.Threading.Tasks;
using ThAmCo.ProductsAPI.Data;
using ThAmCo.ProductsAPI.OrderRepo;
using Xunit;

namespace ThAmCo.Orders.Tests
{
    public class GetOrderAsyncTests
    {
        private readonly FakeOrderRepository _repository;

        public GetOrderAsyncTests()
        {
            _repository = new FakeOrderRepository();
        }

        [Fact]
        public async Task GetOrderAsync_ShouldReturnOrderForValidId()
        {
            // Arrange
            var orderId = 1;

            // Act
            var result = await _repository.GetOrderAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
        }

        [Fact]
        public async Task GetOrderAsync_ShouldReturnNullForInvalidId()
        {
            // Arrange
            var invalidOrderId = 9999;

            // Act
            var result = await _repository.GetOrderAsync(invalidOrderId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrderAsync_ShouldReturnOrderWithCorrectStatus()
        {
            // Arrange
            var orderId = 1; 

            // Act
            var result = await _repository.GetOrderAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(OrderStatus.Delivered, result.Status);
        }

        [Fact]
        public async Task GetOrderAsync_ShouldReturnOrderWithCorrectProducts()
        {
            // Arrange
            var orderId = 1; 

            // Act
            var result = await _repository.GetOrderAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result.Products, p => p.Name == "Hair Brush");
            Assert.Contains(result.Products, p => p.Name == "Cat Treats");
        }

        [Fact]
        public async Task GetOrderAsync_ShouldReturnOrderWithCorrectOrderDate()
        {
            // Arrange
            var orderId = 1;

            // Act
            var result = await _repository.GetOrderAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.OrderDate < DateTime.Now);
            Assert.True(result.OrderDate > DateTime.Now.AddDays(-6));
        }

        [Fact]
        public async Task GetOrderAsync_ShouldReturnDifferentOrdersForDifferentIds()
        {
            // Arrange
            var orderId1 = 1;
            var orderId2 = 2;

            // Act
            var result1 = await _repository.GetOrderAsync(orderId1);
            var result2 = await _repository.GetOrderAsync(orderId2);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotEqual(result1.OrderId, result2.OrderId);
        }

        [Fact]
        public async Task GetOrderAsync_ShouldReturnSameOrderForRepeatedRequests()
        {
            // Arrange
            var orderId = 1;

            // Act
            var result1 = await _repository.GetOrderAsync(orderId);
            var result2 = await _repository.GetOrderAsync(orderId);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.Equal(result1.OrderId, result2.OrderId);
            Assert.Equal(result1.OrderDate, result2.OrderDate);
            Assert.Equal(result1.Status, result2.Status);
            Assert.Equal(result1.Products.Count, result2.Products.Count);
        }

        [Fact]
        public async Task GetOrderAsync_ShouldReturnNullForZeroId()
        {
            // Arrange
            var orderId = 0;

            // Act
            var result = await _repository.GetOrderAsync(orderId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrderAsync_ShouldReturnNullForNegativeId()
        {
            // Arrange
            var orderId = -1;

            // Act
            var result = await _repository.GetOrderAsync(orderId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrderAsync_ShouldReturnOrderAfterCreation()
        {
            // Arrange
            var newOrder = await _repository.CreateOrderAsync();

            // Act
            var result = await _repository.GetOrderAsync(newOrder.OrderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newOrder.OrderId, result.OrderId);
            Assert.Equal(newOrder.OrderDate, result.OrderDate);
            Assert.Equal(newOrder.Status, result.Status);
            Assert.Empty(result.Products);
        }
    }
}