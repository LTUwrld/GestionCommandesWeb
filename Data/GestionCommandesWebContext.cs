using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GestionCommandesWeb.Models;

namespace GestionCommandesWeb.Data
{
    public class GestionCommandesWebContext : DbContext
    {
        public GestionCommandesWebContext (DbContextOptions<GestionCommandesWebContext> options)
            : base(options)
        {
        }

        public DbSet<GestionCommandesWeb.Models.Orders> Orders { get; set; } = default!;
        public DbSet<GestionCommandesWeb.Models.Customers> Customers { get; set; } = default!;
        public DbSet<GestionCommandesWeb.Models.Order_Details> Order_Details { get; set; } = default!;
        public DbSet<GestionCommandesWeb.Models.Products> Products { get; set; } = default!;
        public DbSet<GestionCommandesWeb.Models.Shippers> Shippers { get; set; } = default!;



        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Order_Details>().HasKey(k => new { k.OrderID, k.ProductID });

            builder.Entity<Order_Details>()
                .HasOne(p => p.Orders)
                .WithMany(o => o.Order_Details)
                .HasForeignKey(od => od.OrderID);

            builder.Entity<Order_Details>()
                .HasOne(p => p.Products)
                .WithMany(pr => pr.Order_Details)
                .HasForeignKey(od => od.ProductID);

            builder.Entity<Orders>()
            .HasOne(o => o.Customers)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerID);

            builder.Entity<Orders>()
            .HasOne(o => o.Shippers)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.ShipVia);

            base.OnModelCreating(builder);
        }
    }
}
