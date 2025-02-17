namespace ThAmCo.ProductsAPI.UnderCutters
{
    public class UnderCuttersServiceFake : IUnderCuttersService
    {
        private ProductDTO[] _products =
        {
        new ProductDTO { Id = 1, Name = "Fake product A"},
        new ProductDTO { Id = 2, Name = "Fake product B"},
        new ProductDTO { Id = 3, Name = "Fake product C"},
    };

        public Task<IEnumerable<ProductDTO>> GetProductsAsync()
        {
            var products = _products.AsEnumerable();
            return Task.FromResult(products);
        }



        public Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        public Task<ProductDTO> AddProductAsync(ProductDTO product)
        {
            var maxId = _products.Max(p => p.Id);
            product.Id = maxId + 1;
            var productList = _products.ToList();
            productList.Add(product);
            _products = productList.ToArray();
            return Task.FromResult(product);
        }

        public Task DeleteProductAsync(ProductDTO product)
        {
            var productList = _products.ToList();
            var productToRemove = productList.FirstOrDefault(p => p.Id == product.Id);
            if (productToRemove != null)
            {
                productList.Remove(productToRemove);
                _products = productList.ToArray();
            }
            return Task.CompletedTask;
        }

        public Task UpdateProductAsync(ProductDTO product)
        {
            var productList = _products.ToList();
            var index = productList.FindIndex(p => p.Id == product.Id);
            if (index >= 0)
            {
                productList[index] = product;
                _products = productList.ToArray();
            }
            return Task.CompletedTask;
        }
    }

}
