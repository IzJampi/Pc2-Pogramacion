using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    options.SignIn.RequireConfirmedAccount = false; // más simple para pruebas
})
.AddRoles<IdentityRole>() // 🔥 IMPORTANTE
.AddEntityFrameworkStores<ApplicationDbContext>();

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

app.UseAuthentication(); // 🔥 IMPORTANTE
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();


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