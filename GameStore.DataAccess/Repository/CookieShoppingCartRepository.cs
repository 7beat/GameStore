using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;
using GameStore.Utility;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace GameStore.DataAccess.Repository
{
	public class CookieShoppingCartRepository : ICookieShoppingCartRepository
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CookieShoppingCartRepository(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public void Add(ShoppingCart item)
		{
			List<ShoppingCart> cartItems = GetAll();
			ShoppingCart existingItem = cartItems.FirstOrDefault(x => x.ProductId == item.ProductId);

			if (existingItem != null)
			{
				existingItem.Count++;
			}
			else
			{
				cartItems.Add(item);
			}

			UpdateCartItems(cartItems);
		}

		public List<ShoppingCart> GetAll()
		{
			List<ShoppingCart> cartItems = new List<ShoppingCart>();
			var request = _httpContextAccessor.HttpContext.Request;

			if (request.Cookies.ContainsKey(AppConsts.CookieCart))
			{
				string cartCookie = request.Cookies[AppConsts.CookieCart];
				cartItems = JsonConvert.DeserializeObject<List<ShoppingCart>>(cartCookie);
			}

			return cartItems;
		}

		public void Update(ShoppingCart item)
		{
			List<ShoppingCart> cartItems = GetAll();
			var existingItem = cartItems.FirstOrDefault(x => x.ProductId == item.ProductId);

			if (existingItem != null && item.Count > 0)
			{
				existingItem.Count = item.Count;
			}
			else
			{
				cartItems.Remove(existingItem);
			}

			UpdateCartItems(cartItems);
		}

        private void UpdateCartItems(List<ShoppingCart> cartItems)
		{
			var response = _httpContextAccessor.HttpContext.Response;
			string cartJson = JsonConvert.SerializeObject(cartItems);
			response.Cookies.Append(AppConsts.CookieCart, cartJson, new CookieOptions
			{
				Expires = DateTime.Now.AddDays(30)
			});
		}
	}
}
