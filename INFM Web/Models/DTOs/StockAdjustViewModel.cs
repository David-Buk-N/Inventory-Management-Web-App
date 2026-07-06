namespace INFM_Web.Models.DTOs
{
    public class StockAdjustViewModel
    {
        public int Stock_Id { get; set; }

        [Display(Name = "Product")]
        public string ProductName { get; set; } = "";

        [Display(Name = "Warehouse")]
        public string WarehouseName { get; set; } = "";

        [Display(Name = "Current Quantity")]
        public int CurrentQuantity { get; set; }

        [Required]
        [Display(Name = "Change (+ receive / - issue)")]
        public int Change { get; set; }

        [Display(Name = "Reason / Note")]
        public string? Note { get; set; }
    }
}
