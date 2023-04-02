using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PlatformController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlatformController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var platformsList = await _unitOfWork.Platform.GetAllAsync();
            return View(platformsList);
        }
    }
}
