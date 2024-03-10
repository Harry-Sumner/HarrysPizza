using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using HarrysPizza.Data;
using HarrysPizza.Models;

namespace HarrysPizza.Pages.Admin
{
    public class CreateModel2 : PageModel
    {
        private readonly HarrysPizza.Data.HarrysPizzaContext _context;

        public CreateModel2(HarrysPizza.Data.HarrysPizzaContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ImageSlideshow ImageSlideshow { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            foreach (var file in Request.Form.Files)
            {
                MemoryStream stream = new MemoryStream();
                file.CopyTo(stream);
                ImageSlideshow.ImageData = stream.ToArray();

                stream.Close();
                stream.Dispose();
            }

            _context.ImageSlideshow.Add(ImageSlideshow);
            await _context.SaveChangesAsync();

            return RedirectToPage("./AdminIndex");
        }
    }
}
