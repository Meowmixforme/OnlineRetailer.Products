using Microsoft.EntityFrameworkCore;
using ThAmCo.ProductsAPI.Data;
using ThAmCo.ProductsAPI.ProductsRepo;
using Xunit;

namespace ThAmCo.Products.Tests
{
    public class GetProductsAsyncTests
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
        public async Task GetProductsAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using (var context = new ProductsContext(options))
            {
                context.Products.AddRange(seedData);
                await context.SaveChangesAsync();
            }

            using (var context = new ProductsContext(options))
            {
                var repo = new ProductsRepo(context);

                // Act
                var result = await repo.GetProductsAsync();

                // Assert
                Assert.Equal(seedData.Count, result.Count());
                Assert.Equal(seedData.Select(p => p.Id).OrderBy(id => id),
                             result.Select(p => p.Id).OrderBy(id => id));
            }
        }

        [Fact]
        public async Task GetProductsAsync_ShouldReturnEmptyListIfNoProducts()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ProductsContext(options);
            var repo = new ProductsRepo(context);

            // Act
            var result = await repo.GetProductsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProductsAsync_ShouldReturnCorrectProductDetails()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData().Take(1).ToList();
            using (var context = new ProductsContext(options))
            {
                context.Products.AddRange(seedData);
                await context.SaveChangesAsync();
            }

            using (var context = new ProductsContext(options))
            {
                var repo = new ProductsRepo(context);

                // Act
                var result = await repo.GetProductsAsync();

                // Assert
                var product = result.First();
                Assert.Equal(seedData[0].Id, product.Id);
                Assert.Equal(seedData[0].Name, product.Name);
                Assert.Equal(seedData[0].Price, product.Price);
                Assert.Equal(seedData[0].Description, product.Description);
            }
        }

        [Fact]
        public async Task GetProductsAsync_ShouldHandleLargeNumberOfProducts()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var largeNumberOfProducts = Enumerable.Range(1, 10000)
                .Select(i => new Product { Id = i, Name = $"Product {i}", Price = i * 1.5f, Description = $"Description {i}" })
                .ToList();

            using (var context = new ProductsContext(options))
            {
                context.Products.AddRange(largeNumberOfProducts);
                await context.SaveChangesAsync();
            }

            using (var context = new ProductsContext(options))
            {
                var repo = new ProductsRepo(context);

                // Act
                var result = await repo.GetProductsAsync();

                // Assert
                Assert.Equal(largeNumberOfProducts.Count, result.Count());
            }
        }

        [Fact]
        public async Task GetProductsAsync_ShouldReturnProductsInTheCorrectOrder()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using (var context = new ProductsContext(options))
            {
                context.Products.AddRange(seedData);
                await context.SaveChangesAsync();
            }

            using (var context = new ProductsContext(options))
            {
                var repo = new ProductsRepo(context);

                // Act
                var result = await repo.GetProductsAsync();

                // Assert
                Assert.Equal(seedData.OrderBy(p => p.Id).Select(p => p.Id),
                             result.Select(p => p.Id));
            }
        }

        [Fact]
        public async Task GetProductsAsync_ShouldHandleProductsWithSameNames()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var productsWithSameNames = new List<Product>
            {
                new Product { Id = 1, Name = "Same Name", Price = 10.0f, Description = "Description 1" },
                new Product { Id = 2, Name = "Same Name", Price = 20.0f, Description = "Description 2" },
                new Product { Id = 3, Name = "Same Name", Price = 30.0f, Description = "Description 3" }
            };

            using (var context = new ProductsContext(options))
            {
                context.Products.AddRange(productsWithSameNames);
                await context.SaveChangesAsync();
            }

            using (var context = new ProductsContext(options))
            {
                var repo = new ProductsRepo(context);

                // Act
                var result = await repo.GetProductsAsync();

                // Assert
                Assert.Equal(productsWithSameNames.Count, result.Count());
                Assert.Equal(productsWithSameNames.Select(p => p.Id).OrderBy(id => id),
                             result.Select(p => p.Id).OrderBy(id => id));
            }
        }

        [Fact]
        public async Task GetProductsAsync_ShouldHandleProductsWithExtremeValues()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var extremeProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Min Price", Price = 0.01f, Description = "Minimum price" },
                new Product { Id = 2, Name = "Max Price", Price = float.MaxValue, Description = "Maximum price" },
                new Product { Id = 3, Name = new string('A', 255), Price = 10.0f, Description = new string('B', 1000) }
            };

            using (var context = new ProductsContext(options))
            {
                context.Products.AddRange(extremeProducts);
                await context.SaveChangesAsync();
            }

            using (var context = new ProductsContext(options))
            {
                var repo = new ProductsRepo(context);

                // Act
                var result = await repo.GetProductsAsync();

                // Assert
                Assert.Equal(extremeProducts.Count, result.Count());
                foreach (var expected in extremeProducts)
                {
                    var actual = result.FirstOrDefault(p => p.Id == expected.Id);
                    Assert.NotNull(actual);
                    Assert.Equal(expected.Name, actual.Name);
                    Assert.Equal(expected.Price, actual.Price);
                    Assert.Equal(expected.Description, actual.Description);
                }
            }
        }

        [Fact]
        public async Task GetProductsAsync_ShouldHandleManyReads()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using (var context = new ProductsContext(options))
            {
                context.Products.AddRange(seedData);
                await context.SaveChangesAsync();
            }

            using (var context = new ProductsContext(options))
            {
                var repo = new ProductsRepo(context);

                // Act
                var tasks = Enumerable.Range(0, 10).Select(_ => repo.GetProductsAsync()).ToList();
                var results = await Task.WhenAll(tasks);

                // Assert
                Assert.All(results, result =>
                {
                    Assert.Equal(seedData.Count, result.Count());
                    Assert.Equal(seedData.Select(p => p.Id).OrderBy(id => id),
                                 result.Select(p => p.Id).OrderBy(id => id));
                });
            }
        }

        [Fact]
        public async Task GetProductsAsync_ShouldNotReturnDeletedProducts()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using (var context = new ProductsContext(options))
            {
                context.Products.AddRange(seedData);
                await context.SaveChangesAsync();
            }

            using (var context = new ProductsContext(options))
            {
                var repo = new ProductsRepo(context);

                // Delete a product
                var productToDelete = seedData.First();
                context.Products.Remove(productToDelete);
                await context.SaveChangesAsync();

                // Act
                var result = await repo.GetProductsAsync();

                // Assert
                Assert.Equal(seedData.Count - 1, result.Count());
                Assert.DoesNotContain(result, p => p.Id == productToDelete.Id);
            }
        }

        [Fact]
        public async Task GetProductsAsync_ShouldReturnUpdatedProducts()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var seedData = GetSeedData();
            using (var context = new ProductsContext(options))
            {
                context.Products.AddRange(seedData);
                await context.SaveChangesAsync();
            }

            using (var context = new ProductsContext(options))
            {
                var repo = new ProductsRepo(context);

                // Update a product
                var productToUpdate = seedData.First();
                productToUpdate.Name = "Updated Name";
                productToUpdate.Price = 999.99f;
                productToUpdate.Description = "Updated Description";
                context.Products.Update(productToUpdate);
                await context.SaveChangesAsync();

                // Act
                var result = await repo.GetProductsAsync();

                // Assert
                var updatedProduct = result.FirstOrDefault(p => p.Id == productToUpdate.Id);
                Assert.NotNull(updatedProduct);
                Assert.Equal("Updated Name", updatedProduct.Name);
                Assert.Equal(999.99f, updatedProduct.Price);
                Assert.Equal("Updated Description", updatedProduct.Description);
            }
        }
    }
}