using GameStore.DataAccess.Repository.IRepository;
using GameStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
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
            if (!string.IsNullOrEmpty(searchQuery))
            {
                Console.WriteLine("Search Bar used");
            }

            var productList = await _unitOfWork.Product.GetAllAsync("Platform");

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
    }
}