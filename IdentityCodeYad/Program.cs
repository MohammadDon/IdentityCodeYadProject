using IdentityCodeYad.Data;
using IdentityCodeYad.Tools;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        // User Options
        options.User.RequireUniqueEmail = true;
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789+";
        // Signin Options
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = true;
        // Password Options
        options.Password.RequireUppercase = false;
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireLowercase = true;
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;
        // LockOut
        options.Lockout.AllowedForNewUsers = false;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
        options.Lockout.MaxFailedAccessAttempts = 3;
        // Stores Options
        //options.Stores.MaxLengthForKeys = 10;
        options.Stores.ProtectPersonalData = false;

        //options.Tokens.AuthenticatorTokenProvider = "";

        //options.ClaimsIdentity.UserNameClaimType = "ClaimTypes.Name";
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<PersianIdentityErrors>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/LogOut";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(3);
});

#region Services

builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();

#endregion

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
