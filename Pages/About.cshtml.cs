using HarrysPizza.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HarrysPizza.Pages
{

        public class AboutModel : PageModel
        {
            private readonly HarrysPizza.Data.HarrysPizzaContext _context;

            public AboutModel(HarrysPizza.Data.HarrysPizzaContext context)
            {
                _context = context;
            }

      
            public IList<ImageSlideshow> ImageSlideshow { get; set; } = default!;

            public async Task OnGetAsync()
            {
                if (_context.ImageSlideshow != null)
                {
                    ImageSlideshow = await _context.ImageSlideshow.ToListAsync();
                }
            }
    }
}
