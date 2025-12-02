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

        // Controllers/AuthController.cs - متد Register

       
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            // ۱. بررسی اعتبارسنجی مدل سمت سرور
            if (!ModelState.IsValid)
            {
                // 👈 مسیر ۱: ModelState نامعتبر است -> بازگشت خطا (با return)
                var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault();
                return BadRequest(new { message = error?.ErrorMessage ?? "اطلاعات وارد شده نامعتبر است." });
            }

            // ۲. بررسی وجود کاربر با این شماره موبایل (برای جلوگیری از تکرار)
            var existingUser = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
            if (existingUser != null)
            {
                // 👈 مسیر ۲: کاربر موجود است -> بازگشت خطا (با return)
                return BadRequest(new { message = "شماره موبایل قبلاً ثبت شده است." });
            }

            // ۳. ایجاد شیء کاربر جدید
            var user = new ApplicationUser
            {
                UserName = model.PhoneNumber,
                PhoneNumber = model.PhoneNumber,
                PhoneNumberConfirmed = true,
                Email = null,
                EmailConfirmed = false,
                FullName = model.FullName
            };

            // ۴. ثبت نام کاربر در دیتابیس
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // 👈 مسیر ۳: ثبت نام موفق -> بازگشت موفقیت (با return)
                await _signInManager.SignInAsync(user, isPersistent: true);
                return Ok(new { message = "ثبت نام و ورود موفقیت آمیز." });
            }

            // 👈 مسیر ۴ (پیش فرض): ثبت نام ناموفق به دلیل خطاهای Identity -> بازگشت خطا (با return)
            return BadRequest(new { message = result.Errors.First().Description });
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            // ۱. پیدا کردن کاربر بر اساس شماره موبایل (استفاده از FindByPhoneNumberAsync)
            // این روش برای جستجوی کاربر موجود یا جدید استانداردتر است.
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);

            if (user == null)
            {
                // === حالت ۱: کاربر جدید ===
                // اگر کاربر پیدا نشد، 404 برمی‌گرداند.
                return NotFound(new
                {
                    message = "کاربر با این شماره موبایل یافت نشد.",
                    needsRegistration = true
                });
            }

            // ۲. اگر رمز عبور ارسال نشده باشد (مرحله اول لاگین دو مرحله‌ای)
            if (string.IsNullOrEmpty(model.Password))
            {
                // === حالت ۲: کاربر موجود است، اما رمز عبور می‌خواهد (400) ===
                return BadRequest(new { message = "لطفاً رمز عبور را وارد کنید." });
            }

            // === ۳. تلاش برای ورود با رمز عبور کامل (مرحله دوم) ===

            // ما باید UserName را برای متد PasswordSignInAsync فراهم کنیم. در Identity، معمولاً UserName همان Email است.
            var userName = user.Email ?? user.UserName;

            if (string.IsNullOrEmpty(userName))
            {
                // این حالت نباید رخ دهد، اما برای پوشش خطای داخلی ضروری است.
                return StatusCode(500, new { message = "خطای داخلی: اطلاعات نام کاربری کاربر ناقص است." });
            }

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

            // === حالت ۵: رمز عبور اشتباه یا سایر خطاهای ورود ===
            // (این خط تضمین می‌کند که اگر هیچ یک از شرط‌های بالا برآورده نشد، متد مقداری برگرداند.)
            return Unauthorized(new { message = "رمز عبور یا اطلاعات وارد شده صحیح نیست." });
        }
    }
}