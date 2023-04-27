using GameStore.Models;
using GameStore.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameStoreWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        // Products
        public DbSet<Platform> Platforms { get; set; }
		public DbSet<Genre> Genres { get; set; }
		public DbSet<Product> Products { get; set; }

		// Users
		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<ShoppingCart> ShoppingCarts { get; set; }

		// Orders/Payments
		public DbSet<OrderHeader> OrderHeaders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}