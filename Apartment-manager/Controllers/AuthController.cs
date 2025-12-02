// Controllers/AuthController.cs

using ApartmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq; // اضافه کردن این using برای First().Description در Register
using System.Threading.Tasks;
using Apartment_manager.Data;

namespace ApartmentManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    return Ok(new { message = "ثبت نام و ورود موفقیت آمیز." });
                }

                return BadRequest(new { message = result.Errors.First().Description });
            }
            return BadRequest(new { message = "اطلاعات وارد شده نامعتبر است." });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            // ۱. پیدا کردن کاربر بر اساس شماره موبایل
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);

            if (user == null)
            {
                // ===> اگر کاربر وجود ندارد: پاسخ ۴۰۴ <===
                return NotFound(new
                {
                    message = "کاربر با این شماره موبایل یافت نشد.",
                    needsRegistration = true // پرچم برای JavaScript
                });
            }

            // ۲. اگر رمز عبور ارسال نشده باشد (مرحله ۱ در Login.html)
            if (string.IsNullOrEmpty(model.Password))
            {
                // ===> اگر کاربر وجود دارد اما رمز خواسته نشده: پاسخ ۴۰۰ (برو مرحله رمز) <===
                return BadRequest(new { message = "لطفاً رمز عبور را وارد کنید." });
            }

            // ۳. تلاش برای ورود با رمز عبور کامل (مرحله ۲ در Login.html)
            var result = await _signInManager.PasswordSignInAsync(
                user.Email, // استفاده از ایمیل به عنوان UserName در Identity
                model.Password,
                isPersistent: true,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // ورود موفق
                return Ok(new { message = "ورود موفقیت آمیز.", redirectUrl = "/Home.html" });
            }

            // ۴. رمز عبور اشتباه
            return Unauthorized(new { message = "رمز عبور اشتباه است." });
        }
    }
}