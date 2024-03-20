using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HarrysPizza.Data;
using HarrysPizza.Models;
using Microsoft.AspNetCore.Mvc;

namespace HarrysPizza.Pages
{
    [Authorize (Roles = "Admin, Customer")] //Only allow admin and customers to view - stops page breaking when no user is logged in
    public class CheckoutModel : PageModel
    {
        public OrderHistory Order = new(); //Create new instance of Order
        private readonly HarrysPizzaContext _db;
        private readonly UserManager<HarrysPizzaUser> _UserManager;
        public IList<CheckoutItem> Items { get; private set; } //Create a list implementing CheckoutItem class called Items; declare getters and setters.

        public decimal Total;
        public long AmountPayable; //Declare variables that store total and payable amount

        public CheckoutModel(HarrysPizzaContext db, UserManager<HarrysPizzaUser> userManager)
        {
            _db = db;
            _UserManager = userManager;
          
        }
        public async Task OnGetAsync()
        {
            var user = await _UserManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db.CheckoutCustomers.FindAsync(user.Email); //find details of the user

            Items = _db.CheckoutItems.FromSqlRaw(
                "SELECT Menu.ID, Menu.Price, " + 
                "Menu.Name, " +
                "BasketItem.BasketID, BasketItem.Quantity " +
                "FROM Menu INNER JOIN BasketItem " +
                "ON Menu.ID = BasketItem.ItemID " +
                "WHERE BasketID = {0}", customer.BasketID
                ).ToList(); // return Items in order and calculate subtotal

            Total = 0;

            foreach (var item in Items)
            {
                Total += (item.Quantity * item.Price);
            }
            AmountPayable = (long)Total; //calculates total amount payable
        }
        public async Task<IActionResult> OnPostBuyAsync()
        {
            var currentOrder = _db.OrderHistory.FromSqlRaw("SELECT * FROM OrderHistory")
                .OrderByDescending(b => b.OrderNo)
                .FirstOrDefault(); //selects and sorts orderhistory

            if (currentOrder == null)
            {
                Order.OrderNo = 1; //if no order present then make one and assign as 1
            }
            else
            {
                Order.OrderNo = currentOrder.OrderNo + 1; //increment last order by 1 to create a new order
            }

            var user = await _UserManager.GetUserAsync(User);
            Order.Email = user.Email; //gets details of logged in user and set order email as users email
            _db.OrderHistory.Add(Order); //add all detauils stored in Order to database

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
                    OrderNo = Order.OrderNo, //store order details in OrderItem
                    ItemID = item.ItemID,
                    Quantity = item.Quantity
                }; 
                _db.OrderItems.Add(oi); //add order item to database
                _db.BasketItem.Remove(item); //remove items from basket to clear for users next order
            }
            await _db.SaveChangesAsync(); //save all changes to sql database
            return RedirectToPage("/Index"); //return to home page once order complete
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id) //takes id passed from button
        {
            var users = await _UserManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db
                .CheckoutCustomers
                .FindAsync(users.Email); //locate user details

            var item = await _db.BasketItem.FindAsync(id, customer.BasketID); //locate basket and item id in basketitem and save in variable

            if (item != null)
            {
                if (item.Quantity == 1) //If there is only 1 item then remove it from the basket
                {
                    _db.BasketItem.Remove(item);
                }
                else //if more than 1 then decrease the quantity by 1 and update data
                {
                    item.Quantity -= 1;
                    _db.BasketItem.Update(item); //update with new item details
                }

                await _db.SaveChangesAsync(); //save changes
            }

            return RedirectToPage("/Checkout"); //return to checkout page
        }

        public async Task<IActionResult> OnPostAddAsync(int id) //id of item is passed in via button page handler
        {
            var users = await _UserManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db //get details of user logged in
                .CheckoutCustomers
                .FindAsync(users.Email); 

            var item = await _db.BasketItem.FindAsync(id, customer.BasketID); //find the item and basketID in basket item database and assign to item

            if (item != null)
            {
                item.Quantity += 1; //if item is not null then increment item quantity by 1
                _db.BasketItem.Update(item); //update database with new item values
                await _db.SaveChangesAsync(); //save chnages to sql db
            }

            return RedirectToPage("/Checkout"); //return back to checkout upon completion
        }
    }
}
