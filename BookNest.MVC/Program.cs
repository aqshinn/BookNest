using BookNest.Data.Data;
using Microsoft.EntityFrameworkCore;
using BookNest.Business.Interfaces;
using BookNest.Business.Services;
using Microsoft.AspNetCore.Identity;
using BookNest.Core.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;

    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Configure authentication options
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(20);

    // Important: Add dynamic redirect based on the area being accessed
    options.Events.OnRedirectToLogin = context =>
    {
        // If user tries to access /AdminPanel, redirect to admin login
        if (context.Request.Path.StartsWithSegments("/AdminPanel"))
        {
            // Send them to admin login page with return URL
            context.Response.Redirect("/Account/AdminLogin?ReturnUrl=" + context.Request.Path + context.Request.QueryString);
        }
        else
        {
            // For other areas, use standard login page
            context.Response.Redirect(context.RedirectUri);
        }
        return Task.CompletedTask;
    };
});


builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IFileService, FileService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    // 1. Create roles if they do not exist.
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    if (!await roleManager.RoleExistsAsync("Member"))
        await roleManager.CreateAsync(new IdentityRole("Member"));

    // 2. Admin your own test account (Replace the email below with your registered email!)
    var adminEmail = "superadmin@example.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}


app.Run();
