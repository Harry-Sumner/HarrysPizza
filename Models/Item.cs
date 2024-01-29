using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarrysPizza.Models
{
    public class Item
    {

        [Key]
        public int ID { get; set; }
        [StringLength(20)]
        public string Menu { get; set; }
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public decimal Price { get; set; }
        [StringLength(20)]
        public string DietrySpecial { get; set; }
        public string ImageDescription { get; set; }
        public byte[] ImageData { get; set; }

    }
}
