namespace ThAmCo.ProductsAPI.Data
{
    public static class ProductsInitialiser
    {
        public static async Task SeedTestData(ProductsContext context)
        {
            if (context.Products.Any())
            {
                //db seems to be seeded
                return;
            }

            //Seed database with test data-- wouldn't normally do this for production code

            var products = new List<Product>
    {
        new() { Id = 1, Name = "Test product G", Price = 2.00f, Description = " Test"},
        new() { Id = 2, Name = "Test product H", Price = 2.00f, Description = " Test"},
        new() { Id = 3, Name = "Test product I", Price = 2.00f, Description = " Test"},

    };
            products.ForEach(p => context.Add(p));
            await context.SaveChangesAsync();
        }

    }
}

