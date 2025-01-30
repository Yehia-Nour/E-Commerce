using BestStoreMVC.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BestStoreMVC.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterDto registerDto);
        Task SignOutAsync();
        Task<bool> LoginAsync(string email, string password, bool rememberMe);
        Task<ProfileDto?> GetProfileAsync(ClaimsPrincipal user);
        Task<IdentityResult> UpdateProfileAsync(ClaimsPrincipal user, ProfileDto profileDto);
        Task<IdentityResult> ChangePasswordAsync(ClaimsPrincipal user, PasswordDto passwordDto);
        Task<bool> ForgotPasswordAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
