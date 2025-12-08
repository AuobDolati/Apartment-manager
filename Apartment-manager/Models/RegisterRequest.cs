using System.ComponentModel.DataAnnotations;

namespace ApartmentManager.Models
{
    // مدل کامل برای ثبت تمام اطلاعات ساختمان و مدیر
    public class RegisterRequest
    {
        // 1. اطلاعات مدیر/کاربر
        [Required(ErrorMessage = "نام و نام خانوادگی اجباری است.")]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "شماره موبایل اجباری است.")]
        [Phone(ErrorMessage = "شماره موبایل معتبر نیست.")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "رمز ورود اجباری است.")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "رمز ورود باید حداقل 4 کاراکتر باشد.")]
        public required string Password { get; set; }

        public string? IntroducerMobile { get; set; } // موبایل معرف (اختیاری)

        // 2. اطلاعات ساختمان
        [Required(ErrorMessage = "نام ساختمان اجباری است.")]
        public required string BuildingName { get; set; }

        [Required(ErrorMessage = "شهر اجباری است.")]
        public required string City { get; set; }

        public string? Address { get; set; } // آدرس ساختمان (اختیاری)

        [Required(ErrorMessage = "نوع کاربری ساختمان اجباری است.")]
        public required string UsageType { get; set; } // مثال: مسکونی، تجاری، سایر

        [Required(ErrorMessage = "تعداد واحد اجباری است.")]
        [Range(1, 9999, ErrorMessage = "تعداد واحد باید بین 1 تا 9999 باشد.")]
        public required int UnitCount { get; set; }
    }
}

namespace ApartmentManager.Models
{
    // مدل مورد نیاز برای مرحله دوم (تایید کد پیامک)
    public class VerifyCodeRequest
    {
        [Required(ErrorMessage = "شماره موبایل اجباری است.")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "کد تایید اجباری است.")]
        public required string VerificationCode { get; set; }
    }
}