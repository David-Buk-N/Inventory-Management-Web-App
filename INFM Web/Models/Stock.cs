namespace INFM_Web.Models
{
    /// <summary>
    /// The quantity of a single <see cref="Product"/> held in a single
    /// <see cref="Warehouse"/>. A product can have one Stock row per warehouse.
    /// </summary>
    [Table("Stock")]
    public class Stock
    {
        [Key]
        public int Stock_Id { get; set; }

        [Required]
        [Display(Name = "Product")]
        public int Product_Id { get; set; }
        public Product? Product { get; set; }

        [Required]
        [Display(Name = "Warehouse")]
        public int Warehouse_Id { get; set; }
        public Warehouse? Warehouse { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int Quantity { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
