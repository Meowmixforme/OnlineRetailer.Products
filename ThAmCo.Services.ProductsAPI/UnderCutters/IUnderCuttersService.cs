namespace ThAmCo.ProductsAPI.UnderCutters
{
    public interface IUnderCuttersService
    {
        Task<IEnumerable<ProductDTO>> GetProductsAsync();
        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task<ProductDTO> AddProductAsync(ProductDTO product);
        Task DeleteProductAsync(ProductDTO product);

        Task UpdateProductAsync(ProductDTO product);
    }
}
