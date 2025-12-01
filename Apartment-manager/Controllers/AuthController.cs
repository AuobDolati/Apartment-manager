using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ApartmentManager.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore; // برای جستجوی مستقیم در دیتابیس

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = "شماره موبایل یا رمز عبور نامعتبر است." });
        }

        // 1. جستجو بر اساس شماره موبایل
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);

        if (user == null)
        {
            // 2. شماره موبایل یافت نشد -> کاربر جدید است و باید ثبت نام کند
            return NotFound(new { success = false, message = "کاربر یافت نشد. لطفا ابتدا ثبت نام کنید.", needsRegistration = true });
        }

        // 3. کاربر پیدا شد -> تلاش برای ورود

        // ASP.NET Identity به طور سنتی از UserName یا Email برای ورود استفاده می کند، 
        // بنابراین از UserName کاربر برای فراخوانی PasswordSignInAsync استفاده می کنیم.
        var result = await _signInManager.PasswordSignInAsync(
            user.UserName, // از UserName ذخیره شده استفاده می کنیم
            model.Password,
            isPersistent: true,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return Ok(new { success = true, message = "ورود موفقیت‌آمیز" });
        }

        return Unauthorized(new { success = false, message = "رمز عبور اشتباه است." });
    }
    // داخل کلاس AuthController
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = "لطفا تمام فیلدهای اجباری را تکمیل کنید." });
        }

        var user = new IdentityUser
        {
            UserName = model.Email, // Identity از Email به عنوان UserName استفاده می کند
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            EmailConfirmed = true // به فرض اینکه در WebView نیازی به تایید ایمیل نیست
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // ورود کاربر بلافاصله پس از ثبت نام
            await _signInManager.SignInAsync(user, isPersistent: false);
            return Ok(new { success = true, message = "ثبت نام و ورود موفقیت‌آمیز" });
        }

        // اگر ثبت نام ناموفق بود (مثلا ایمیل تکراری)
        var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
        return BadRequest(new { success = false, message = errorMessages });
    }
}