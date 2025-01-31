using BestStoreMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BestStoreMVC.Services
{
    public interface IUserService
    {
        (List<ApplicationUser> PaginatedItems, int TotalPages) GetAllUsers(int pageIndex);
        Task<(ApplicationUser? User, List<string> Roles, List<SelectListItem> AvailableRoles)> GetUserDetailsAsync(string id);
        Task<(bool Success, string Message)> EditUserRoleAsync(string userId, string newRole, string currentUserId);
        Task<(bool Success, string Message)> DeleteUserAccountAsync(string userId, string currentUserId);

    }
}
