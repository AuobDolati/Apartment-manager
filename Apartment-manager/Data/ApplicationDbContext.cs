using ApartmentManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManager.Data
{
    // === مهم: ارث بری از IdentityDbContext<ApplicationUser> ===
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // اگر مدل‌های دیگری دارید، آن‌ها را در اینجا به عنوان DbSet تعریف کنید.
        // public DbSet<ModelName> ModelNames { get; set; }
    }
}