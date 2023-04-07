using GameStoreWeb.Areas.Customer.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GameStoreWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (Request.Cookies.ContainsKey("ShoppingCart"))
            {
                string cartCookie = Request.Cookies["ShoppingCart"];
                var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartCookie);
                int totalQuantity = 0;

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

