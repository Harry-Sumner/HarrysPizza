using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HarrysPizza.Data;
using HarrysPizza.Models;

namespace HarrysPizza.Pages.Admin.Slideshow
{
    public class DeleteModel : PageModel
    {
        private readonly HarrysPizza.Data.HarrysPizzaContext _context;

        public DeleteModel(HarrysPizza.Data.HarrysPizzaContext context)
        {
            _context = context;
        }

        [BindProperty]
      public ImageSlideshow ImageSlideshow { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.ImageSlideshow == null)
            {
                return NotFound();
            }

            var imageslideshow = await _context.ImageSlideshow.FirstOrDefaultAsync(m => m.ImageID == id);

            if (imageslideshow == null)
            {
                return NotFound();
            }
            else 
            {
                ImageSlideshow = imageslideshow;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.ImageSlideshow == null)
            {
                return NotFound();
            }
            var imageslideshow = await _context.ImageSlideshow.FindAsync(id);

            if (imageslideshow != null)
            {
                ImageSlideshow = imageslideshow;
                _context.ImageSlideshow.Remove(ImageSlideshow);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./AdminIndex");
        }
    }
}
