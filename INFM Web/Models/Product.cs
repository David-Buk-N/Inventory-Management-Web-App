
namespace INFM_Web.Models
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public int Product_Id { get; set; }

        [Required]
        [DisplayName("Product Name")]
        public string? Product_Name { get; set; }

        [Required]
        [DisplayName("SKU")]
        public string? SKU { get; set; }

        [Required]
        [DisplayName("Unit Price")]
        [DataType(DataType.Currency)]
        public decimal Product_Price { get; set; }

        [Required]
        [DisplayName("Product Description")]
        public string? ProductDescription { get; set; }

        [DisplayName("Reorder Level")]
        public int ReorderLevel { get; set; }

        [Required]
        [DisplayName("Category")]
        public int Category_Id { get; set; }
        public Category? Category { get; set; }

        [Required]
        [DisplayName("Supplier")]
        public int Supplier_Id { get; set; }
        public Supplier? Supplier { get; set; }

        // Stock is tracked per warehouse via the Stock join entity.
        public ICollection<Stock>? Stocks { get; set; }

        [NotMapped]
        [DisplayName("Total Stock")]
        public int TotalStock => Stocks?.Sum(s => s.Quantity) ?? 0;

        [NotMapped]
        [DisplayName("Low Stock")]
        public bool IsLowStock => TotalStock <= ReorderLevel;

        [NotMapped]
        public string? Category_Name { get; set; }
    }
}
