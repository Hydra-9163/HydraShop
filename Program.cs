using Microsoft.EntityFrameworkCore;
using MachineShopManager.Data;
using Microsoft.AspNetCore.Identity;
using MachineShopManager.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Uploads de até 100 MB por projeto (soma de todos os arquivos) + 5 MB de folga para os campos do formulário.
// Fica aqui (e não por atributo) porque o antiforgery lê o formulário antes dos atributos de limite.
const long maxRequestBodyBytes = 105L * 1024 * 1024;
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = maxRequestBodyBytes);
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = maxRequestBodyBytes;
    options.ValueCountLimit = int.MaxValue; // sem limite de quantidade de arquivos
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    // O projeto possui uma tela de login própria; as rotas padrão do Identity
    // (Account/Login e Account/AccessDenied) não existem nesta aplicação.
    options.LoginPath = "/Operator/Login";
    options.AccessDeniedPath = "/Home/Index";
});

builder.Services.AddAntiforgery(options =>
{
    // Permite validar chamadas fetch autenticadas sem abrir mão de CSRF.
    options.HeaderName = "RequestVerificationToken";
});

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

// Arquivos enviados em tempo de execução (wwwroot/uploads) precisam do UseStaticFiles; o MapStaticAssets
// só conhece o que existia no build. Extensões de CAD/G-code não têm MIME padrão, então senão dariam 404.
var contentTypes = new FileExtensionContentTypeProvider();
foreach (var ext in new[] { ".stl", ".obj", ".step", ".stp", ".iges", ".igs", ".gcode" })
    contentTypes.Mappings[ext] = "application/octet-stream";
app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = contentTypes });

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Aplicar migrations do Entity Framework Core
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

// Executar IdentitySeeder após as migrations
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();