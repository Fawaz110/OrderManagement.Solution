using System.ComponentModel.DataAnnotations;

namespace Service.Dtos
{
    public class ProductDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1, (double)decimal.MaxValue, ErrorMessage = "Price must be at least 1")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Stock must be at least 1")]
        public int Stock { get; set; }
    }
}
