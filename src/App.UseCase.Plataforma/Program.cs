using app.plataforma;
using app.plataforma.Handlers;
using app.plataforma.Handlers.Hubs;
using app.plataforma.Identity;
using app.plataforma.Interfaces;
using app.plataforma.Services;
using AspNetCore.Identity.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 50485760; // 10MB em bytes
});

builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 502400000; // tamanho máximo em bytes
});


builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection(nameof(MongoDBSettings)));


builder.Services.AddSingleton<MongoDBContext>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoDBContext(settings.ConnectionString, settings.DatabaseName);
});

builder.Services.AddSingleton<IMongoClient>(_ =>
{
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

//3 elementos
builder.Services.AddSingleton<List<User>>();
builder.Services.AddSingleton<List<Connection>>();
builder.Services.AddSingleton<List<Call>>();


var mongoDBSettings = builder.Configuration.GetSection("MongoDBSettings").Get<MongoDBSettings>();


builder.Services.AddIdentityMongoDbProvider<ApplicationUser>(identity =>
{
    identity.Password.RequireDigit = false;
    identity.Password.RequireLowercase = false;
    identity.Password.RequireNonAlphanumeric = false;
    identity.Password.RequireUppercase = false;
    identity.Password.RequiredLength = 1;
    identity.Password.RequiredUniqueChars = 0;

    identity.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseStreamSocket();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<LiveHub>("/live", options =>
    {
        options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
    });
});

app.Run();
