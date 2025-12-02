// Models/RegisterRequest.cs

using System.ComponentModel.DataAnnotations;

namespace ApartmentManager.Models
{
    public class RegisterRequest
    {
        [Required]
        public required string FullName { get; set; }

        [Required]
        [Phone(ErrorMessage = "شماره موبایل نامعتبر است.")]
        public required string PhoneNumber { get; set; }

        // ❌ فیلد Email حذف شده باشد

        [Required]
        public required string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "تکرار رمز عبور باید با رمز عبور اصلی یکسان باشد.")]
        public required string ConfirmPassword { get; set; }

        
        
    }
}