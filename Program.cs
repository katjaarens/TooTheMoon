using Microsoft.EntityFrameworkCore;
using TooTheMoon.Data;

var builder = WebApplication.CreateBuilder(args);

// Datenbank-Verbindung
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=wedding.db"));

// Session-Konfiguration HIER nach oben verschoben:
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromHours(2); // Bleibt 2 Stunden eingeloggt
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Session-Middleware aktivieren (muss vor UseRouting / MapControllerRoute stehen)
app.UseSession();

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

// Datenbank beim Start automatisch erstellen falls nicht vorhanden
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();