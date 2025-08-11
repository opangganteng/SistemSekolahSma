using Microsoft.AspNetCore.Authentication.Cookies;
using SistemSekolahSMA.Data;
using SistemSekolahSMA.Data.Repositories;
using SistemSekolahSMA.Services;
using SistemSekolahSMA.Repositories; // NEW: Add Analytics repositories namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database Connection Factory
builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

// Repositories - SEMUA REPOSITORY (including ANALYTICS)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGuruRepository, GuruRepository>();
builder.Services.AddScoped<IKelasRepository, KelasRepository>();
builder.Services.AddScoped<IMataPelajaranRepository, MataPelajaranRepository>();
builder.Services.AddScoped<SiswaRepository, SiswaRepository>();
builder.Services.AddScoped<IJadwalRepository, JadwalRepository>();
builder.Services.AddScoped<IPresensiSiswaRepository, PresensiSiswaRepository>();
builder.Services.AddScoped<IPresensiGuruRepository, PresensiGuruRepository>();
builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

// Services - SEMUA SERVICE (including ANALYTICS)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGuruService, GuruService>();
builder.Services.AddScoped<ISiswaService, SiswaService>();
builder.Services.AddScoped<IKelasService, KelasService>();
builder.Services.AddScoped<IMataPelajaranService, MataPelajaranService>();
builder.Services.AddScoped<IJadwalService, JadwalService>();
builder.Services.AddScoped<IPresensiService, PresensiService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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