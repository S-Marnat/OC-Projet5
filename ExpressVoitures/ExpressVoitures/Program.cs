using ExpressVoitures.Data;
using ExpressVoitures.Interfaces;
using ExpressVoitures.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
})
.AddCookie(IdentityConstants.ApplicationScheme);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IFinitionService, FinitionService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IMarqueService, MarqueService>();
builder.Services.AddScoped<IModeleService, ModeleService>();
builder.Services.AddScoped<IReparationService, ReparationService>();
builder.Services.AddScoped<IVenteService, VenteService>();
builder.Services.AddScoped<IVoitureService, VoitureService>();

var app = builder.Build();

// Rôle Administrateur
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Créer le rôle Administrateur
    if (!await roleManager.RoleExistsAsync("Administrateur"))
    {
        await roleManager.CreateAsync(new IdentityRole("Administrateur"));
    }

    // Créer un admin local si besoin
    var adminEmail = "admin@expressvoitures.com";
    var adminPassword = "Password123$";

    var user = await userManager.FindByEmailAsync(adminEmail);

    if (user == null)
    {
        user = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(user, adminPassword);
    }

    // Ajouter l’utilisateur au rôle
    if (!await userManager.IsInRoleAsync(user, "Administrateur"))
    {
        await userManager.AddToRoleAsync(user, "Administrateur");
    }
}

if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Créer le rôle Administrateur
    if (!await roleManager.RoleExistsAsync("Administrateur"))
    {
        await roleManager.CreateAsync(new IdentityRole("Administrateur"));
    }

    // Créer un admin local si besoin
    var adminEmail = Environment.GetEnvironmentVariable("ROOT_EMAIL");
    var adminPassword = Environment.GetEnvironmentVariable("ROOT_PASSWORD");

    var user = await userManager.FindByEmailAsync(adminEmail);

    if (user == null)
    {
        user = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(user, adminPassword);
    }

    // Ajouter l’utilisateur au rôle
    if (!await userManager.IsInRoleAsync(user, "Administrateur"))
    {
        await userManager.AddToRoleAsync(user, "Administrateur");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
