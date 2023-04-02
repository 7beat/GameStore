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

        public IActionResult Index()
        {
            var platformsList = _unitOfWork.Platform.GetAll();
            return View(platformsList);
        }
    }
}
