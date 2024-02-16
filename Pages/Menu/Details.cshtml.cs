using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HarrysPizza.Data;
using HarrysPizza.Models;

namespace HarrysPizza.Pages.Menu
{
    public class DetailsModel : PageModel
    {
        private readonly HarrysPizza.Data.HarrysPizzaContext _context;

        public DetailsModel(HarrysPizza.Data.HarrysPizzaContext context)
        {
            _context = context;
        }

      public Item Item { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FirstOrDefaultAsync(m => m.ID == id);
            if (item == null)
            {
                return NotFound();
            }
            else 
            {
                Item = item;
            }
            return Page();
        }
    }
}
