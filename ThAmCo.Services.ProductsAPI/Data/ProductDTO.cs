﻿namespace ThAmCo.ProductsAPI.Data
{
    public class ProductDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public float Price { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
