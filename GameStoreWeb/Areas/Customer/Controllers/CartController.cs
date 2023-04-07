using GameStore.DataAccess.Repository;
using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;
using GameStore.Models.ViewModels;
using GameStore.Utility;
using GameStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace GameStoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CartController(ILogger<CartController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
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
    }
}