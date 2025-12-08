using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyWebApp.Data; // استفاده از فضای نام مدل‌ها و DbContext سفارشی شما
using System.Text;

// تنظیمات CORS برای اجازه دادن به درخواست‌های فرانت‌اند
const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ==============================================================================
// 💡 ۱. پیکربندی سرویس پایگاه داده و DbContext
// ==============================================================================

// برای مثال از SQLite استفاده شده است. شما می‌توانید آن را به SQL Server یا PostgreSQL تغییر دهید.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       "Data Source=app.db"; // رشته اتصال پیش‌فرض برای توسعه

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString)
);


// ==============================================================================
// 💡 ۲. پیکربندی ASP.NET Core Identity
// ==============================================================================

builder.Services.AddIdentity<ApplicationUser, IdentityRole>() // استفاده از ApplicationUser سفارشی
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// تنظیمات سخت‌گیری رمز عبور (اختیاری)
builder.Services.Configure<IdentityOptions>(options =>
{
    // تنظیمات برای توسعه (Development) - رمزهای عبور ساده را مجاز می‌کند
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;

    // تنظیمات نام کاربری (برای استفاده از شماره موبایل به جای ایمیل)
    options.User.RequireUniqueEmail = false;
    options.User.AllowedUserNameCharacters = null; // برای جلوگیری از محدودیت در شماره موبایل
});


// ==============================================================================
// 💡 ۳. پیکربندی JWT Authentication
// ==============================================================================

// بازیابی کلید مخفی JWT از تنظیمات (مانند appsettings.json)
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YOUR_LONG_AND_SECURE_SECRET_KEY_MIN_16_CHARS";

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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});


// ==============================================================================
// 💡 ۴. تعریف سیاست CORS
// ==============================================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          // 🚨 برای توسعه (Development): اجازه دسترسی به همه مبدأها، هدرها و متدها
                          policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // 💡 ایجاد و اعمال خودکار Migration در زمان توسعه (اختیاری اما مفید)
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
}

app.UseHttpsRedirection();

// 💡 ۵. استفاده از Static Files برای سرویس‌دهی فایل‌های HTML, JS, CSS از wwwroot
app.UseStaticFiles();

// 💡 ۶. استفاده از سیاست CORS (باید قبل از Authorization و MapControllers باشد)
app.UseCors(MyAllowSpecificOrigins);

// 💡 ۷. استفاده از احراز هویت (Authentication)
// این خط برای فعال کردن JWT و Identity ضروری است و باید قبل از UseAuthorization باشد.
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();