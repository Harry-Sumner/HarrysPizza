using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarrysPizza.Models
{
    public class CheckoutCustomer
    {
        [Key]
        [StringLength(100)]
        public string Email{ get; set; }

        [StringLength (50)]
        public string Name { get; set; }
        public int BasketID { get; set; }
    }
}
