using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HarrysPizza.Models;

namespace HarrysPizza.Data
{
    public class HarrysPizzaContext : IdentityDbContext
    {
        public HarrysPizzaContext (DbContextOptions<HarrysPizzaContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Items { get; set; } = default!;

        public DbSet<CheckoutCustomer> CheckoutCustomers { get; set; } = default!;
        public DbSet<Basket> Basket { get; set; } = default!;
        public DbSet<BasketItem> BasketItem { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Item>().ToTable("Menu");

            modelBuilder.Entity<BasketItem>().HasKey(t => new {t.ItemID, t.BasketID});
        }
    }
}
