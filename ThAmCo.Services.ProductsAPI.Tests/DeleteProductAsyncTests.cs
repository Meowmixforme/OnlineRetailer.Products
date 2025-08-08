using Microsoft.EntityFrameworkCore;
using ThAmCo.ProductsAPI.Data;
using ThAmCo.ProductsAPI.ProductsRepo;
using Xunit;

namespace ThAmCo.Products.Tests
{
    public class DeleteProductAsyncTests
    {
        private DbContextOptions<ProductsContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<ProductsContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private List<Product> GetSeedData()
        {
            var fakeRepo = new ProductRepoFake();
            return fakeRepo.GetSeedData();
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldRemoveProductFromDatabase()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var productToDelete = seedData[0]; // index of product list

            // Act
            await repo.DeleteProductAsync(productToDelete.Id);

            // Assert
            var deletedProduct = await context.Products.FindAsync(productToDelete.Id);
            Assert.Null(deletedProduct);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldNotThrowExceptionForNonexistentId()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ProductsContext(options);
            var repo = new ProductsRepo(context);
            var nonexistentId = 999;

            // Act & Assert
            await repo.DeleteProductAsync(nonexistentId);
            var products = await context.Products.ToListAsync();
            Assert.Empty(products);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldDecreaseProductCount()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var initialCount = await context.Products.CountAsync();

            // Act
            await repo.DeleteProductAsync(seedData[0].Id);

            // Assert
            var newCount = await context.Products.CountAsync();
            Assert.Equal(initialCount - 1, newCount);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldHandleMultipleDeletes()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);

            // Act
            foreach (var product in seedData)
            {
                await repo.DeleteProductAsync(product.Id);
            }

            // Assert
            var remainingProducts = await context.Products.ToListAsync();
            Assert.Empty(remainingProducts);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldNotAffectOtherProducts()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var productToDelete = seedData[0];

            // Act
            await repo.DeleteProductAsync(productToDelete.Id);

            // Assert
            var remainingProducts = await context.Products.ToListAsync();
            Assert.Equal(seedData.Count - 1, remainingProducts.Count);
            Assert.DoesNotContain(remainingProducts, p => p.Id == productToDelete.Id);
            Assert.All(remainingProducts, p => Assert.Contains(seedData, sp => sp.Id == p.Id));
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldHandleEmptyDatabase()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ProductsContext(options);
            var repo = new ProductsRepo(context);

            // Act
            await repo.DeleteProductAsync(1);

            // Assert
            var products = await context.Products.ToListAsync();
            Assert.Empty(products);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldHandleManyDeletes()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);

            // Act
            var deleteTasks = seedData.Select(p => repo.DeleteProductAsync(p.Id)).ToList();
            await Task.WhenAll(deleteTasks);

            // Assert
            var remainingProducts = await context.Products.ToListAsync();
            Assert.Empty(remainingProducts);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldHandleRepeatedDeleteRequests()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var productToDelete = seedData[0];

            // Act
            await repo.DeleteProductAsync(productToDelete.Id);
            await repo.DeleteProductAsync(productToDelete.Id);
            await repo.DeleteProductAsync(productToDelete.Id);

            // Assert
            var deletedProduct = await context.Products.FindAsync(productToDelete.Id);
            Assert.Null(deletedProduct);
            var remainingProducts = await context.Products.ToListAsync();
            Assert.Equal(seedData.Count - 1, remainingProducts.Count);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldHandleLargeNumberOfProducts()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ProductsContext(options);
            var largeNumberOfProducts = Enumerable.Range(1, 10000)
                .Select(i => new Product { Id = i, Name = $"Product {i}", Price = i * 1.5f, Description = $"Description {i}" })
                .ToList();
            context.Products.AddRange(largeNumberOfProducts);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);

            // Act
            foreach (var product in largeNumberOfProducts)
            {
                await repo.DeleteProductAsync(product.Id);
            }

            // Assert
            var remainingProducts = await context.Products.ToListAsync();
            Assert.Empty(remainingProducts);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldNotAffectDeletedProduct()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var productToDelete = seedData[0];

            // Act
            await repo.DeleteProductAsync(productToDelete.Id);
            var deletedProduct = await context.Products.FindAsync(productToDelete.Id);
            Assert.Null(deletedProduct);

            // Attempt to delete same product again
            await repo.DeleteProductAsync(productToDelete.Id);

            // Assert
            var remainingProducts = await context.Products.ToListAsync();
            Assert.Equal(seedData.Count - 1, remainingProducts.Count);
            Assert.DoesNotContain(remainingProducts, p => p.Id == productToDelete.Id);
        }
    }
}