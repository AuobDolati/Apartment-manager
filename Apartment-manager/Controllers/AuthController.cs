using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ApartmentManager.Models; // برای دسترسی به LoginRequest
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // برای Ignore کردن احراز هویت

[ApiController]
[Route("api/[controller]")] // آدرس API: /api/Auth
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    // تزریق وابستگی (Dependency Injection) برای دسترسی به سرویس های Identity
    public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // این endpoint درخواست ورود را پردازش می کند
    [HttpPost("login")] // آدرس کامل: /api/Auth/login
    [AllowAnonymous] // اجازه دسترسی بدون نیاز به احراز هویت
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = "اطلاعات ورودی معتبر نیست." });
        }

        // تلاش برای ورود
        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            isPersistent: true, // مهم: کوکی را حفظ می کند که برای WebView حیاتی است
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            // Identity کوکی احراز هویت را در پاسخ HTTP قرار می دهد
            return Ok(new { success = true, message = "ورود موفقیت‌آمیز" });
        }

        // اگر ورود موفقیت آمیز نبود
        return Unauthorized(new { success = false, message = "نام کاربری یا رمز عبور اشتباه است." });
    }
}