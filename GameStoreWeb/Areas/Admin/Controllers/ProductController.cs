using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models.ViewModels;
using GameStore.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                GenreList = (await _unitOfWork.Genre.GetAllAsync())
                .Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                }).ToList(),
                PlatformList = (await _unitOfWork.Platform.GetAllAsync())
                .Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                }).ToList()
            };

            if (id is null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = await _unitOfWork.Product.GetFirstOrDefaultAsync(x => x.Id == id);
                return View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    if (obj.Product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                if (obj.Product.Id == 0)
                {
                    await _unitOfWork.Product.AddAsync(obj.Product);
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                    TempData["success"] = "Product updated successfully";
                }

                await _unitOfWork.SaveAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        #region API

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var productList = await _unitOfWork.Product.GetAllAsync(AppIncludes.Product.Platform, AppIncludes.Product.Genre);

            return Json(new { data = productList });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var obj = await _unitOfWork.Product.GetFirstOrDefaultAsync(x => x.Id == id);
            var title = obj.Title;

            if (obj is null)
                return Json(new { success = false, message = "Error while deleting" });

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);

            _unitOfWork.Product.Remove(obj);
            await _unitOfWork.SaveAsync();

            return Json(new { success = true, message = $"{title} deleted successfully!" });
        }

        #endregion
    }
}
