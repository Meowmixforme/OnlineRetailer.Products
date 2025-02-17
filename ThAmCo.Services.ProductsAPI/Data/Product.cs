using System.ComponentModel.DataAnnotations;

namespace ThAmCo.ProductsAPI.Data
{
    public class Product
    {

        public Product()
        {

        }

        public Product(int id, string name, float price, string description)
        {
            Id = id;
            Name = name;
            Price = price;
            Description = description;
        }

        [Key]
        public int Id { get; set; }
        //[Required]
        //[StringLength(80)]
        public string Name { get; set; } = string.Empty;
        //[Required]
        //[StringLength(100)]
        public string Description { get; set; } = string.Empty;
        public float Price { get; set; }
    }
}
