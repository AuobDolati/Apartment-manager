using Apartment_manager.Data;
using ApartmentManager.Data;
using ApartmentManager.Models; // مطمئن شوید که این using وجود دارد
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Apartment_manager.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ۱. ثبت DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ۲. ثبت Identity: استفاده از ApplicationUser به جای IdentityUser
// شما مدل کاربری سفارشی خودتان (ApplicationUser) را به Identity معرفی می‌کنید.
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ۳. سرویس‌های کنترلر و صفحات
builder.Services.AddControllers();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ۴. فعال‌سازی هویت و مجوز
app.UseAuthentication();
app.UseAuthorization();

// ۵. نگاشت کنترلرها و صفحات
app.MapControllers();
app.MapRazorPages();

// ۶. تنظیم صفحه پیش‌فرض (هدایت از مسیر ریشه به Login.html)
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/" || context.Request.Path == "/index.html")
    {
        context.Request.Path = "/Login.html";
    }
    await next();
});


app.Run();