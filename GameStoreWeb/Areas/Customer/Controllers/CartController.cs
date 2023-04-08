using GameStore.DataAccess.Repository;
using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;
using GameStore.Models.ViewModels;
using GameStore.Utility;
using GameStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;

namespace GameStoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly IUnitOfWork _unitOfWork;

		[BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }

		public CartController(ILogger<CartController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

				ShoppingCartVM = new ShoppingCartVM()
				{
					ListCart = await _unitOfWork.ShoppingCart.GetAllAsync(x => x.ApplicationUserId == claim.Value, "Product"),
					OrderHeader = new(),
					CartItems = null
				};
				foreach (var cart in ShoppingCartVM.ListCart)
				{
					ShoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
				}
				return View(ShoppingCartVM);
			}


            List<Product> products = new();

            var cookieCart = _unitOfWork.CookieShoppingCart.GetAll();
            double cartTotal = 0;

            foreach (var item in cookieCart)
            {
                var product = await _unitOfWork.Product.GetFirstOrDefaultAsync(x => x.Id == item.ProductId);
                cartTotal += (product.Price * item.Quantity);
                products.Add(product);
            }

            ShoppingCartVM cartVM = new ShoppingCartVM()
            {
                CartItems = cookieCart, 
                CartTotal = cartTotal
            };

            foreach (var item in cartVM.CartItems)
            {
                item.Product = await _unitOfWork.Product.GetFirstOrDefaultAsync(x => x.Id == item.ProductId);
			}

            return View(cartVM);
        }

		public async Task<IActionResult> Plus(int cartId)
		{
            if (User.Identity.IsAuthenticated)
            {
				var cart = await _unitOfWork.ShoppingCart.GetFirstOrDefaultAsync(x => x.Id == cartId);
				_unitOfWork.ShoppingCart.IncrementCount(cart, 1);
				_unitOfWork.Save();
			}
			else
            {
                var cookieCart = _unitOfWork.CookieShoppingCart.GetAll().Where(x => x.ProductId == cartId).SingleOrDefault();
                cookieCart.Quantity++;
                _unitOfWork.CookieShoppingCart.Update(cookieCart);
            }
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Minus(int cartId)
        {
			if (User.Identity.IsAuthenticated)
			{
				var cart = await _unitOfWork.ShoppingCart.GetFirstOrDefaultAsync(x => x.Id == cartId);

				if (cart.Count <= 1)
				{
					_unitOfWork.ShoppingCart.Remove(cart);
				}
				else
				{
					_unitOfWork.ShoppingCart.DecrementCount(cart, 1);
				}
				_unitOfWork.Save();
			}
			else
			{
				var cookieCart = _unitOfWork.CookieShoppingCart.GetAll().Where(x => x.ProductId == cartId).SingleOrDefault();
				cookieCart.Quantity--;
				_unitOfWork.CookieShoppingCart.Update(cookieCart);
			}
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Remove(int cartId)
		{
			if (User.Identity.IsAuthenticated)
			{
				var cart = await _unitOfWork.ShoppingCart.GetFirstOrDefaultAsync(x => x.Id == cartId);
				_unitOfWork.ShoppingCart.Remove(cart);
				_unitOfWork.Save();
			}
			else
			{
				var cookieCart = _unitOfWork.CookieShoppingCart.GetAll().Where(x => x.ProductId == cartId).SingleOrDefault();
				cookieCart.Quantity = 0;
				_unitOfWork.CookieShoppingCart.Update(cookieCart);
			}
			return RedirectToAction(nameof(Index));
		}

	}
}