using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TooTheMoon.Data;

var builder = WebApplication.CreateBuilder(args);

// Render-Port verwenden
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

Environment.SetEnvironmentVariable(
    "ASPNETCORE_URLS",
    $"http://+:{port}");

// PostgreSQL-Verbindung aus Render-Environment-Variable
var connectionString =
    builder.Configuration["ConnectionStrings:DefaultConnection"];

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Die ConnectionStrings:DefaultConnection wurde nicht gefunden.");
}

// Datenbank konfigurieren
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.CommandTimeout(60);

        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
    });
});


// MVC und Razor Views
builder.Services.AddControllersWithViews();

// Data-Protection-Schlüssel dauerhaft speichern
//
// Bei Render muss /var/data als Persistent Disk eingebunden sein.
var dataProtectionKeysPath =
    "/var/data/dataprotection-keys";

Directory.CreateDirectory(dataProtectionKeysPath);

builder.Services
    .AddDataProtection()
    .PersistKeysToFileSystem(
        new DirectoryInfo(dataProtectionKeysPath))
    .SetApplicationName("TooTheMoonWeddingApp");

// Session-Speicher
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".TooTheMoon.Session.v2";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy =
        CookieSecurePolicy.SameAsRequest;

    options.IdleTimeout =
        TimeSpan.FromMinutes(30);
});

var app = builder.Build();

// Weitergeleitete Header von Render verarbeiten
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
};

forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardedHeadersOptions);

// Statische Dateien
app.UseStaticFiles();

// Routing
app.UseRouting();

// Session
app.UseSession();

// Autorisierung
app.UseAuthorization();

// Standardroute
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
