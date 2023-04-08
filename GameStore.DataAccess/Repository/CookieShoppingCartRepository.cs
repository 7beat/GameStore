using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;
using GameStore.Utility;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.DataAccess.Repository
{
    public class CookieShoppingCartRepository : ICookieShoppingCartRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieShoppingCartRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Add(CartItem item)
        {
            List<CartItem> cartItems = GetAll();
            CartItem existingItem = cartItems.FirstOrDefault(x => x.ProductId == item.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cartItems.Add(item);
            }

            UpdateCartItems(cartItems);
        }

		public void Add(ShoppingCart item)
		{
			List<ShoppingCart> cartItems = GetAll2();
			ShoppingCart existingItem = cartItems.FirstOrDefault(x => x.ProductId == item.ProductId);
            Console.WriteLine();
            if (existingItem != null)
			{
				existingItem.Count++;
			}
			else
			{
				cartItems.Add(item);
			}

			UpdateCartItems2(cartItems); // Intakes List of ShoppingCart
		}

		public List<ShoppingCart> GetAll2()
		{
			List<ShoppingCart> cartItems = new List<ShoppingCart>();
			var request = _httpContextAccessor.HttpContext.Request;

			if (request.Cookies.ContainsKey("CartCookie2"))
			{
				string cartCookie = request.Cookies["CartCookie2"];
				cartItems = JsonConvert.DeserializeObject<List<ShoppingCart>>(cartCookie);
			}

			return cartItems;
		}

		public List<CartItem> GetAll()
        {
            List<CartItem> cartItems = new List<CartItem>();
            var request = _httpContextAccessor.HttpContext.Request;

            if (request.Cookies.ContainsKey(AppConsts.CookieCart))
            {
                string cartCookie = request.Cookies[AppConsts.CookieCart];
                cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartCookie);
            }

            return cartItems;
        }

		public void Update(CartItem item) // do usunięcia ma 0
		{
			List<CartItem> cartItems = GetAll();
			CartItem existingItem = cartItems.FirstOrDefault(x => x.ProductId == item.ProductId);

			if (existingItem != null && item.Quantity > 0)
			{
				existingItem.Quantity = item.Quantity;
			}
			else
			{
				cartItems.Remove(existingItem);
			}

			UpdateCartItems(cartItems);
		}

		private void UpdateCartItems(List<CartItem> cartItems)
        {
            var response = _httpContextAccessor.HttpContext.Response;
            string cartJson = JsonConvert.SerializeObject(cartItems);
            response.Cookies.Append(AppConsts.CookieCart, cartJson, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30)
            });
        }

		private void UpdateCartItems2(List<ShoppingCart> cartItems)
		{
			var response = _httpContextAccessor.HttpContext.Response;
			string cartJson = JsonConvert.SerializeObject(cartItems);
			response.Cookies.Append("CartCookie2", cartJson, new CookieOptions
			{
				Expires = DateTime.Now.AddDays(30)
			});
		}
	}
}
