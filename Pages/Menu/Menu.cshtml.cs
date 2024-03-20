using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        private readonly UserManager<HarrysPizzaUser> _userManager;
        private readonly HarrysPizzaContext _db;
        public IndexModel(HarrysPizza.Data.HarrysPizzaContext context, HarrysPizzaContext db,  UserManager<HarrysPizzaUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _context = context;
        }
        private readonly HarrysPizza.Data.HarrysPizzaContext _context;
 


        public async Task<IActionResult> OnPostOrderAsync (int itemID)
        { 
            var user = await _userManager.GetUserAsync(User); //Get details of logged in user
            CheckoutCustomer customer = await _db
            .CheckoutCustomers
            .FindAsync(user.Email); //find user details


            var item = _db.BasketItem //select data from database and assign to item
                .FromSqlRaw("SELECT * FROM BasketItem WHERE ItemID = {0}" + 
                " AND BasketID = {1}", itemID, customer.BasketID)
                .ToList()
                .FirstOrDefault();

            if (item == null)
            {
                BasketItem newItem = new BasketItem  //If item isnt already in the basket then add it and save changes
                {
                    BasketID = customer.BasketID,
                    ItemID = itemID,
                    Quantity = 1,
                };
                _db.BasketItem.Add(newItem);
                await _db.SaveChangesAsync();
            }
            else //Item is already in the basket to increase quantity by 1 and save changes
            {
                item.Quantity = item.Quantity + 1;
                _db.Attach(item).State = EntityState.Modified;
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e) //Throw error message
                {
                    throw new Exception($"Order not found!", e);
                }
            }
           
            return RedirectToPage();
        }

        [BindProperty]
        public string Search { get; set; } //create a search field


        public IActionResult OnPostSearch()
        {
            Item = _context.Items
                .FromSqlRaw("SELECT * FROM Menu WHERE Name LIKE '%" + Search + "%'").ToList(); //Selects all menu items which contains search field in name and return them
            return Page();
        }

        public IActionResult OnPostClear() //clear previous results and display all items
        {
            Item = _context.Items
                .FromSqlRaw("SELECT * FROM Menu").ToList();
            return Page();
        }

        public IList<Item> Item { get;set; } = default!;

        public async Task OnGetAsync() //list menu items in menu database if the database is not empty.
        {
            if (_context.Items != null)
            {
                Item = await _context.Items.ToListAsync();
            }
        }
    }
}
