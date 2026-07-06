namespace INFM_Web.Models
{
    [Table("Warehouse")]
    public class Warehouse
    {
        [Key]
        public int Warehouse_Id { get; set; }

        [Required]
        [Display(Name = "Warehouse Name")]
        public string WarehouseName { get; set; }

        [Required]
        [Display(Name = "Warehouse Code")]
        [MaxLength(20)]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Location / Address")]
        public string Location { get; set; }

        [Required]
        public string City { get; set; }

        [Display(Name = "Storage Capacity")]
        public int Capacity { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<Stock>? Stocks { get; set; }

        [NotMapped]
        [Display(Name = "Units On Hand")]
        public int UnitsOnHand => Stocks?.Sum(s => s.Quantity) ?? 0;
    }
}
