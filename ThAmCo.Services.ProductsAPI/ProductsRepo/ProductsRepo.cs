using Microsoft.EntityFrameworkCore;
using Polly;
using ThAmCo.ProductsAPI.Data;


namespace ThAmCo.ProductsAPI.ProductsRepo
{
    public class ProductsRepo : IProductsRepo
    {
        private readonly ProductsContext _productsContext;
        private readonly DbSet<Product> _dbSet;

        public ProductsRepo(ProductsContext productsContext)
        {
            _productsContext = productsContext;
            _dbSet = _productsContext.Set<Product>();
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int Id)
        {
            return await _dbSet.FindAsync(Id);
        }


        public async Task DeleteProductAsync(int id)
        {
            var product = await _dbSet.FindAsync(id);
            if (product == null)
            {
                return;
            }

            if (_productsContext.Entry(product).State == EntityState.Detached)
            {
                _dbSet.Attach(product);
            }

            _dbSet.Remove(product);
            await _productsContext.SaveChangesAsync();
        }


        public async Task<Product> AddProductAsync(Product product)
        {
            await _dbSet.AddAsync(product);
            await _productsContext.SaveChangesAsync();
            return product;
        }

        public async Task UpdateProductAsync(Product product)
        {
            _dbSet.Attach(product);
            _productsContext.Entry(product).State = EntityState.Modified;
            await _productsContext.SaveChangesAsync();
        }
    }
}
