// File: Program.cs

using Apartment_manager.Data;
using ApartmentManager.Data;
using ApartmentManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// === اصلاح مهم: غیرفعال کردن اعتبارسنجی Email توسط Identity ===
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;        // نیاز به اعداد را غیرفعال می‌کند
    options.Password.RequireLowercase = false;    // نیاز به حروف کوچک را غیرفعال می‌کند
    options.Password.RequireNonAlphanumeric = false; // نیاز به کاراکترهای خاص را غیرفعال می‌کند
    options.Password.RequireUppercase = false;    // نیاز به حروف بزرگ را غیرفعال می‌کند
    // ⬅️ حداقل طول به 3 تغییر داده شد
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 0;
   // options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; // مجاز بودن کاراکترهای معمول
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
    
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// نمایش صفحه Login.html به عنوان صفحه پیش فرض
app.MapGet("/", context =>
{
    context.Response.Redirect("/Login.html");
    return Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();