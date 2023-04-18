using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;
using GameStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace GameStoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICookieShoppingCartRepository _cookieCartRepository;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, ICookieShoppingCartRepository cookieCartRepository)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _cookieCartRepository = cookieCartRepository;
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
            await AddToCartAsync(shoppingCart);

            return RedirectToAction(nameof(Index));
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

        private async Task AddToCartAsync(ShoppingCart shoppingCart)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                ShoppingCart cartDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.ApplicationUserId == userId && x.ProductId == shoppingCart.ProductId);

                if (cartDb is null)
                {
                    shoppingCart.ApplicationUserId = userId;
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
                _cookieCartRepository.Add(new ShoppingCart
                {
                    ProductId = shoppingCart.ProductId,
                    Count = 1
                });
                TempData["success"] = "Added to Shopping Cart!";
            }
        }
    }
}