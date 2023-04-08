using BookStore.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IPlatformRepository Platform { get; }
        IGenreRepository Genre { get; }
        IProductRepository Product { get; }
        ICookieShoppingCartRepository CookieShoppingCart { get; }
		IShoppingCartRepository ShoppingCart { get; }
		IApplicationUserRepository ApplicationUser { get; }
		IOrderDetailRepository OrderDetail { get; }
		IOrderHeaderRepository OrderHeader { get; }

		void Save();
        Task SaveAsync();
    }
}
