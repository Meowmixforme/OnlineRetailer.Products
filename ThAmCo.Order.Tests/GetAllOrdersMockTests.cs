using System;
using System.Linq;
using System.Threading.Tasks;
using ThAmCo.ProductsAPI.Data;
using ThAmCo.ProductsAPI.OrderRepo;
using Xunit;

namespace ThAmCo.Orders.Tests
{
    public class GetAllOrdersAsyncTests
    {
        private readonly FakeOrderRepository _repository;

        public GetAllOrdersAsyncTests()
        {
            _repository = new FakeOrderRepository();
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnAllOrders()
        {
            // Act
            var result = await _repository.GetAllOrdersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnOrdersWithUniqueIds()
        {
            // Act
            var result = await _repository.GetAllOrdersAsync();

            // Assert
            var uniqueIds = result.Select(o => o.OrderId).Distinct();
            Assert.Equal(result.Count(), uniqueIds.Count());
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnOrdersWithCorrectStatuses()
        {
            // Act
            var result = await _repository.GetAllOrdersAsync();

            // Assert
            Assert.Contains(result, o => o.Status == OrderStatus.Delivered);
            Assert.Contains(result, o => o.Status == OrderStatus.Processing);
            Assert.Contains(result, o => o.Status == OrderStatus.Pending);
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnOrdersWithProducts()
        {
            // Act
            var result = await _repository.GetAllOrdersAsync();

            // Assert
            Assert.All(result, order => Assert.NotEmpty(order.Products));
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnOrdersWithValidDates()
        {
            // Act
            var result = await _repository.GetAllOrdersAsync();

            // Assert
            Assert.All(result, order => Assert.True(order.OrderDate <= DateTime.Now));
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnSameResultOnMultipleCalls()
        {
            // Act
            var result1 = await _repository.GetAllOrdersAsync();
            var result2 = await _repository.GetAllOrdersAsync();

            // Assert
            Assert.Equal(result1.Count(), result2.Count());
            Assert.Equal(
                result1.OrderBy(o => o.OrderId).Select(o => o.OrderId),
                result2.OrderBy(o => o.OrderId).Select(o => o.OrderId)
            );
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldIncludeNewlyCreatedOrder()
        {
            // Arrange
            var initialOrders = await _repository.GetAllOrdersAsync();
            var newOrder = await _repository.CreateOrderAsync();

            // Act
            var result = await _repository.GetAllOrdersAsync();

            // Assert
            Assert.Equal(initialOrders.Count() + 1, result.Count());
            Assert.Contains(result, o => o.OrderId == newOrder.OrderId);
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldNotIncludeDeletedOrder()
        {
            // Arrange
            var initialOrders = await _repository.GetAllOrdersAsync();
            var orderToDelete = initialOrders.First();
            await _repository.DeleteOrderAsync(orderToDelete.OrderId);

            // Act
            var result = await _repository.GetAllOrdersAsync();

            // Assert
            Assert.Equal(initialOrders.Count() - 1, result.Count());
            Assert.DoesNotContain(result, o => o.OrderId == orderToDelete.OrderId);
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldReflectUpdatedOrder()
        {
            // Arrange
            var initialOrders = await _repository.GetAllOrdersAsync();
            var orderToUpdate = initialOrders.First();
            orderToUpdate.Status = OrderStatus.Delivered;
            await _repository.UpdateOrderAsync(orderToUpdate);

            // Act
            var result = await _repository.GetAllOrdersAsync();

            // Assert
            var updatedOrder = result.First(o => o.OrderId == orderToUpdate.OrderId);
            Assert.Equal(OrderStatus.Delivered, updatedOrder.Status);
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnEmptyListWhenAllOrdersDeleted()
        {
            // Arrange
            var initialOrders = await _repository.GetAllOrdersAsync();
            foreach (var order in initialOrders)
            {
                await _repository.DeleteOrderAsync(order.OrderId);
            }

            // Act
            var result = await _repository.GetAllOrdersAsync();

            // Assert
            Assert.Empty(result);
        }
    }
}
