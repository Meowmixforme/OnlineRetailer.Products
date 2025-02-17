using ThAmCo.ProductsAPI.Data;

namespace ThAmCo.ProductsAPI.OrderRepo
{
    public class Order
    {
        public int OrderId { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
    }

}