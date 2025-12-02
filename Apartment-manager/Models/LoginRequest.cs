// File: Models/LoginRequest.cs

using System.ComponentModel.DataAnnotations;

namespace ApartmentManager.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "شماره موبایل اجباری است.")]
        [Phone(ErrorMessage = "شماره موبایل معتبر نیست.")]
        public required string PhoneNumber { get; set; }

        // فیلد Password باید Nullable باشد تا بتوان آن را در مرحله اول خالی فرستاد
        // [Required] از روی آن حذف می‌شود
        public string? Password { get; set; }
    }
}