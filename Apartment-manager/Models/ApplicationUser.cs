// File: Models/ApplicationUser.cs

using Microsoft.AspNetCore.Identity;

namespace ApartmentManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        // ما نمی‌توانیم required را برای FullName بگذاریم مگر اینکه سازنده را تعریف کنیم.
        // ساده‌ترین راه برای رفع هشدار کامپایلر این است که آن را قابل Null کنیم:
        public string? FullName { get; set; }

        // یا اگر می‌خواهید آن را required نگه دارید، باید یک سازنده اضافه کنید:
        /*
        public ApplicationUser(string fullName)
        {
            this.FullName = fullName;
        }
        */
    }
}