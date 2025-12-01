using System.ComponentModel.DataAnnotations;

namespace ApartmentManager.Models
{
    // این مدل برای دریافت شماره موبایل و رمز عبور از WebView است
    public class LoginRequest
    {
        [Required(ErrorMessage = "شماره موبایل الزامی است.")]
        [Phone(ErrorMessage = "فرمت شماره موبایل معتبر نیست.")]
        [Display(Name = "شماره موبایل")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "رمز عبور الزامی است.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}