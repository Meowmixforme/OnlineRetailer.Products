using ThAmCo.ProductsAPI.Data;


namespace ThAmCo.ProductsAPI.ProductsRepo
{
    public interface IProductsRepo
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> AddProductAsync(Product product);
        Task DeleteProductAsync(int id);

        Task UpdateProductAsync(Product product);

    }
}
