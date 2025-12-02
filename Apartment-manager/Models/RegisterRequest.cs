using System.ComponentModel.DataAnnotations;

namespace ApartmentManager.Models
{
    public class RegisterRequest
    {
        public required string PhoneNumber { get; set; } // جدید
        public required string FullName { get; set; } // جدید
        public required string Email { get; set; } // جدید
        public required string Password { get; set; } // جدید
        public required string ConfirmPassword { get; set; } // جدید
    }
}