using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GameStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GenreController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenreController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var genreList = await _unitOfWork.Genre.GetAllAsync();
            return View(genreList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Genre obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Genre.Add(obj);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Genre created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id == 0)
                return NotFound();
            
            var platformDb = await _unitOfWork.Platform.GetFirstOrDefaultAsync(x => x.Id == id);

            if (platformDb is null)
                return NotFound();

            return View(platformDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var platformDb = _unitOfWork.Platform.GetFirstOrDefault(x => x.Id == id);

            if (platformDb is null)
                return NotFound();

            _unitOfWork.Platform.Remove(platformDb);
            _unitOfWork.Save();

            TempData["success"] = "Platform deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
