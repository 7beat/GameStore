using GameStore.Models.Identity;
using GameStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GameStore.DataAccess.Repository.IRepository;
using GameStore.Utility;

namespace GameStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;
            if (User.IsInRole(AppRoles.Admin))
            {
                orderHeaders = await _unitOfWork.OrderHeader.GetAllAsync(nameof(ApplicationUser));
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                orderHeaders = orderHeaders = await _unitOfWork.OrderHeader.GetAllAsync(x => x.ApplicationUserId == userId, nameof(ApplicationUser));
            }

            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(x => x.PaymentStatus == AppConsts.PaymentStatusPending);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatus == AppConsts.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatus == AppConsts.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatus == AppConsts.StatusApproved);
                    break;
                default:
                    // GetAll
                    break;
            }

            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}
