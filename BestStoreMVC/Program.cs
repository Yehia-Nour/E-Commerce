using BestStoreMVC.Data;
using BestStoreMVC.Models;
using BestStoreMVC.Repositories;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using sib_api_v3_sdk.Client;

namespace BestStoreMVC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
                options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            Configuration.Default.ApiKey.Add("api-key", builder.Configuration["BrevoSettings:ApiKey"]);

            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IAdminOrdersService, AdminOrdersService>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();


            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>))
                    as UserManager<ApplicationUser>;
                var roleManager = scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>))
                    as RoleManager<IdentityRole>;

                await DatabaseInitializer.SeedDataAsync(userManager, roleManager);
            }

            app.Run();
        }
    }
}
