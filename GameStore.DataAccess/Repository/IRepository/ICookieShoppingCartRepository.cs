using GameStore.Models;

namespace GameStore.DataAccess.Repository.IRepository
{
    public interface ICookieShoppingCartRepository
    {
		List<ShoppingCart> GetAll();
		void Add(ShoppingCart item);
		void Update(ShoppingCart item);
        void RemoveCart();
    }
}
