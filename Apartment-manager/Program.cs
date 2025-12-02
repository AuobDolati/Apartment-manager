// Program.cs
using ApartmentManager.Data;
using ApartmentManager.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. تنظیمات دیتابیس EF Core ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- 2. تنظیمات Identity ---
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // تنظیمات رمز عبور (برای تطابق با minlength=3 در کلاینت)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3; // ⬅️ حداقل طول 3 کاراکتر
    options.Password.RequiredUniqueChars = 0;

    options.User.RequireUniqueEmail = false; // ما از PhoneNumber به جای ایمیل استفاده می‌کنیم
    options.User.AllowedUserNameCharacters = null; // اجازه استفاده از شماره موبایل به عنوان UserName
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// --- 3. تنظیمات JWT Authentication (اضافه شده برای توکن) ---
var jwtSettings = builder.Configuration.GetSection("Jwt");
// ⬅️ اینجا اصلاح شده تا اگر Key نبود، از یک مقدار پیش‌فرض استفاده کند
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "YOUR_LONG_AND_SECURE_SECRET_KEY_MIN_16_CHARS");


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// --- 4. سایر سرویس‌ها ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 5. تنظیمات Middleware ---
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // برای نمایش خطاهای 500 در محیط توسعه
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ⬅️ فعال‌سازی Authentication و Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ⬅️ Middleware برای روت کردن صفحات استاتیک (مثل Login.html و Home.html)
app.UseDefaultFiles(); // اجازه می‌دهد که index.html یا default.html به صورت خودکار بارگذاری شوند
app.UseStaticFiles();

app.Run();