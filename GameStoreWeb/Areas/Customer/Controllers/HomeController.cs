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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(string? searchQuery)
        {
            IEnumerable<Product> productList;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                productList = await _unitOfWork.Product.GetAllAsync(x => x.Title.Contains(searchQuery), "Platform");
            }
            else
            {
                productList = await _unitOfWork.Product.GetAllAsync("Platform");
            }

            return View(productList);
        }

        public async Task<IActionResult> Details(int productId)
        {
            if (productId == 0)
                return NotFound();

            ShoppingCart cartObj = new()
            {
                Product = await _unitOfWork.Product.GetFirstOrDefaultAsync(x => x.Id == productId, "Genre", "Platform"),
				Count = 1,
				ProductId = productId,
			};

			return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            if (User.Identity.IsAuthenticated)
            {
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                shoppingCart.ApplicationUserId = userId;

				ShoppingCart cartDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.ApplicationUserId == userId && x.ProductId == shoppingCart.ProductId);

				if (cartDb == null)
				{
					_unitOfWork.ShoppingCart.Add(shoppingCart);
					await _unitOfWork.SaveAsync();
					TempData["success"] = "Added to Shopping Cart!";
				}
				else
				{
					_unitOfWork.ShoppingCart.IncrementCount(cartDb, shoppingCart.Count);
					await _unitOfWork.SaveAsync();
					TempData["success"] = "Quantity increased by 1";
				}
            }
            else
            {
				List<CartItem> cartItems = new List<CartItem>();

				var productDb = await _unitOfWork.Product.GetFirstOrDefaultAsync(x => x.Id == shoppingCart.ProductId);

				CartItem existingItem = _unitOfWork.CookieShoppingCart.GetAll().FirstOrDefault(item => item.ProductId == shoppingCart.ProductId);

				if (existingItem != null)
				{
                    await Console.Out.WriteLineAsync("NOT IMPLEMENTED");
                    //existingItem.Quantity++;
                    //_unitOfWork.CookieShoppingCart.Update(existingItem);
                    //TempData["success"] = "Quantity increased by 1";
                }
				else
				{
					_unitOfWork.CookieShoppingCart.Add(new ShoppingCart
					{
						ProductId = productDb.Id,
                        Count = 1
					});
					TempData["success"] = "Added to Shopping Cart!";
				}
			}

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int productId)
        {
            Console.WriteLine(productId);
            return Ok();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}