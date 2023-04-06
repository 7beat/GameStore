using GameStoreWeb.Areas.Customer.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GameStoreWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<CartItem> cartItems = new List<CartItem>();
            int totalQuantity = 0;

            if (Request.Cookies.ContainsKey("ShoppingCart"))
            {
                string cartCookie = Request.Cookies["ShoppingCart"];
                cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartCookie);

                foreach (var item in cartItems)
                {
                    totalQuantity += item.Quantity;
                }

                return View(totalQuantity);
            }
            else
            {
                return View(0);
            }
        }
    }
}

