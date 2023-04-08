using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;
using GameStore.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace GameStoreWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
		private readonly IUnitOfWork _unitOfWork;

		public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                var cartItems = await _unitOfWork.ShoppingCart.GetAllAsync(x => x.ApplicationUserId == claim.Value);

                int totalQuantity = 0;

                foreach (var item in cartItems)
                {
                    totalQuantity += item.Count;
                }

                return View(totalQuantity);
            }

            if (Request.Cookies.ContainsKey(AppConsts.CookieCart))
            {
                string cartCookie = Request.Cookies[AppConsts.CookieCart];
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

