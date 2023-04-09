using GameStore.DataAccess.Repository;
using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;
using GameStore.Models.ViewModels;
using GameStore.Utility;
using GameStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace GameStoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly IUnitOfWork _unitOfWork;
		private readonly ICookieShoppingCartRepository _cookieCartRepository;

		[BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }

		public CartController(ILogger<CartController> logger, IUnitOfWork unitOfWork, ICookieShoppingCartRepository cookieCartRepository)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
			_cookieCartRepository = cookieCartRepository;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

				ShoppingCartVM = new ShoppingCartVM()
				{
					ListCart = await _unitOfWork.ShoppingCart.GetAllAsync(x => x.ApplicationUserId == userId, "Product"),
					OrderHeader = new(),
				};
				foreach (var cart in ShoppingCartVM.ListCart)
				{
					ShoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
				}
			}
			else
			{
				ShoppingCartVM = new()
				{
					ListCart = GetCookieCartProducts(),
					OrderHeader = new()
				};
				foreach (var cart in ShoppingCartVM.ListCart)
				{
					ShoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
				}
			}
            return View(ShoppingCartVM);
        }

        public async Task<IActionResult> Summary()
        {
			if (User.Identity.IsAuthenticated)
			{
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = await _unitOfWork.ShoppingCart.GetAllAsync(x => x.ApplicationUserId == userId, "Product", "ApplicationUser"),
                    OrderHeader = new()
                };

                ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == userId);

                ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.UserName;
                ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
                ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
                ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
                ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
                ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

                foreach (var cart in ShoppingCartVM.ListCart)
                {
                    ShoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
                }
			}
			else
			{
                ShoppingCartVM = new()
                {
                    ListCart = GetCookieCartProducts(),
                    OrderHeader = new()
                };
                foreach (var cart in ShoppingCartVM.ListCart)
                {
                    ShoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
                }
            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
		[ActionName(nameof(Summary))]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> SummaryPost()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                ShoppingCartVM.ListCart = await _unitOfWork.ShoppingCart.GetAllAsync(x => x.ApplicationUserId == userId, "Product");

                ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
                ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

                foreach (var cart in ShoppingCartVM.ListCart)
                {
                    cart.Price = (cart.Product.Price * cart.Count);
                    ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
                }
            }
            else
            {
                ShoppingCartVM.ListCart = GetCookieCartProducts();

                ShoppingCartVM.OrderHeader.GuestEmailAddress = "Email";
                ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;

                foreach (var cart in ShoppingCartVM.ListCart)
                {
                    cart.Price = (cart.Product.Price * cart.Count);
                    ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
                }
            }

			ShoppingCartVM.OrderHeader.PaymentStatus = AppConsts.PaymentStatusPending;
			ShoppingCartVM.OrderHeader.OrderStatus = AppConsts.StatusPending;

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId, // Product.Id
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            var domain = $"{Request.Scheme}://{Request.Host}/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + $"customer/cart/index",
                //CustomerEmail = ""
            };

            foreach (var item in ShoppingCartVM.ListCart)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title,
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == id, includeProperties: "ApplicationUser");
            
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStripePaymentID(id, orderHeader.SessionId, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(id, AppConsts.StatusApproved, AppConsts.PaymentStatusApproved);
                _unitOfWork.Save();
            }

            // Send email
            //_emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - GameStore", "<p>New Order Created</p>");

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll().Where(x => x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();

            return View(id);
        }

        public async Task<IActionResult> Plus(int cartId)
		{
            if (User.Identity.IsAuthenticated)
            {
				var cart = await _unitOfWork.ShoppingCart.GetFirstOrDefaultAsync(x => x.Id == cartId);
				_unitOfWork.ShoppingCart.IncrementCount(cart, 1);
				_unitOfWork.Save();
			}
			else
            {
                var cookieCart = _cookieCartRepository.GetAll().Where(x => x.ProductId == cartId).SingleOrDefault();
                cookieCart.Count++;
                _cookieCartRepository.Update(cookieCart);
            }
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Minus(int cartId)
        {
			if (User.Identity.IsAuthenticated)
			{
				var cart = await _unitOfWork.ShoppingCart.GetFirstOrDefaultAsync(x => x.Id == cartId);

				if (cart.Count <= 1)
				{
					_unitOfWork.ShoppingCart.Remove(cart);
				}
				else
				{
					_unitOfWork.ShoppingCart.DecrementCount(cart, 1);
				}
				_unitOfWork.Save();
			}
			else
			{
				var cookieCart = _cookieCartRepository.GetAll().Where(x => x.ProductId == cartId).SingleOrDefault();
				cookieCart.Count--;
				_cookieCartRepository.Update(cookieCart);
			}
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Remove(int cartId)
		{
			if (User.Identity.IsAuthenticated)
			{
				var cart = await _unitOfWork.ShoppingCart.GetFirstOrDefaultAsync(x => x.Id == cartId);
				_unitOfWork.ShoppingCart.Remove(cart);
				_unitOfWork.Save();
			}
			else
			{
				var cookieCart = _cookieCartRepository.GetAll().Where(x => x.ProductId == cartId).SingleOrDefault();
				cookieCart.Count = 0;
				_cookieCartRepository.Update(cookieCart);
			}
			return RedirectToAction(nameof(Index));
		}

		private IEnumerable<ShoppingCart> GetCookieCartProducts()
		{
			var cartJson = _cookieCartRepository.GetAll();
			List<ShoppingCart> shoppingCarts = new();

			foreach (var item in cartJson)
			{
				shoppingCarts.Add(new()
				{
                    Product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == item.ProductId),
                    ProductId = item.ProductId,
					Count = item.Count,
				});
			}
			return shoppingCarts;
		}
	}
}