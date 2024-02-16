using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarrysPizza.Models
{
    public class Basket
    {
        [Key]
        public int BasketID {  get; set; }
    }
}
