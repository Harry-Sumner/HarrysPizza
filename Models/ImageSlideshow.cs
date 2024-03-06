namespace HarrysPizza.Models;
using System.ComponentModel.DataAnnotations;

public class ImageSlideshow
{
    [Key]
    public int ImageID { get; set; }
    public string ImageDescription { get; set; }
    public byte[] ImageData { get; set; }


}
