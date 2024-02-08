using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HarrysPizza.Data;
using HarrysPizza.Models;
using Microsoft.AspNetCore.Identity;

namespace HarrysPizza.Pages.Menu
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly HarrysPizzaContext _db;
        public IndexModel(HarrysPizza.Data.HarrysPizzaContext context, HarrysPizzaContext db,  UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _context = context;
        }
        private readonly HarrysPizza.Data.HarrysPizzaContext _context;
 


        public async Task<IActionResult> OnPostOrder (int itemID)
        {
            var user = await _userManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db
            .CheckoutCustomers
            .FindAsync(user.Email);


            var item = _db.BasketItem
                .FromSqlRaw("SELECT * FROM BasketItem WHERE ItemID = {0}" + 
                " AND BasketID = {1}", itemID, customer.BasketID)
                .ToList()
                .FirstOrDefault();

            if (item == null)
            {
                BasketItem newItem = new BasketItem
                {
                    BasketID = customer.BasketID,
                    ItemID = itemID,
                    Quantity = 1,
                };
                _db.BasketItem.Add(newItem);
                await _db.SaveChangesAsync();
            }
            else
            {
                item.Quantity = item.Quantity + 1;
                _db.Attach(item).State = EntityState.Modified;
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    throw new Exception($"Order not found!", e);
                }
            }
            return RedirectToPage();
        }

        [BindProperty]
        public string Search { get; set; }


        public IActionResult OnPostSearch()
        {
            Item = _context.Items
                .FromSqlRaw("SELECT * FROM Menu WHERE Name LIKE '%" + Search + "%'").ToList();
            return Page();
        }

        public IActionResult OnPostClear()
        {
            Item = _context.Items
                .FromSqlRaw("SELECT * FROM Menu").ToList();
            return Page();
        }

        public IList<Item> Item { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Items != null)
            {
                Item = await _context.Items.ToListAsync();
            }
        }
    }
}
