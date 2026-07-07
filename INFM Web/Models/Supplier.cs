
namespace INFM_Web.Models
{
    public class Supplier
    {
        [Key]
        public int Supplier_Id { get; set; }
        [Required]
        public required string SupplierName { get; set; }
        [Required]
        [Display(Name = "Street Address")]
        public required string Address { get; set; }
        [Required]
        [Display(Name = "City")]
        public required string City { get; set; }
        [Required]
        [Display(Name = "Province")]
        public required string Province { get; set; }
        [Required]
        [Display(Name = "Postal Code")]
        public required string PostalCode { get; set; }
        [Required]
        [Display(Name = "E-mail")]
        public required string Email { get; set; }
        [Required]
        [Display(Name = "Phone")]
        public required string Phone { get; set; }
        [Required]
        [Display(Name = "Contact Person")]
        public required string ContactPerson { get; set; }
    }
}
