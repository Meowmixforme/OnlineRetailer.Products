using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThAmCo.ProductsAPI.Data;

namespace ThAmCo.ProductsAPI.ProductsRepo
{
    public class ProductRepoFake : IProductsRepo
    {
        private readonly List<Product> _products;

        public ProductRepoFake(List<Product>? initialProducts = null)
        {
            _products = initialProducts ?? new List<Product>
    {
        new Product { Id = 1, Name = "Hair Brush", Price = 3.00f, Description = "A fashionable product for combing your hair" },
        new Product { Id = 2, Name = "Cat Treats", Price = 6.50f, Description = "Five paws up for the best treats" },
        new Product { Id = 3, Name = "Headphones", Price = 56.99f, Description = "Award winning headphones" },
        new Product { Id = 4, Name = "Notebook", Price = 2.50f, Description = "A handy notebook for your thoughts" },
        new Product { Id = 5, Name = "Travel Mug", Price = 15.00f, Description = "Keeps your drinks hot or cold" },
        new Product { Id = 6, Name = "Bluetooth Speaker", Price = 29.99f, Description = "Portable sound for any occasion" },
        new Product { Id = 7, Name = "Yoga Mat", Price = 25.00f, Description = "Comfortable mat for your yoga practice" },
        new Product { Id = 8, Name = "Water Bottle", Price = 10.00f, Description = "Stay hydrated on the go" },
        new Product { Id = 9, Name = "Desk Lamp", Price = 18.00f, Description = "A stylish lamp for your workspace" },
        new Product { Id = 10, Name = "Sunglasses", Price = 50.00f, Description = "Protect your eyes in style" },
        new Product { Id = 11, Name = "Fitness Tracker", Price = 70.00f, Description = "Track your health and fitness goals" },
        new Product { Id = 12, Name = "Instant Pot", Price = 80.00f, Description = "Versatile cooker for quick meals" },
        new Product { Id = 13, Name = "Running Shoes", Price = 120.00f, Description = "Comfortable shoes for long runs" },
        new Product { Id = 14, Name = "Electric Toothbrush", Price = 40.00f, Description = "Keep your teeth clean and healthy" },
        new Product { Id = 15, Name = "Wireless Charger", Price = 25.00f, Description = "Conveniently charge your devices" },
        new Product { Id = 16, Name = "Portable Power Bank", Price = 20.00f, Description = "Charge your devices on the go" },
        new Product { Id = 17, Name = "Smart Thermostat", Price = 150.00f, Description = "Control your home temperature remotely" },
        new Product { Id = 18, Name = "Gaming Headset", Price = 60.00f, Description = "Enhance your gaming experience" },
        new Product { Id = 19, Name = "Kitchen Scale", Price = 15.00f, Description = "Accurate measurements for your recipes" },
        new Product { Id = 20, Name = "Electric Kettle", Price = 30.00f, Description = "Boil water quickly and efficiently" }
            };
        }

        public List<Product> GetSeedData() => _products;

        public Task<Product?> GetProductByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        public Task<IEnumerable<Product>> GetProductsAsync()
        {
            var Products = _products.AsEnumerable();
            return Task.FromResult(Products);
        }

        public Task<Product> AddProductAsync(Product product)
        {
            _products.Add(product);
            return Task.FromResult(product);
        }


        public Task DeleteProductAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
            }
            return Task.CompletedTask;
        }


        public async Task UpdateProductAsync(Product product)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
            }
            await Task.CompletedTask;
        }
    }

}