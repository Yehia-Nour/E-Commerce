using BestStoreMVC.Models;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Controllers
{
    [Authorize(Roles = "client")]
    [Route("/Client/Orders/{action=Index}/{id?}")]
    public class ClientOrdersController : Controller
    {
        private readonly IClientOrdersService _ordersService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly int pageSize = 5;

        public ClientOrdersController(IClientOrdersService ordersService, UserManager<ApplicationUser> userManager)
        {
            _ordersService = ordersService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var orders = await _ordersService.GetOrdersAsync(currentUser.Id, pageIndex, pageSize);
            int totalPages = await _ordersService.GetTotalPagesAsync(currentUser.Id, pageSize);

            ViewBag.Orders = orders;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var order = await _ordersService.GetOrderDetailsAsync(currentUser.Id, id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }

            return View(order);
        }
    }
}
