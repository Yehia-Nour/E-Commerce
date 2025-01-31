using BestStoreMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BestStoreMVC.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly int _pageSize = 5;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public (List<ApplicationUser> PaginatedItems, int TotalPages) GetAllUsers(int pageIndex)
        {
            IQueryable<ApplicationUser> query = _userManager.Users.OrderByDescending(u => u.CreatedAt);

            var (paginatedItems, totalPages) = ApplyPagination(query, pageIndex);
            return (paginatedItems, totalPages);
        }

        public async Task<(ApplicationUser? User, List<string> Roles, List<SelectListItem> AvailableRoles)> GetUserDetailsAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return (null, new List<string>(), new List<SelectListItem>());

            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null)
                return (null, new List<string>(), new List<SelectListItem>());

            var roles = await _userManager.GetRolesAsync(appUser);
            var availableRoles = _roleManager.Roles.ToList();

            var items = availableRoles.Select(role => new SelectListItem
            {
                Text = role.NormalizedName,
                Value = role.Name,
                Selected = roles.Contains(role.Name!)
            }).ToList();

            return (appUser, roles.ToList(), items);
        }

        public async Task<(bool Success, string Message)> EditUserRoleAsync(string userId, string newRole, string currentUserId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newRole))
                return (false, "Invalid user ID or role.");

            var roleExists = await _roleManager.RoleExistsAsync(newRole);
            var appUser = await _userManager.FindByIdAsync(userId);

            if (appUser == null || !roleExists)
                return (false, "User or role does not exist.");

            if (currentUserId == appUser.Id)
                return (false, "You cannot update your own role!");

            // Update user role
            var userRoles = await _userManager.GetRolesAsync(appUser);
            await _userManager.RemoveFromRolesAsync(appUser, userRoles);
            await _userManager.AddToRoleAsync(appUser, newRole);

            return (true, "User role updated successfully.");
        }

        public async Task<(bool Success, string Message)> DeleteUserAccountAsync(string userId, string currentUserId)
        {
            if (string.IsNullOrEmpty(userId))
                return (false, "Invalid user ID.");

            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
                return (false, "User not found.");

            if (currentUserId == appUser.Id)
                return (false, "You cannot delete your own account!");

            // Delete user
            var result = await _userManager.DeleteAsync(appUser);
            if (!result.Succeeded)
                return (false, "Unable to delete account: " + result.Errors.First().Description);

            return (true, "User account deleted successfully.");
        }

        private (List<ApplicationUser> PaginatedItems, int TotalPages) ApplyPagination(IQueryable<ApplicationUser> query, int pageIndex)
        {
            if (pageIndex < 1) pageIndex = 1;

            int totalCount = query.Count();
            int totalPages = (int)Math.Ceiling(totalCount / (double)_pageSize);
            var paginatedItems = query.Skip((pageIndex - 1) * _pageSize).Take(_pageSize).ToList();

            return (paginatedItems, totalPages);
        }
    }
}