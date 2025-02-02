using BestStoreMVC.DTOs;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private const decimal shippingFee = 10.0m;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            ViewBag.CartItems = _cartService.GetCartItems();
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Subtotal = _cartService.GetSubtotal();
            ViewBag.Total = _cartService.GetTotal(shippingFee);

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Index(CheckoutDto model)
        {
            ViewBag.CartItems = _cartService.GetCartItems();
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Subtotal = _cartService.GetSubtotal();
            ViewBag.Total = _cartService.GetTotal(shippingFee);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!_cartService.ProcessCheckout(model))
            {
                ViewBag.ErrorMessage = "Your cart is empty";
                return View(model);
            }

            if (model.PaymentMethod == "paypal" || model.PaymentMethod == "credit_card")
            {
                return RedirectToAction("Index", "Checkout");
            }

            return RedirectToAction("Confirm");
        }
    }
}
