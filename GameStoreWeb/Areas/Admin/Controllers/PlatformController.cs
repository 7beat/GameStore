using Microsoft.AspNetCore.Mvc;

namespace GameStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PlatformController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
