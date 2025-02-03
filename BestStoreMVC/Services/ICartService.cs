using BestStoreMVC.DTOs;
using BestStoreMVC.Models;
using System.Security.Claims;

namespace BestStoreMVC.Services
{
    public interface ICartService
    {
        List<OrderItem> GetCartItems();
        decimal GetSubtotal();
        decimal GetTotal(decimal shippingFee);
        bool IsCartEmpty();
        bool ProcessCheckout(CheckoutDto model);
        CartSummaryDto GetCartSummary(decimal shippingFee);
        Task<(bool success, string message)> CreateOrderAsync(ClaimsPrincipal user);
    }
}
