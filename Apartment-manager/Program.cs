using ApartmentManager.Data;
using ApartmentManager.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders; // ⬅️ لازم برای DefaultFilesOptions: اضافه شد.

var builder = WebApplication.CreateBuilder(args);

// --- 1. تنظیمات دیتابیس EF Core ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- 2. تنظیمات Identity ---
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // تنظیمات رمز عبور 
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 0;

    options.User.RequireUniqueEmail = false;
    options.User.AllowedUserNameCharacters = null;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// --- 3. تنظیمات JWT Authentication ---
var jwtSettings = builder.Configuration.GetSection("Jwt");
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
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // ⬅️ فراخوانی صحیح در اینجا


// --------------------------------------------------------
// 🚀 رفع مشکل Default Document و تنظیم ترتیب
// --------------------------------------------------------
// تنظیم صریح نام فایل پیش‌فرض برای آدرس ریشه
var defaultFileOptions = new DefaultFilesOptions();
defaultFileOptions.DefaultFileNames.Clear(); // حذف نام‌های پیش‌فرض مثل index.html
defaultFileOptions.DefaultFileNames.Add("login.html"); // اضافه کردن فایل مورد نظر

// فعال‌سازی Default Files (باید قبل از StaticFiles باشد)
app.UseDefaultFiles(defaultFileOptions);

// فعال‌سازی Static Files (باید قبل از Authentication و Routing باشد)
app.UseStaticFiles();
// --------------------------------------------------------


// ⬅️ فعال‌سازی Authentication و Authorization (پس از Static Files)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();