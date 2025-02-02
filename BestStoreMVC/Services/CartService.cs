using BestStoreMVC.DTOs;
using BestStoreMVC.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace BestStoreMVC.Services
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public CartService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        private Dictionary<int, int> GetCartDictionary()
        {
            var request = _httpContextAccessor.HttpContext!.Request;
            var response = _httpContextAccessor.HttpContext.Response;

            string cookieValue = request.Cookies["shopping_cart"] ?? "";

            try
            {
                var cartJson = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(cookieValue));
                var dictionary = JsonSerializer.Deserialize<Dictionary<int, int>>(cartJson);
                if (dictionary != null)
                {
                    return dictionary;
                }
            }
            catch
            {
                response.Cookies.Delete("shopping_cart");
            }

            return new Dictionary<int, int>();
        }

        public List<OrderItem> GetCartItems()
        {
            var cartItems = new List<OrderItem>();
            var cartDictionary = GetCartDictionary();

            foreach (var pair in cartDictionary)
            {
                var product = _context.Products.Find(pair.Key);
                if (product == null) continue;

                cartItems.Add(new OrderItem
                {
                    Product = product,
                    Quantity = pair.Value,
                    UnitPrice = product.Price
                });
            }

            return cartItems;
        }

        public decimal GetSubtotal()
        {
            return GetCartItems().Sum(item => item.Quantity * item.UnitPrice);
        }

        public decimal GetTotal(decimal shippingFee)
        {
            return GetSubtotal() + shippingFee;
        }

        public bool IsCartEmpty()
        {
            return GetCartItems().Count == 0;
        }

        public bool ProcessCheckout(CheckoutDto model)
        {
            if (IsCartEmpty())
            {
                return false;
            }

            var httpContext = _httpContextAccessor.HttpContext!;
            httpContext.Session.SetString("DeliveryAddress", model.DeliveryAddress);
            httpContext.Session.SetString("PaymentMethod", model.PaymentMethod);

            return true;
        }
    }
}
