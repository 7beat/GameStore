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

        public void Update(CartItem item)
        {
            List<CartItem> cartItems = GetAll();
            CartItem existingItem = cartItems.FirstOrDefault(x => x.ProductId == item.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity = item.Quantity;
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
    }
}
