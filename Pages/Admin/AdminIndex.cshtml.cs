﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HarrysPizza.Data;
using HarrysPizza.Models;

namespace HarrysPizza.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly HarrysPizza.Data.HarrysPizzaContext _context;

        public IndexModel(HarrysPizza.Data.HarrysPizzaContext context)
        {
            _context = context;
        }

        public IList<Item> Item { get;set; } = default!;
        // Creates a list view based on Item Class - with constructors

        public IList<ImageSlideshow> ImageSlideshow { get; set; } = default!;
        // Creates a list baed on the Image slideshow class - with constructors


        public async Task OnGetAsync()
        {
            if (_context.ImageSlideshow != null)
            {
                ImageSlideshow = await _context.ImageSlideshow.ToListAsync();
            }
            if (_context.Items != null)
            {
                Item = await _context.Items.ToListAsync();
            }
        }
    }
}
