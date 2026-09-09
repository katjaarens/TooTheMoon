using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using TooTheMoon.Data;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.DataProtection;

Environment.SetEnvironmentVariable("DOTNET_USE_POLLING_FILE_WATCHER", "1");

// Render-Port abfangen und Kestrel über die ASPNETCORE_URLS Variable steuern
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
Environment.SetEnvironmentVariable("ASPNETCORE_URLS", $"http://+:{port}");

var builder = WebApplication.CreateBuilder(args);

// Datenbank (mit PostgreSQL / Npgsql für Supabase)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// MVC + Razor Views
builder.Services.AddControllersWithViews();

// Data Protection absichern, damit Container-Neustarts Cookies nicht ungültig machen
builder.Services.AddDataProtection()
    .SetApplicationName("TooTheMoonWeddingApp");

// Session aktivieren & Cookie-Sicherheit für Render-Proxy anpassen
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

var app = builder.Build();

// WICHTIG: Forwarded Headers für Render aktivieren, damit HTTPS korrekt erkannt wird
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();