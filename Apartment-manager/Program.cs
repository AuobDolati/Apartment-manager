using ApartmentManager.Data; // استفاده از یک using صحیح (ApartmentManager.Data)
using ApartmentManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// === ۱. ثبت DbContext و Identity (ترکیب شده در یک بلاک) ===

// ثبت DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(connectionString));

// ثبت Identity (فقط یک بار)
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
  .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// === ۲. سرویس‌های کنترلر و صفحات ===
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

// === ۳. فعال‌سازی هویت و مجوز ===
app.UseAuthentication();
app.UseAuthorization();

// === ۴. نگاشت کنترلرها و صفحات ===
app.MapControllers();
app.MapRazorPages();

// === ۵. تنظیم صفحه پیش‌فرض ===
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/" || context.Request.Path == "/index.html")
    {
        context.Request.Path = "/Login.html";
    }
    await next();
});


app.Run();