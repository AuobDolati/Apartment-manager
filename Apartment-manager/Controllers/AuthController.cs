// Controllers/AuthController.cs
// توجه: فضای نام (Namespace) در خط زیر به MyWebApp.Data تغییر یافت تا با فایل‌های مدل جدید سازگار باشد.
using ApartmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyWebApp.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Apartment_manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration; // ⬅️ اضافه شدن تزریق

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration) // ⬅️ اضافه شدن به سازنده
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // --- متد کمکی: تولید توکن JWT ---
        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Id),
                new Claim("fullname", user.FullName ?? "User") // استفاده از FullName
            };

            var keyString = _configuration["Jwt:Key"] ?? "YOUR_LONG_AND_SECURE_SECRET_KEY_MIN_16_CHARS";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7), // انقضای توکن
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // --- ۱. متد Login (بررسی ثبت نام / ورود نهایی) ---
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            // ۱. بررسی اعتبارسنجی مدل
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "شماره موبایل و رمز عبور الزامی است." });
            }

            var user = await _userManager.FindByNameAsync(model.PhoneNumber);

            // ۲. اگر کاربر یافت نشد (404 - باید ثبت نام کند)
            if (user == null)
            {
                // این پاسخ 404 برای هدایت کاربر به فرم ثبت نام استفاده می‌شود
                return NotFound(new { message = "شماره موبایل یافت نشد. لطفا ثبت نام کنید." });
            }

            // ۳. اگر رمز عبور خالی باشد (از فلو کلاینت می‌آید - مرحله ۱)
            if (string.IsNullOrEmpty(model.Password))
            {
                // 400 Bad Request: یعنی کاربر وجود دارد اما رمز عبور را وارد نکرده.
                return BadRequest(new { message = "رمز عبور برای این شماره لازم است. لطفا وارد کنید." });
            }

            // ۴. ورود نهایی (مرحله ۲)
            var result = await _signInManager.PasswordSignInAsync(model.PhoneNumber, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                string token = GenerateJwtToken(user); // ⬅️ تولید توکن برای ورود موفق

                return Ok(new
                {
                    message = "ورود موفقیت آمیز.",
                    token = token, // ⬅️ ارسال توکن
                    redirectUrl = "/Home.html"
                });
            }

            return Unauthorized(new { message = "رمز عبور اشتباه است." });
        }


        // --- ۲. متد Register (ثبت نام نهایی) ---
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "اطلاعات ارسالی صحیح نمی‌باشد.", errors = ModelState });
            }

            var user = new ApplicationUser
            {
                UserName = model.PhoneNumber,
                PhoneNumber = model.PhoneNumber,
                FullName = model.FullName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // تولید توکن پس از ثبت نام موفق
                string token = GenerateJwtToken(user);

                return Ok(new
                {
                    message = "ثبت نام با موفقیت انجام شد. در حال ورود...",
                    token = token, // ⬅️ ارسال توکن
                    redirectUrl = "/Home.html"
                });
            }

            var errorMessages = result.Errors.Select(e => e.Description);
            return BadRequest(new
            {
                message = "خطا در ثبت نام.",
                errors = errorMessages
            });
        }
    }
}