using eShopLegacyMVC.Data;
using eShopLegacyMVC.Models;
using eShopLegacyMVC.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("CatalogDBContext") ?? 
    throw new InvalidOperationException("Connection string 'CatalogDBContext' not found.");
builder.Services.AddDbContext<CatalogDBContext>(options =>
    options.UseSqlServer(connectionString));

var identityConnectionString = builder.Configuration.GetConnectionString("IdentityDBContext") ?? 
    throw new InvalidOperationException("Connection string 'IdentityDBContext' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(identityConnectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<FileServiceConfiguration>();
builder.Services.AddScoped<WeatherService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

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

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Catalog}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CatalogDBContext>();
    
    var useMockData = app.Configuration.GetValue<bool>("AppSettings:UseMockData");
    if (!useMockData)
    {
        context.Database.Migrate();
        SeedData.Initialize(services);
    }
}

app.Run();
