using BookStore.DataAccess.Repository;
using BookStore.DataAccess.Repository.IRepository;
using GameStore.DataAccess.Repository.IRepository;
using GameStoreWeb.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public IPlatformRepository Platform { get; private set; }
        public IGenreRepository Genre { get; private set; }
        public IProductRepository Product { get; private set; }
		public IShoppingCartRepository ShoppingCart { get; private set; }
		public IApplicationUserRepository ApplicationUser { get; private set; }
		public IOrderHeaderRepository OrderHeader { get; private set; }
		public IOrderDetailRepository OrderDetail { get; private set; }

		private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Platform = new PlatformRepository(_db);
            Genre = new GenreRepository(_db);
            Product = new ProductRepository(_db);
			ApplicationUser = new ApplicationUserRepository(_db);
			ShoppingCart = new ShoppingCartRepository(_db);
			OrderHeader = new OrderHeaderRepository(_db);
			OrderDetail = new OrderDetailRepository(_db);
		}

        public void Save()
        {
            _db.SaveChanges(); 
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
