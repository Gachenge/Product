using Abno.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Abno.Data
{
    public class ApplicationDbContext : IdentityDbContext <User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserProduct>()
               .HasOne(up => up.Product)
               .WithMany(p => p.UserProducts)
               .HasForeignKey(up => up.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserProduct>()
                .HasOne(up => up.ProductUser)
                .WithMany(u => u.UserProducts)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
        public DbSet<Product> Product { get; set; }
        public DbSet<UserProduct> UserProducts { get; set; }
    }
}
