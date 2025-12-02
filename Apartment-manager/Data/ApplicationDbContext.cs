// ApplicationDbContext.cs

using ApartmentManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

// === توجه: اگر نام پروژه شما Apartment_manager.Web باشد، Namespace باید آن باشد.
// اما با توجه به ساختار پروژه شما، فرض می‌کنیم:
namespace ApartmentManager.Data // مطمئن شوید نام Namespace پروژه شما در اینجا صحیح است
{
    // باید از ApplicationUser به عنوان نوع کاربر ارث ببرد
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}