using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;

namespace BookStore.DataAccess.Repository.IRepository
{
	public interface IShoppingCartRepository : IRepository<ShoppingCart>
	{
		int IncrementCount(ShoppingCart shoppingCart, int count);
		int DecrementCount(ShoppingCart shoppingCart, int count);
	}
}
