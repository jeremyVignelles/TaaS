using Microsoft.AspNetCore.Authentication.Cookies;
using TaaS.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(cookieOptions =>
    {
        cookieOptions.SlidingExpiration = true;
        cookieOptions.ExpireTimeSpan = TimeSpan.FromDays(31);
        cookieOptions.Cookie.IsEssential = true;
        cookieOptions.Cookie.MaxAge = TimeSpan.FromDays(31);
    });
builder.Services.AddSingleton<TimelapseDataAccess>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
