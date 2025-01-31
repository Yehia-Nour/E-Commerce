using BestStoreMVC.Models;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/[controller]/{Action=Index}/{id?}")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }
        public IActionResult Index(int pageIndex = 1)
        {
            var (users, totalPages) = _userService.GetAllUsers(pageIndex);

            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            return View(users);
        }
        public async Task<IActionResult> Details(string? id)
        {
            var (appUser, roles, availableRoles) = await _userService.GetUserDetailsAsync(id!);

            if (appUser == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Roles = roles;
            ViewBag.SelectItems = availableRoles;

            return View(appUser);
        }

        public async Task<IActionResult> EditRole(string? id, string? newRole)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var (success, message) = await _userService.EditUserRoleAsync(id!, newRole!, currentUser!.Id);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] = message;
            return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> DeleteAccount(string? id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var (success, message) = await _userService.DeleteUserAccountAsync(id!, currentUser!.Id);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] = message;
            return RedirectToAction("Index");
        }
    }
}
