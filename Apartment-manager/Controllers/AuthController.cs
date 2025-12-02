// File: Controllers/AuthController.cs

using ApartmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault();
                return BadRequest(new { message = error?.ErrorMessage ?? "اطلاعات وارد شده نامعتبر است." });
            }

            var existingUser = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
            if (existingUser != null)
            {
                return BadRequest(new { message = "شماره موبایل قبلاً ثبت شده است." });
            }

            var user = new ApplicationUser
            {
                // UserName باید تنظیم شود، آن را برابر با شماره موبایل قرار می‌دهیم
                UserName = model.PhoneNumber,

                PhoneNumber = model.PhoneNumber,
                PhoneNumberConfirmed = true,

                // Email را نادیده می‌گیریم
                Email = null,
                EmailConfirmed = false,

                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: true);
                return Ok(new { message = "ثبت نام و ورود موفقیت آمیز." });
            }

            return BadRequest(new { message = result.Errors.First().Description });
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            // ۱. بررسی اعتبارسنجی مدل سمت سرور (ModelState) 
            // اگر شماره موبایل اجباری (Required) نباشد، در اینجا 400 برمی‌گرداند.
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault();
                return BadRequest(new { message = error?.ErrorMessage ?? "اطلاعات وارد شده نامعتبر است." });
            }

            // ۲. پیدا کردن کاربر بر اساس شماره موبایل
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);

            if (user == null)
            {
                // === حالت ۱: کاربر جدید (404) ===
                return NotFound(new
                {
                    message = "کاربر با این شماره موبایل یافت نشد. لطفا ثبت نام کنید.",
                    needsRegistration = true
                });
            }

            // ۳. اگر رمز عبور ارسال نشده باشد (فقط شماره موبایل برای بررسی وجود کاربر)
            if (string.IsNullOrEmpty(model.Password))
            {
                // === حالت ۲: کاربر موجود است، اما رمز عبور می‌خواهد (400) ===
                return BadRequest(new { message = "لطفاً رمز عبور را وارد کنید." });
            }

            // ۴. تلاش برای ورود با رمز عبور کامل

            // UserName همان PhoneNumber است که در زمان ثبت نام تنظیم کردیم
            var userName = user.UserName;

            var result = await _signInManager.PasswordSignInAsync(
                userName,
                model.Password,
                isPersistent: true,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // === حالت ۴: ورود موفق ===
                return Ok(new { message = "ورود موفقیت آمیز.", redirectUrl = "/Home.html" });
            }

            // === حالت ۵: رمز عبور اشتباه ===
            return Unauthorized(new { message = "رمز عبور یا اطلاعات وارد شده صحیح نیست." });
        }
    }
}