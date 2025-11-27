using Microsoft.EntityFrameworkCore;
using WebPortal.Controllers;
using WebPortal.CustomMiddleware;
using WebPortal.DbStuff;
using WebPortal.DbStuff.Repositories;
using WebPortal.DbStuff.Repositories.Interfaces;
using WebPortal.Hubs;
using WebPortal.Services;
using WebPortal.Services.Apis;
using WebPortal.Services.AutoRegistrationInDI;
using WebPortal.Services.BackgroudServices;
using WebPortal.Services.Permissions;
using WebPortal.Services.Permissions.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpLogging(opt => opt.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All);
builder.Logging.AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Information);

builder.Services
    .AddAuthentication(AuthController.AUTH_KEY)
    .AddCookie(AuthController.AUTH_KEY, o =>
    {
        o.LoginPath = "/Auth/Login";
        o.ForwardForbid = "/Auth/Forbid ";
    });

// Register db context
var connectionString = builder.Configuration.GetConnectionString("DefaultDbConnection")!;

builder.Services.AddDbContext<WebPortalContext>(
    x => x.UseSqlServer(connectionString)
    );

// Register Repositories
builder.Services.AddScoped<IUserRepositrory, UserRepositrory>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ISourcePDFService, SourcePDFService>();

//CompShop
builder.Services.AddScoped<ICompShopFileService, CompShopFileService>(); 
builder.Services.AddScoped<ICompShopPermission, CompShopPermission>();

builder.Services.AddScoped<IGirlPermission, GirlPermission>();

builder.Services.AddHttpClient<WaifuApi>(x=>
{
    x.BaseAddress = new Uri("https://api.waifu.im");
});

builder.Services.AddHttpClient<WeatherApi>(x=>
{
    x.BaseAddress = new Uri("https://api.open-meteo.com");
});

builder.Services.AddHttpClient<JokeApi>(x=>
{
    x.BaseAddress = new Uri("https://official-joke-api.appspot.com");
});

builder.Services.AddHttpClient<CatsApi>(x =>
{
    x.BaseAddress = new Uri("https://cataas.com");
});

builder.Services.AddHttpClient<IssApi>(client =>
{
    client.BaseAddress = new Uri("https://api.wheretheiss.at/");
});

builder.Services.AddHttpClient<WikiPageApi>(client =>
{
    client.BaseAddress = new Uri("https://en.wikipedia.org/");
    client.DefaultRequestHeaders.Add("User-Agent", "WebPortalApp/1.0");
});

var authResolver = new AutoRegisterService();
authResolver.RegisterAllRepositories(builder.Services);
authResolver.RegisterAllByAttribute(builder.Services);

builder.Services.AddScoped<SeedService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHostedService<AvatarCleaneaper>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seed = scope.ServiceProvider.GetRequiredService<SeedService>();
    seed.Seed();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment() 
    && Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower() != "development")
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Who am I?
app.UseAuthentication();
// What can I do?
app.UseAuthorization();

app.UseMiddleware<CustomLocalizationMiddleware>();


app.MapHub<NotificationHub>("/hubs/notifaction");

// Enable attribute-routed API controllers like /api/CatalogApi
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
