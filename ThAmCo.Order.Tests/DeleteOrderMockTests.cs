using System;
using System.Linq;
using System.Threading.Tasks;
using ThAmCo.ProductsAPI.Data;
using ThAmCo.ProductsAPI.OrderRepo;
using Xunit;

namespace ThAmCo.Orders.Tests
{
    public class DeleteOrderAsyncTests
    {
        private readonly FakeOrderRepository _repository;

        public DeleteOrderAsyncTests()
        {
            _repository = new FakeOrderRepository();
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldDeleteExistingOrder()
        {
            // Arrange
            var orderId = 1;

            // Act
            var result = await _repository.DeleteOrderAsync(orderId);

            // Assert
            Assert.True(result);
            var deletedOrder = await _repository.GetOrderAsync(orderId);
            Assert.Null(deletedOrder);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldReturnFalseForNonExistentOrder()
        {
            // Arrange
            var nonExistentOrderId = 9999;

            // Act
            var result = await _repository.DeleteOrderAsync(nonExistentOrderId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldNotAffectOtherOrders()
        {
            // Arrange
            var orderIdToDelete = 1;
            var orderIdToKeep = 2;
            var initialOrderCount = (await _repository.GetAllOrdersAsync()).Count();

            // Act
            await _repository.DeleteOrderAsync(orderIdToDelete);

            // Assert
            var remainingOrders = await _repository.GetAllOrdersAsync();
            Assert.Equal(initialOrderCount - 1, remainingOrders.Count());
            Assert.Contains(remainingOrders, o => o.OrderId == orderIdToKeep);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldHandleDeletingAlreadyDeletedOrder()
        {
            // Arrange
            var orderId = 1;
            await _repository.DeleteOrderAsync(orderId);

            // Act
            var result = await _repository.DeleteOrderAsync(orderId);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public async Task DeleteOrderAsync_ShouldHandleNegativeOrderId()
        {
            // Arrange
            var negativeOrderId = -1;

            // Act
            var result = await _repository.DeleteOrderAsync(negativeOrderId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldHandleZeroOrderId()
        {
            // Arrange
            var zeroOrderId = 0;

            // Act
            var result = await _repository.DeleteOrderAsync(zeroOrderId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldHandleMaxIntOrderId()
        {
            // Arrange
            var maxIntOrderId = int.MaxValue;

            // Act
            var result = await _repository.DeleteOrderAsync(maxIntOrderId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldNotChangeNextOrderIdAfterDeletion()
        {
            // Arrange
            var initialOrderCount = (await _repository.GetAllOrdersAsync()).Count();
            await _repository.DeleteOrderAsync(initialOrderCount);

            // Act
            var newOrder = await _repository.CreateOrderAsync();

            // Assert
            Assert.Equal(initialOrderCount + 1, newOrder.OrderId);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldHandleManyDeletes()
        {
            // Arrange
            var orderId = 1;

            // Act
            var task1 = _repository.DeleteOrderAsync(orderId);
            var task2 = _repository.DeleteOrderAsync(orderId);

            await Task.WhenAll(task1, task2);

            // Assert
            var results = new[] { await task1, await task2 };
            Assert.Contains(true, results); 
            Assert.Contains(false, results); 
            var deletedOrder = await _repository.GetOrderAsync(orderId);
            Assert.Null(deletedOrder);
        }
    }
}