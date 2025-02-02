using BestStoreMVC.Data;
using BestStoreMVC.DTOs;
using BestStoreMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Security.Claims;

namespace BestStoreMVC.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;


        public AccountService(UserManager<ApplicationUser> userManager,
                              RoleManager<IdentityRole> roleManager,
                              SignInManager<ApplicationUser> signInManager,
                              IConfiguration configuration,
                              IUrlHelperFactory urlHelperFactory,
                              IActionContextAccessor actionContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _urlHelperFactory = urlHelperFactory;

            _actionContextAccessor = actionContextAccessor;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterDto registerDto)
        {
            var user = new ApplicationUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.Email,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Address = registerDto.Address,
                CreatedAt = DateTime.Now,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "client");
            }

            return result;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> LoginAsync(string email, string password, bool rememberMe)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
            return result.Succeeded;
        }

        public async Task<ProfileDto?> GetProfileAsync(ClaimsPrincipal user)
        {
            var appUser = await _userManager.GetUserAsync(user);
            if (appUser == null)
            {
                return null;
            }

            return new ProfileDto
            {
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Email = appUser.Email ?? "",
                PhoneNumber = appUser.PhoneNumber,
                Address = appUser.Address
            };
        }

        public async Task<IdentityResult> UpdateProfileAsync(ClaimsPrincipal user, ProfileDto profileDto)
        {
            var appUser = await _userManager.GetUserAsync(user);
            if (appUser == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            appUser.FirstName = profileDto.FirstName;
            appUser.LastName = profileDto.LastName;
            appUser.UserName = profileDto.Email;
            appUser.Email = profileDto.Email;
            appUser.PhoneNumber = profileDto.PhoneNumber;
            appUser.Address = profileDto.Address;

            return await _userManager.UpdateAsync(appUser);
        }

        public async Task<IdentityResult> ChangePasswordAsync(ClaimsPrincipal user, PasswordDto passwordDto)
        {
            var appUser = await _userManager.GetUserAsync(user);
            if (appUser == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            var result = await _userManager.ChangePasswordAsync(appUser, passwordDto.CurrentPassword, passwordDto.NewPassword);
            return result;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            string resetUrl = _configuration["AppSettings:BaseUrl"] +
                              urlHelper.Action("ResetPassword", "Account", new { token }, protocol: "https");

            string senderName = _configuration["BrevoSettings:SenderName"] ?? "Admin";
            string senderEmail = _configuration["BrevoSettings:SenderEmail"] ?? "no-reply@beststore.com";
            string username = $"{user.FirstName} {user.LastName}";
            string subject = "Password Reset";
            string message = $"Dear {username},\n\n" +
                             "You can reset your password using the following link:\n\n" +
                             $"{resetUrl}\n\n" +
                             "Best Regards";

            EmailSender.SendEmail(senderName, senderEmail, username, email, subject, message);
            return true;
        }
        public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }



    }
}
