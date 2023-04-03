using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }

            var platformDb = await _unitOfWork.Platform.GetFirstOrDefaultAsync(x => x.Id == id);

            if (platformDb is null)
                    return NotFound();

            return View(platformDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Platform platform)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Platform.Update(platform);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Platform edited successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(platform);
        }
    }
}
