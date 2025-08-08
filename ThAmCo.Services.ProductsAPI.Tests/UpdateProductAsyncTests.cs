using Microsoft.EntityFrameworkCore;
using ThAmCo.ProductsAPI.Data;
using ThAmCo.ProductsAPI.ProductsRepo;
using Xunit;

namespace ThAmCo.Products.Tests
{
    public class UpdateProductAsyncTests
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
        public async Task UpdateProductAsync_ShouldUpdateExistingProduct()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var productToUpdate = seedData[12];
            productToUpdate.Name = "Updated Name";
            productToUpdate.Price = 99.99f;
            productToUpdate.Description = "Updated Description";

            // Act
            await repo.UpdateProductAsync(productToUpdate);

            // Assert
            var updatedProduct = await context.Products.FindAsync(productToUpdate.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Name", updatedProduct.Name);
            Assert.Equal(99.99f, updatedProduct.Price);
            Assert.Equal("Updated Description", updatedProduct.Description);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldNotAffectOtherProducts()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var productToUpdate = seedData[0];
            productToUpdate.Name = "Updated Name";

            // Act
            await repo.UpdateProductAsync(productToUpdate);

            // Assert
            var otherProducts = await context.Products.Where(p => p.Id != productToUpdate.Id).ToListAsync();
            Assert.All(otherProducts, p => Assert.Equal(seedData.First(sp => sp.Id == p.Id).Name, p.Name));
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldHandleNonExistentProduct()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ProductsContext(options);
            var repo = new ProductsRepo(context);
            var nonExistentProduct = new Product { Id = 999, Name = "Non-existent", Price = 10.0f, Description = "Test" };

            // Act & Assert
            await Assert.ThrowsAnyAsync<Exception>(() => repo.UpdateProductAsync(nonExistentProduct));
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldUpdateAllFields()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var productToUpdate = seedData[0];
            productToUpdate.Name = "New Name";
            productToUpdate.Price = 123.45f;
            productToUpdate.Description = "New Description";

            // Act
            await repo.UpdateProductAsync(productToUpdate);

            // Assert
            var updatedProduct = await context.Products.FindAsync(productToUpdate.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal("New Name", updatedProduct.Name);
            Assert.Equal(123.45f, updatedProduct.Price);
            Assert.Equal("New Description", updatedProduct.Description);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldHandleEmptyFields()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var productToUpdate = seedData[0];
            productToUpdate.Name = "";
            productToUpdate.Description = "";

            // Act
            await repo.UpdateProductAsync(productToUpdate);

            // Assert
            var updatedProduct = await context.Products.FindAsync(productToUpdate.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal("", updatedProduct.Name);
            Assert.Equal("", updatedProduct.Description);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldHandleLargeValues()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var productToUpdate = seedData[0];
            productToUpdate.Name = new string('A', 255);
            productToUpdate.Price = float.MaxValue;
            productToUpdate.Description = new string('B', 1000); 

            // Act
            await repo.UpdateProductAsync(productToUpdate);

            // Assert
            var updatedProduct = await context.Products.FindAsync(productToUpdate.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal(new string('A', 255), updatedProduct.Name);
            Assert.Equal(float.MaxValue, updatedProduct.Price);
            Assert.Equal(new string('B', 1000), updatedProduct.Description);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldHandleMultipleUpdates()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var productToUpdate = seedData[0];

            // Act
            productToUpdate.Name = "First Update";
            await repo.UpdateProductAsync(productToUpdate);
            productToUpdate.Name = "Second Update";
            await repo.UpdateProductAsync(productToUpdate);
            productToUpdate.Name = "Third Update";
            await repo.UpdateProductAsync(productToUpdate);

            // Assert
            var updatedProduct = await context.Products.FindAsync(productToUpdate.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Third Update", updatedProduct.Name);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldHandleManyUpdates()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);

            // Act
            var updateTasks = seedData.Select(p =>
            {
                p.Name += " Updated";
                return repo.UpdateProductAsync(p);
            }).ToList();

            await Task.WhenAll(updateTasks);

            // Assert
            var updatedProducts = await context.Products.ToListAsync();
            Assert.All(updatedProducts, p => Assert.EndsWith(" Updated", p.Name));
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldHandleUpdateWithNoChanges()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var productToUpdate = seedData[0];

            // Act
            await repo.UpdateProductAsync(productToUpdate);

            // Assert
            var updatedProduct = await context.Products.FindAsync(productToUpdate.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal(productToUpdate.Name, updatedProduct.Name);
            Assert.Equal(productToUpdate.Price, updatedProduct.Price);
            Assert.Equal(productToUpdate.Description, updatedProduct.Description);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldHandleUpdateAfterDelete()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using var context = new ProductsContext(options);
            context.Products.AddRange(seedData);
            await context.SaveChangesAsync();
            var repo = new ProductsRepo(context);
            var productToUpdate = seedData[0];

            // Delete the product
            await repo.DeleteProductAsync(productToUpdate.Id);

            // Act & Assert
            productToUpdate.Name = "Updated After Delete";
            await Assert.ThrowsAnyAsync<Exception>(() => repo.UpdateProductAsync(productToUpdate));
        }
    }
}