using BestStoreMVC.DTOs;
using Microsoft.AspNetCore.Identity;

namespace BestStoreMVC.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterDto registerDto);
        Task SignOutAsync();
        Task<bool> LoginAsync(string email, string password, bool rememberMe);
    }
}
