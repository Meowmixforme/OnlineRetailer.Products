using System;
using System.Linq;
using System.Threading.Tasks;
using ThAmCo.ProductsAPI.Data;
using ThAmCo.ProductsAPI.OrderRepo;
using Xunit;

namespace ThAmCo.Orders.Tests
{
    public class AddProductAsyncTests
    {
        private readonly FakeOrderRepository _repository;

        public AddProductAsyncTests()
        {
            _repository = new FakeOrderRepository();
        }

        [Fact]
        public async Task AddProductToOrder_ShouldAddProductSuccessfully()
        {
            // Arrange
            var orderId = 1;
            var newProduct = new Product { Id = 100, Name = "Test Product", Price = 10.0f, Description = "Test Description" };

            // Act
            var result = await _repository.AddProductToOrderAsync(orderId, newProduct);

            // Assert
            Assert.True(result);
            var updatedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(updatedOrder);
            Assert.Contains(updatedOrder.Products, p => p.Id == newProduct.Id);
        }

        [Fact]
        public async Task AddProductToOrder_ShouldReturnFalseForNonExistentOrder()
        {
            // Arrange
            var nonExistentOrderId = 9999;
            var product = new Product { Id = 101, Name = "Test Product", Price = 10.0f, Description = "Test Description" };

            // Act
            var result = await _repository.AddProductToOrderAsync(nonExistentOrderId, product);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddProductToOrder_ShouldAllowAddingMultipleProducts()
        {
            // Arrange
            var orderId = 2;
            var product1 = new Product { Id = 102, Name = "Product 1", Price = 10.0f, Description = "Description 1" };
            var product2 = new Product { Id = 103, Name = "Product 2", Price = 20.0f, Description = "Description 2" };

            // Act
            await _repository.AddProductToOrderAsync(orderId, product1);
            await _repository.AddProductToOrderAsync(orderId, product2);

            // Assert
            var updatedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(updatedOrder);
            Assert.Contains(updatedOrder.Products, p => p.Id == product1.Id);
            Assert.Contains(updatedOrder.Products, p => p.Id == product2.Id);
        }

        [Fact]
        public async Task AddProductToOrder_ShouldAllowAddingDuplicateProducts()
        {
            // Arrange
            var orderId = 3;
            var product = new Product { Id = 104, Name = "Duplicate Product", Price = 10.0f, Description = "Duplicate Description" };

            // Act
            await _repository.AddProductToOrderAsync(orderId, product);
            await _repository.AddProductToOrderAsync(orderId, product);

            // Assert
            var updatedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(updatedOrder);
            Assert.Equal(2, updatedOrder.Products.Count(p => p.Id == product.Id));
        }

        [Fact]
        public async Task AddProductToOrder_ShouldNotAffectOtherOrders()
        {
            // Arrange
            var orderId1 = 1;
            var orderId2 = 2;
            var product = new Product { Id = 105, Name = "Test Product", Price = 10.0f, Description = "Test Description" };

            // Act
            await _repository.AddProductToOrderAsync(orderId1, product);

            // Assert
            var updatedOrder1 = await _repository.GetOrderAsync(orderId1);
            var updatedOrder2 = await _repository.GetOrderAsync(orderId2);
            Assert.NotNull(updatedOrder1);
            Assert.NotNull(updatedOrder2);
            Assert.Contains(updatedOrder1.Products, p => p.Id == product.Id);
            Assert.DoesNotContain(updatedOrder2.Products, p => p.Id == product.Id);
        }

        [Fact]
        public async Task AddProductToOrder_ShouldAddProductWithZeroPrice()
        {
            // Arrange
            var orderId = 1;
            var product = new Product { Id = 106, Name = "Free Product", Price = 0f, Description = "Free Description" };

            // Act
            var result = await _repository.AddProductToOrderAsync(orderId, product);

            // Assert
            Assert.True(result);
            var updatedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(updatedOrder);
            Assert.Contains(updatedOrder.Products, p => p.Id == product.Id && p.Price == 0f);
        }

        [Fact]
        public async Task AddProductToOrder_ShouldAddProductWithNegativePrice()
        {
            // Arrange
            var orderId = 2;
            var product = new Product { Id = 107, Name = "Discount Product", Price = -5.0f, Description = "Discount Description" };

            // Act
            var result = await _repository.AddProductToOrderAsync(orderId, product);

            // Assert
            Assert.True(result);
            var updatedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(updatedOrder);
            Assert.Contains(updatedOrder.Products, p => p.Id == product.Id && p.Price == -5.0f);
        }

        [Fact]
        public async Task AddProductToOrder_ShouldAddProductWithEmptyName()
        {
            // Arrange
            var orderId = 3;
            var product = new Product { Id = 108, Name = string.Empty, Price = 10.0f, Description = "Empty Name Description" };

            // Act
            var result = await _repository.AddProductToOrderAsync(orderId, product);

            // Assert
            Assert.True(result);
            var updatedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(updatedOrder);
            Assert.Contains(updatedOrder.Products, p => p.Id == product.Id && string.IsNullOrEmpty(p.Name));
        }

        [Fact]
        public async Task AddProductToOrder_ShouldAddProductWithEmptyDescription()
        {
            // Arrange
            var orderId = 1;
            var product = new Product { Id = 109, Name = "No Description Product", Price = 10.0f, Description = string.Empty };

            // Act
            var result = await _repository.AddProductToOrderAsync(orderId, product);

            // Assert
            Assert.True(result);
            var updatedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(updatedOrder);
            Assert.Contains(updatedOrder.Products, p => p.Id == product.Id && string.IsNullOrEmpty(p.Description));
        }

        [Fact]
        public async Task AddProductToOrder_ShouldAddProductWithMaxFloatValue()
        {
            // Arrange
            var orderId = 2;
            var product = new Product { Id = 110, Name = "Expensive Product", Price = float.MaxValue, Description = "Max Price Description" };

            // Act
            var result = await _repository.AddProductToOrderAsync(orderId, product);

            // Assert
            Assert.True(result);
            var updatedOrder = await _repository.GetOrderAsync(orderId);
            Assert.NotNull(updatedOrder);
            Assert.Contains(updatedOrder.Products, p => p.Id == product.Id && p.Price == float.MaxValue);
        }
    }
}