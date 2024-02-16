using System.ComponentModel.DataAnnotations;

namespace HarrysPizza.Models
{
    public class OrderItem
    {
        [Required]
        public int OrderNo { get; set; }
        [Required]
        public int ItemID { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
