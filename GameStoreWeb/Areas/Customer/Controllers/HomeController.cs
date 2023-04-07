using GameStore.DataAccess.Repository;
using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;
using GameStore.Utility;
using GameStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

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

        public async Task<IActionResult> Details(int? productId)
        {
            if (productId is null || productId == 0)
                return NotFound();

            var productDb = await _unitOfWork.Product.GetFirstOrDefaultAsync(x => x.Id == productId, AppIncludes.Product.Platform, AppIncludes.Product.Genre);

            return View(productDb);
        }

        // Concept Cookies based Shopping Cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int productId)
        {
            List<CartItem> cartItems = new List<CartItem>();

            var productDb = await _unitOfWork.Product.GetFirstOrDefaultAsync(x => x.Id == productId);

            CartItem existingItem = _unitOfWork.CookieShoppingCart.GetAll().FirstOrDefault(item => item.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
                _unitOfWork.CookieShoppingCart.Update(existingItem);
                TempData["success"] = "Quantity increased by 1";
            }
            else
            {
                _unitOfWork.CookieShoppingCart.Add(new CartItem
                {
                    ProductId = productDb.Id,
                    ProductName = productDb.Title,
                    Quantity = 1
                });
                TempData["success"] = "Added to Shopping Cart!";
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