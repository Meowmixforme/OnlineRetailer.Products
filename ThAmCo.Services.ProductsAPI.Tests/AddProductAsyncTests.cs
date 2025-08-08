using Microsoft.EntityFrameworkCore;
using ThAmCo.ProductsAPI.Data;
using ThAmCo.ProductsAPI.ProductsRepo;
using Xunit;

namespace ThAmCo.Products.Tests
{
    public class ProductRepoTests
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
        public async Task AddProductAsync_ShouldAddNewProduct()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ProductsContext(options);
            var repo = new ProductsRepo(context);
            var newProduct = new Product { Name = "This Is A Test Product", Price = 10.99f, Description = "This Is A Test Description" };

            // Act
            var result = await repo.AddProductAsync(newProduct);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.Id);
            Assert.Equal(newProduct.Name, result.Name);
            Assert.Equal(newProduct.Price, result.Price);
            Assert.Equal(newProduct.Description, result.Description);
        }

        [Fact]
        public async Task AddProductAsync_ShouldIncrementProductCount()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ProductsContext(options);
            var repo = new ProductsRepo(context);
            var initialProducts = GetSeedData();
            context.Products.AddRange(initialProducts);
            await context.SaveChangesAsync();
            var newProduct = new Product { Name = "A New Product", Price = 15.99f, Description = "A New Description" };

            // Act
            await repo.AddProductAsync(newProduct);

            // Assert
            Assert.Equal(initialProducts.Count + 1, await context.Products.CountAsync());
        }

        [Fact]
        public async Task AddProductAsync_ShouldAssignUniqueId()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ProductsContext(options);
            var repo = new ProductsRepo(context);
            var initialProducts = GetSeedData();
            context.Products.AddRange(initialProducts);
            await context.SaveChangesAsync();
            var newProduct = new Product { Name = "Unique Product", Price = 20.99f, Description = "Unique Description" };

            // Act
            var result = await repo.AddProductAsync(newProduct);

            // Assert
            Assert.NotEqual(0, result.Id);
            Assert.DoesNotContain(initialProducts, p => p.Id == result.Id);
        }

        [Fact]
        public async Task AddProductAsync_ShouldSaveData()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ProductsContext(options);
            var repo = new ProductsRepo(context);
            var newProduct = new Product { Name = "Saved Product", Price = 25.99f, Description = "Saved Description" };

            // Act
            var addedProduct = await repo.AddProductAsync(newProduct);

            // Assert
            using var newContext = new ProductsContext(options);
            var persistedProduct = await newContext.Products.FindAsync(addedProduct.Id);
            Assert.NotNull(persistedProduct);
            Assert.Equal(newProduct.Name, persistedProduct.Name);
            Assert.Equal(newProduct.Price, persistedProduct.Price);
            Assert.Equal(newProduct.Description, persistedProduct.Description);
        }

        [Fact]
        public async Task AddProductAsync_ShouldHandleMultipleAdditions()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ProductsContext(options);
            var repo = new ProductsRepo(context);
            var products = new List<Product>
            {
                new Product { Name = "Top Hat", Price = 10.99f, Description = "Perfect for the dapper man" },
                new Product { Name = "Tails", Price = 20.99f, Description = "Turn heads with this product" },
                new Product { Name = "Cane", Price = 30.99f, Description = "Don't point it at anyone" }
            };

            // Act
            foreach (var product in products)
            {
                await repo.AddProductAsync(product);
            }

            // Assert
            Assert.Equal(products.Count, await context.Products.CountAsync());
        }

        [Fact]
        public async Task AddProductAsync_ShouldHandleLargeNumberOfProducts()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ProductsContext(options);
            var repo = new ProductsRepo(context);
            var largeNumberOfProducts = Enumerable.Range(1, 1000)
                .Select(i => new Product { Name = $"Product {i}", Price = i * 1.99f, Description = $"Description {i}" })
                .ToList();

            // Act
            foreach (var product in largeNumberOfProducts)
            {
                await repo.AddProductAsync(product);
            }

            // Assert
            Assert.Equal(largeNumberOfProducts.Count, await context.Products.CountAsync());
        }

        [Fact]
        public async Task AddProductAsync_ShouldHandleProductWithMaxValues()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ProductsContext(options);
            var repo = new ProductsRepo(context);
            var maxProduct = new Product
            {
                Name = new string('A', 255), 
                Price = float.MaxValue,
                Description = new string('B', 1000)
            };

            // Act
            var result = await repo.AddProductAsync(maxProduct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(maxProduct.Name, result.Name);
            Assert.Equal(maxProduct.Price, result.Price);
            Assert.Equal(maxProduct.Description, result.Description);
        }

        [Fact]
        public async Task AddProductAsync_ShouldHandleProductWithMinValues()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ProductsContext(options);
            var repo = new ProductsRepo(context);
            var minProduct = new Product
            {
                Name = "",
                Price = 0.01f,
                Description = ""
            };

            // Act
            var result = await repo.AddProductAsync(minProduct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(minProduct.Name, result.Name);
            Assert.Equal(minProduct.Price, result.Price);
            Assert.Equal(minProduct.Description, result.Description);
        }

        [Fact]
        public async Task AddProductAsync_ShouldHandleManyAdditions()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new ProductsContext(options);
            var repo = new ProductsRepo(context);
            var products = Enumerable.Range(1, 100)
                .Select(i => new Product { Name = $"One OF Many Product {i}", Price = i * 1.5f, Description = $"One Of Many Descriptions {i}" })
                .ToList();

            // Act
            var tasks = products.Select(p => repo.AddProductAsync(p));
            await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(products.Count, await context.Products.CountAsync());
        }
    }
}