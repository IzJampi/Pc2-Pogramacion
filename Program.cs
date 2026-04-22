using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Pc2_Pogramacion.Data;

var builder = WebApplication.CreateBuilder(args);

// 🔹 DB + SQLITE
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 🔹 IDENTITY + ROLES
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// 🔥 REDIS CACHE
builder.Services.AddStackExchangeRedisCache(options =>
{
    var redis = builder.Configuration["Redis__ConnectionString"];

    if (!string.IsNullOrEmpty(redis))
    {
        options.Configuration = redis;
    }
    else
    {
        options.Configuration = "localhost:6379"; // fallback
    }
});

// 🔥 SESIONES
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 🔹 PIPELINE
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// 🔥 SESIÓN (ANTES de Auth)
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// 🔹 SEED: ROL + USUARIO COORDINADOR
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    string role = "Coordinador";

    if (!await roleManager.RoleExistsAsync(role))
        await roleManager.CreateAsync(new IdentityRole(role));

    var user = await userManager.FindByEmailAsync("admin@uni.com");

    if (user == null)
    {
        user = new IdentityUser
        {
            UserName = "admin@uni.com",
            Email = "admin@uni.com"
        };

        await userManager.CreateAsync(user, "Admin123!");
        await userManager.AddToRoleAsync(user, role);
    }
}

app.Run();