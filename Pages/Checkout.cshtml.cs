using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HarrysPizza.Data;
using HarrysPizza.Models;
using Microsoft.AspNetCore.Mvc;

namespace HarrysPizza.Pages
{
    [Authorize (Roles = "Admin, Customer")]
    public class CheckoutModel : PageModel
    {
        public OrderHistory Order = new();
        private readonly HarrysPizzaContext _db;
        private readonly UserManager<HarrysPizzaUser> _UserManager;
        public IList<CheckoutItem> Items { get; private set; }

        public decimal Total;
        public long AmountPayable;

        public CheckoutModel(HarrysPizzaContext db, UserManager<HarrysPizzaUser> userManager)
        {
            _db = db;
            _UserManager = userManager;
          
        }
        public async Task OnGetAsync()
        {
            var user = await _UserManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db.CheckoutCustomers.FindAsync(user.Email);

            Items = _db.CheckoutItems.FromSqlRaw(
                "SELECT Menu.ID, Menu.Price, " + 
                "Menu.Name, " +
                "BasketItem.BasketID, BasketItem.Quantity " +
                "FROM Menu INNER JOIN BasketItem " +
                "ON Menu.ID = BasketItem.ItemID " +
                "WHERE BasketID = {0}", customer.BasketID
                ).ToList();

            Total = 0;

            foreach (var item in Items)
            {
                Total += (item.Quantity * item.Price);
            }
            AmountPayable = (long)Total;
        }
        public async Task<IActionResult> OnPostBuyAsync()
        {
            var currentOrder = _db.OrderHistory.FromSqlRaw("SELECT * FROM OrderHistory")
                .OrderByDescending(b => b.OrderNo)
                .FirstOrDefault();

            if (currentOrder == null)
            {
                Order.OrderNo = 1;
            }
            else
            {
                Order.OrderNo = currentOrder.OrderNo + 1;
            }

            var user = await _UserManager.GetUserAsync(User);
            Order.Email = user.Email;
            _db.OrderHistory.Add(Order);

            CheckoutCustomer customer = await _db
                .CheckoutCustomers
                .FindAsync(user.Email);

            var basketItem = 
                _db.BasketItem
                .FromSqlRaw("SELECT * FROM BasketItem WHERE BasketID = {0}",customer.BasketID)
                .ToList();

            foreach (var item in basketItem)
            {
                OrderItem oi = new OrderItem
                {
                    OrderNo = Order.OrderNo,
                    ItemID = item.ItemID,
                    Quantity = item.Quantity
                };
                _db.OrderItems.Add(oi);
                _db.BasketItem.Remove(item);
            }
            await _db.SaveChangesAsync();
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var users = await _UserManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db
                .CheckoutCustomers
                .FindAsync(users.Email);

            var item = await _db.BasketItem.FindAsync(id, customer.BasketID);

            if (item != null)
            {
                if (item.Quantity == 1)
                {
                    _db.BasketItem.Remove(item);
                }
                else
                {
                    item.Quantity -= 1;
                    _db.BasketItem.Update(item);
                }

                await _db.SaveChangesAsync();
            }

            return RedirectToPage("/Checkout");
        }

        public async Task<IActionResult> OnPostAddAsync(int id)
        {
            var users = await _UserManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db
                .CheckoutCustomers
                .FindAsync(users.Email);

            var item = await _db.BasketItem.FindAsync(id, customer.BasketID);

            if (item != null)
            {
                item.Quantity += 1;
                _db.BasketItem.Update(item);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage("/Checkout");
        }
    }
}
