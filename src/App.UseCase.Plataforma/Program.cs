using app.plataforma;
using app.plataforma.Handlers;
using app.plataforma.Models;
using app.plataforma.Services;
using App.UseCase.Plataforma.Services;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbGenericRepository;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection(nameof(MongoDBSettings)));


builder.Services.AddSingleton<MongoDBContext>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoDBContext(settings.ConnectionString, settings.DatabaseName);
});

builder.Services.AddSingleton<IMongoClient>(_ => {
    var connectionString =
        builder
            .Configuration
            .GetSection("MongoDBSettings:ConnectionString")?
            .Value;
    return new MongoClient(connectionString);
});

//inject
builder.Services.AddScoped<IPostagensService, PostagensService>();
builder.Services.AddScoped<IFavoritosService, FavoritosService>();
builder.Services.AddScoped<IUsuarioPostagem, UsuarioPostagem>();




var mongoDBSettings = builder.Configuration.GetSection("MongoDBSettings").Get<MongoDBSettings>();





builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});


builder.Services.AddSingleton<MongoDBContext>(serviceProvider =>
{
    return new MongoDBContext(mongoDBSettings.ConnectionString, mongoDBSettings.DatabaseName);
});

//builder.Services
//    .AddIdentityCore<MongoUser>()
//    .AddRoles<MongoRole>()
//    .AddMongoDbStores<MongoUser, MongoRole, ObjectId>(mongo =>
//    {
//        mongo.ConnectionString = mongoDBSettings.ConnectionString;
//        // other options
//    })
//    .AddDefaultTokenProviders();




builder.Services.AddIdentityMongoDbProvider<ApplicationUser>(identity =>
{
    identity.Password.RequireDigit = false;
    identity.Password.RequireLowercase = false;
    identity.Password.RequireNonAlphanumeric = false;
    identity.Password.RequireUppercase = false;
    identity.Password.RequiredLength = 1;
    identity.Password.RequiredUniqueChars = 0;

    identity.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
    identity.Lockout.MaxFailedAccessAttempts = 5;
    identity.Lockout.AllowedForNewUsers = true;
    // User settings.
    //identity.User.AllowedUserNameCharacters ="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    identity.User.AllowedUserNameCharacters = "";
    identity.User.RequireUniqueEmail = true;

},
    mongo =>
    {
        mongo.ConnectionString = mongoDBSettings.ConnectionString;

    }

);


builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddRoles<ApplicationRole>()
    .AddMongoDbStores<ApplicationUser, ApplicationRole, ObjectId>(mongo =>
    {
        mongo.ConnectionString = mongoDBSettings.ConnectionString;
        // other options
    });
  

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
