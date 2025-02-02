using BestStoreMVC.DTOs;
using BestStoreMVC.Models;

namespace BestStoreMVC.Services
{
    public interface ICartService
    {
        List<OrderItem> GetCartItems();
        decimal GetSubtotal();
        decimal GetTotal(decimal shippingFee);
        bool IsCartEmpty();
        bool ProcessCheckout(CheckoutDto model);
    }
}
