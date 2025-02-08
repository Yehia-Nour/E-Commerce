using BestStoreMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/Orders/{action=Index}/{id?}")]
    public class AdminOrdersController : Controller
    {
        private readonly IAdminOrdersService _adminOrdersService;

        public AdminOrdersController(IAdminOrdersService adminOrdersService)
        {
            _adminOrdersService = adminOrdersService;
        }

        public IActionResult Index(int pageIndex)
        {
            var result = _adminOrdersService.GetAllOrders(pageIndex);

            ViewBag.Orders = result.Orders;
            ViewBag.PageIndex = result.PageIndex;
            ViewBag.TotalPages = result.TotalPages;

            return View();
        }

        public IActionResult Details(int id)
        {
            var order = _adminOrdersService.GetOrderById(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.NumOrders = _adminOrdersService.GetClientOrderCount(order.ClientId);
            return View(order);
        }

        public IActionResult Edit(int id, string? payment_status, string? order_status)
        {
            bool updated = _adminOrdersService.UpdateOrderStatus(id, payment_status, order_status);
            if (!updated)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Details", new { id });
        }
    }
}
