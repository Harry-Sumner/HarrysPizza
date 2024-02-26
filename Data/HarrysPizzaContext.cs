using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HarrysPizza.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace HarrysPizza.Data
{
    public class HarrysPizzaContext : IdentityDbContext<HarrysPizzaUser>
    {
        public HarrysPizzaContext (DbContextOptions<HarrysPizzaContext> options)
            : base(options)
        {
        }

        public DbSet<HarrysPizzaUser> HarrysPizzaUser { get; set; }

        public DbSet<Item> Items { get; set; } = default!;

        public DbSet<CheckoutCustomer> CheckoutCustomers { get; set; } = default!;
        public DbSet<Basket> Basket { get; set; } = default!;
        public DbSet<BasketItem> BasketItem { get; set; } = default!;
        public DbSet<OrderHistory> OrderHistory { get; set; } = default!;
        public DbSet<OrderItem> OrderItems { get; set; } = default!;

        [NotMapped]
        public DbSet<CheckoutItem> CheckoutItems { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<HarrysPizzaUser>().Ignore(e => e.Name);
            modelBuilder.Entity<Item>().ToTable("Menu");

            modelBuilder.Entity<BasketItem>().HasKey(t => new {t.ItemID, t.BasketID});
            modelBuilder.Entity<OrderItem>().HasKey(t => new {t.OrderNo, t.ItemID});
        }
    }
}
