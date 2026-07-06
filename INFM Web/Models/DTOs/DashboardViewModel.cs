namespace INFM_Web.Models.DTOs
{
    public class DashboardViewModel
    {
        public int ProductCount { get; set; }
        public int SupplierCount { get; set; }
        public int WarehouseCount { get; set; }
        public int TotalUnitsOnHand { get; set; }
        public int LowStockCount { get; set; }
    }
}
