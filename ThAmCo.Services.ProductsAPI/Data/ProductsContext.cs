using Microsoft.EntityFrameworkCore;
using Polly;

namespace ThAmCo.ProductsAPI.Data
{
    public class ProductsContext : DbContext
    {
        public ProductsContext()
        {
            // location of development Products database
            var folder = Environment.SpecialFolder.MyDocuments;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "Products.db");
        }
        public string DbPath { get; set; } = string.Empty;
        public DbSet<Product> Products { get; set; }


        public ProductsContext(DbContextOptions<ProductsContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(x =>
            {
                x.Property(p => p.Name).IsRequired();
            });

            modelBuilder.Entity<Product>().HasData(
        new() { Id = 1, Name = "Test product G", Price = 2.00f, Description = " Test" },
        new() { Id = 2, Name = "Test product H", Price = 2.00f, Description = " Test" },
        new() { Id = 3, Name = "Test product I", Price = 2.00f, Description = " Test" }

                );

            //protected override void OnModelCreating(ModelBuilder builder)
            //{
            //    base.OnModelCreating(builder);



            //builder.Entity<Product>(p =>
            //{
            //    //p.Property(p => p.Name).IsRequired();
            //    //p.Property(p => p.Description).IsRequired();
            //    //p.Property(p => p.Price).IsRequired();

            //});

            //            builder.Entity<Product>()
            //                .HasMany(p => p.Products)
            //               .WithOne(o => o.Order)
            //               .HasForeignKey(p => p.Id);
        }
    }
}

