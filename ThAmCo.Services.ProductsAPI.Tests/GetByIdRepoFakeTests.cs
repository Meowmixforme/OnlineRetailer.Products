using Microsoft.EntityFrameworkCore;
using ThAmCo.ProductsAPI.Data;
using ThAmCo.ProductsAPI.ProductsRepo;
using Xunit;

namespace ThAmCo.Products.Tests
{
    public class GetProductByIdAsyncTests
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
        public async Task GetProductByIdAsync_ShouldReturnCorrectProduct()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var expectedProduct = seedData[0]; // index of product in list

            // Act
            var result = await repo.GetProductByIdAsync(expectedProduct.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProduct.Id, result.Id);
            Assert.Equal(expectedProduct.Name, result.Name);
            Assert.Equal(expectedProduct.Price, result.Price);
            Assert.Equal(expectedProduct.Description, result.Description);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnNullForNonexistentId()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var nonexistentId = seedData.Max(p => p.Id) + 1;

            // Act
            var result = await repo.GetProductByIdAsync(nonexistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldHandleMinimumId()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var minId = seedData.Min(p => p.Id);

            // Act
            var result = await repo.GetProductByIdAsync(minId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(minId, result.Id);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldHandleMaximumId()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var maxId = seedData.Max(p => p.Id);

            // Act
            var result = await repo.GetProductByIdAsync(maxId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(maxId, result.Id);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnNullForNegativeId()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);

            // Act
            var result = await repo.GetProductByIdAsync(-1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldHandleEmptyDatabase()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ProductsContext(options);
            var repo = new ProductsRepo(context);

            // Act
            var result = await repo.GetProductByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldHandleLargeId()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ProductsContext(options);
            var largeId = int.MaxValue;
            var product = new Product { Id = largeId, Name = "Large ID Product", Price = 9.99f, Description = "Test" };
            context.Products.Add(product);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);

            // Act
            var result = await repo.GetProductByIdAsync(largeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(largeId, result.Id);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnCorrectProductAfterUpdate()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var productToUpdate = seedData[0];

            // Update product
            productToUpdate.Name = "Updated Name";
            productToUpdate.Price = 99.99f;
            productToUpdate.Description = "Updated Description";
            context.Products.Update(productToUpdate);
            await context.SaveChangesAsync();

            // Act
            var result = await repo.GetProductByIdAsync(productToUpdate.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productToUpdate.Id, result.Id);
            Assert.Equal("Updated Name", result.Name);
            Assert.Equal(99.99f, result.Price);
            Assert.Equal("Updated Description", result.Description);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldHandleManyRequests()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);

            // Act
            var tasks = seedData.Select(p => repo.GetProductByIdAsync(p.Id)).ToList();
            var results = await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(seedData.Count, results.Length);
            for (int i = 0; i < seedData.Count; i++)
            {
                Assert.NotNull(results[i]);
                var result = results[i]!;
                Assert.Equal(seedData[i].Id, result.Id);
                Assert.Equal(seedData[i].Name, result.Name);
                Assert.Equal(seedData[i].Price, result.Price);
                Assert.Equal(seedData[i].Description, result.Description);
            }
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnNullForDeletedProduct()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var productToDelete = seedData[0];

            // Delete product
            context.Products.Remove(productToDelete);
            await context.SaveChangesAsync();

            // Act
            var result = await repo.GetProductByIdAsync(productToDelete.Id);

            // Assert
            Assert.Null(result);
        }
    }
}