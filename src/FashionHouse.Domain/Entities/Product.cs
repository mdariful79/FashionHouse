using FashionHouse.Domain.Contracts;
using FashionHouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Domain.Entities
{
    public class Product : IAggregateRoot<Guid>
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        //Navigation
        public Category Category { get; set; }
        public SubCategory SubCategory { get; set; }
        //public Inventory? Inventory { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        //public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
    }
}
