using System;
using System.Linq;
using System.Threading.Tasks;
using ThAmCo.ProductsAPI.Data;
using ThAmCo.ProductsAPI.OrderRepo;
using Xunit;

namespace ThAmCo.Orders.Tests
{
    public class RemoveProductFromOrderAsyncTests
    {
        private readonly FakeOrderRepository _repository;

        public RemoveProductFromOrderAsyncTests()
        {
            _repository = new FakeOrderRepository();
        }

        [Fact]
        public async Task RemoveProductFromOrderAsync_ShouldRemoveExistingProduct()
        {
            // Arrange
            var orderId = 1; 
            var productIdToRemove = 1; 

            // Act
            var result = await _repository.RemoveProductFromOrderAsync(orderId, productIdToRemove);

            // Assert
            Assert.True(result);
            var updatedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(updatedOrder);
            Assert.DoesNotContain(updatedOrder.Products, p => p.Id == productIdToRemove);
        }

        [Fact]
        public async Task RemoveProductFromOrderAsync_ShouldReturnFalseForNonExistentOrder()
        {
            // Arrange
            var nonExistentOrderId = 9999;
            var productId = 1;

            // Act
            var result = await _repository.RemoveProductFromOrderAsync(nonExistentOrderId, productId);

            // Assert
            Assert.False(result);
            var order = await _repository.GetOrderAsync(nonExistentOrderId);
            Assert.Null(order);
        }

        [Fact]
        public async Task RemoveProductFromOrderAsync_ShouldReturnFalseForNonExistentProduct()
        {
            // Arrange
            var orderId = 1; 
            var nonExistentProductId = 9999;

            // Act
            var result = await _repository.RemoveProductFromOrderAsync(orderId, nonExistentProductId);

            // Assert
            Assert.False(result);
            var order = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(order);
            Assert.DoesNotContain(order.Products, p => p.Id == nonExistentProductId);
        }

        [Fact]
        public async Task RemoveProductFromOrderAsync_ShouldNotAffectOtherProducts()
        {
            // Arrange
            var orderId = 1; 
            var productIdToRemove = 1;
            var initialOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(initialOrder);
            var initialProductCount = initialOrder.Products.Count;

            // Act
            var result = await _repository.RemoveProductFromOrderAsync(orderId, productIdToRemove);

            // Assert
            Assert.True(result);
            var updatedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(updatedOrder);
            Assert.Equal(initialProductCount - 1, updatedOrder.Products.Count);
            Assert.All(updatedOrder.Products, p => Assert.NotEqual(productIdToRemove, p.Id));
        }

        [Fact]
        public async Task RemoveProductFromOrderAsync_ShouldHandleRemovingLastProduct()
        {
            // Arrange
            var orderId = 1; 
            var initialOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(initialOrder);
            foreach (var product in initialOrder.Products.ToList())
            {
                await _repository.RemoveProductFromOrderAsync(orderId, product.Id);
            }

            // Act
            var result = await _repository.RemoveProductFromOrderAsync(orderId, 1); 

            // Assert
            Assert.False(result);
            var updatedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(updatedOrder);
            Assert.Empty(updatedOrder.Products);
        }

        [Fact]
        public async Task RemoveProductFromOrderAsync_ShouldNotAffectOtherOrders()
        {
            // Arrange
            var orderId1 = 1;
            var orderId2 = 2;

            var initialOrder1 = await _repository.GetOrderAsync(orderId1);
            var initialOrder2 = await _repository.GetOrderAsync(orderId2);

            Assert.NotNull(initialOrder1);
            Assert.NotNull(initialOrder2);

            // Chooseproduct in first order
            var productToRemove = initialOrder1.Products.First();

            // Act
            var result = await _repository.RemoveProductFromOrderAsync(orderId1, productToRemove.Id);

            // Assert
            Assert.True(result);

            var updatedOrder1 = await _repository.GetOrderAsync(orderId1);
            var updatedOrder2 = await _repository.GetOrderAsync(orderId2);

            Assert.NotNull(updatedOrder1);
            Assert.NotNull(updatedOrder2);

            // Check product was removed from order1
            Assert.DoesNotContain(updatedOrder1.Products, p => p.Id == productToRemove.Id);

            // Check that order2 wasn't affected
            Assert.Equal(initialOrder2.Products.Count, updatedOrder2.Products.Count);
            Assert.Equal(
                initialOrder2.Products.Select(p => p.Id).OrderBy(id => id),
                updatedOrder2.Products.Select(p => p.Id).OrderBy(id => id)
            );
        }

        [Fact]
        public async Task RemoveProductFromOrderAsync_ShouldHandleRemovingNonExistentProductFromExistingOrder()
        {
            // Arrange
            var orderId = 1;
            var nonExistentProductId = 9999;
            var initialOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(initialOrder);

            // Act
            var result = await _repository.RemoveProductFromOrderAsync(orderId, nonExistentProductId);

            // Assert
            Assert.False(result);
            var updatedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(updatedOrder);
            Assert.Equal(initialOrder.Products.Count, updatedOrder.Products.Count);
        }

        [Fact]
        public async Task RemoveProductFromOrderAsync_ShouldHandleRemovingProductFromEmptyOrder()
        {
            // Arrange
            var emptyOrder = await _repository.CreateOrderAsync();
            Assert.NotNull(emptyOrder);
            var productId = 1;

            // Act
            var result = await _repository.RemoveProductFromOrderAsync(emptyOrder.OrderId, productId);

            // Assert
            Assert.False(result);
            var updatedOrder = await _repository.GetOrderAsync(emptyOrder.OrderId);
            Assert.NotNull(updatedOrder);
            Assert.Empty(updatedOrder.Products);
        }

        [Fact]
        public async Task RemoveProductFromOrderAsync_ShouldRemoveOneInstanceOfProduct()
        {
            // Arrange
            var orderId = 1;
            var productIdToAdd = 100;
            var productToAdd = new Product { Id = productIdToAdd, Name = "Test Product", Price = 10.0f };
            await _repository.AddProductToOrderAsync(orderId, productToAdd);
            await _repository.AddProductToOrderAsync(orderId, productToAdd); 

            var initialOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(initialOrder);
            var initialCount = initialOrder.Products.Count(p => p.Id == productIdToAdd);

            // Act
            var result = await _repository.RemoveProductFromOrderAsync(orderId, productIdToAdd);

            // Assert
            Assert.True(result);
            var updatedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(updatedOrder);
            var updatedCount = updatedOrder.Products.Count(p => p.Id == productIdToAdd);
            Assert.Equal(initialCount - 1, updatedCount);
        }

        [Fact]
        public async Task RemoveProductFromOrderAsync_ShouldHandleRemovingFromDeliveredOrder()
        {
            // Arrange
            var orderId = 1; 
            var productIdToRemove = 1; 

            // Act
            var result = await _repository.RemoveProductFromOrderAsync(orderId, productIdToRemove);

            // Assert
            Assert.True(result);
            var updatedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(updatedOrder);
            Assert.DoesNotContain(updatedOrder.Products, p => p.Id == productIdToRemove);
        }
    }
}
