using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ThAmCo.ProductsAPI.UnderCutters
{
    public class UnderCuttersService : IUnderCuttersService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://undercutters.azurewebsites.net/api/"; // Replace with actual API URL

        public UnderCuttersService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync("products");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<IEnumerable<ProductDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return products ?? Enumerable.Empty<ProductDTO>();
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"products/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return product ?? null; // This explicitly returns null if deserialization fails
        }

        public async Task<ProductDTO> AddProductAsync(ProductDTO product)
        {
            var content = new StringContent(JsonSerializer.Serialize(product), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("products", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var addedProduct = JsonSerializer.Deserialize<ProductDTO>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (addedProduct == null)
            {
                throw new InvalidOperationException("Failed to deserialize the added product");
            }

            return addedProduct;
        }

        public async Task DeleteProductAsync(ProductDTO product)
        {
            var response = await _httpClient.DeleteAsync($"products/{product.Id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateProductAsync(ProductDTO product)
        {
            var content = new StringContent(JsonSerializer.Serialize(product), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"products/{product.Id}", content);
            response.EnsureSuccessStatusCode();
        }
    }
}