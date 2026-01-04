using Coworking.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CoworkingDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuration d'Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configuration des mots de passe
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // Configuration de l'utilisateur
    options.User.RequireUniqueEmail = true;

    // Configuration de la connexion
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<CoworkingDbContext>()
.AddDefaultTokenProviders();

// Ajouter RoleManager
builder.Services.AddScoped<RoleManager<IdentityRole>>();

// Ajouter le service de rappels
builder.Services.AddScoped<Coworking.Infrastructure.Services.ReminderService>();

// Ajouter le service en arrière-plan pour les rappels
builder.Services.AddHostedService<Coworking.Web.BackgroundServices.ReminderBackgroundService>();

// Ajouter le service PDF
builder.Services.AddScoped<Coworking.Web.Services.PdfService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// Initialiser les rôles et l'admin par défaut
using (var scope = app.Services.CreateScope())
{
    await Coworking.Infrastructure.Services.RoleInitializer.InitializeAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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


app.Run();
