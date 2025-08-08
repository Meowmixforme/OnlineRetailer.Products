using System;
using System.Linq;
using System.Threading.Tasks;
using ThAmCo.ProductsAPI.Data;
using ThAmCo.ProductsAPI.OrderRepo;
using Xunit;

namespace ThAmCo.Orders.Tests
{
    public class CreateOrderAsyncTests
    {
        private readonly FakeOrderRepository _repository;

        public CreateOrderAsyncTests()
        {
            _repository = new FakeOrderRepository();
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldReturnNewOrder()
        {
            // Act
            var result = await _repository.CreateOrderAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.OrderId > 0);
            Assert.Equal(OrderStatus.Pending, result.Status);
            Assert.Empty(result.Products);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldIncrementOrderId()
        {
            // Act
            var order1 = await _repository.CreateOrderAsync();
            var order2 = await _repository.CreateOrderAsync();

            // Assert
            Assert.NotEqual(order1.OrderId, order2.OrderId);
            Assert.True(order2.OrderId > order1.OrderId);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldSetCurrentDateTime()
        {
            // Act
            var result = await _repository.CreateOrderAsync();

            // Assert
            Assert.True((DateTime.Now - result.OrderDate).TotalSeconds < 1);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldAddOrderToRepository()
        {
            // Act
            var newOrder = await _repository.CreateOrderAsync();
            var retrievedOrder = await _repository.GetOrderAsync(newOrder.OrderId);

            // Assert
            Assert.NotNull(retrievedOrder);
            Assert.Equal(newOrder.OrderId, retrievedOrder.OrderId);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldCreateMultipleOrders()
        {
            // Act
            var order1 = await _repository.CreateOrderAsync();
            var order2 = await _repository.CreateOrderAsync();
            var order3 = await _repository.CreateOrderAsync();

            // Assert
            Assert.NotEqual(order1.OrderId, order2.OrderId);
            Assert.NotEqual(order2.OrderId, order3.OrderId);
            Assert.NotEqual(order1.OrderId, order3.OrderId);
        }


        [Fact]
        public async Task CreateOrderAsync_ShouldCreateOrderWithEmptyProductList()
        {
            // Act
            var result = await _repository.CreateOrderAsync();

            // Assert
            Assert.NotNull(result.Products);
            Assert.Empty(result.Products);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldCreateOrderWithPendingStatus()
        {
            // Act
            var result = await _repository.CreateOrderAsync();

            // Assert
            Assert.Equal(OrderStatus.Pending, result.Status);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldCreateUniqueOrderIds()
        {
            // Arrange
            var orderIds = new HashSet<int>();

            // Act
            for (int i = 0; i < 100; i++)
            {
                var order = await _repository.CreateOrderAsync();
                orderIds.Add(order.OrderId);
            }

            // Assert
            Assert.Equal(100, orderIds.Count);
        }

    }
}