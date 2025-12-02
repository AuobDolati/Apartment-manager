// Models/ApplicationUser.cs

using Microsoft.AspNetCore.Identity;

namespace ApartmentManager.Models
{
    // ApplicationUser از IdentityUser ارث می‌برد
    public class ApplicationUser : IdentityUser
    {
        // فیلد سفارشی شما
        public required string FullName { get; set; }

        public required string PhoneNumber { get; set; }

        // توجه: Email و UserName به صورت خودکار از IdentityUser ارث می‌رسند.
    }
}