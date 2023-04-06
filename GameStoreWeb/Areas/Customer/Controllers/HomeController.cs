using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;
using GameStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> FilterProducts(string searchQuery)
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

            return PartialView("_ProductList", productList);
        }
    }
}