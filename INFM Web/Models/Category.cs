
namespace INFM_Web.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int Category_Id { get; set; }
        [Required]
        [MaxLength(40)]
        public required string CategoryName { get; set; }

        public required List<Product> Products { get; set; }
    }
}
