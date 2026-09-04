using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TooTheMoon.Data;
using TooTheMoon.Models;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace TooTheMoon.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult RsvpForm()
    {
        return View();
    }

    public IActionResult Trauzeugen()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitRsvp(string name, string email, string attending, bool hasCompanion, string children, string diet, int meatCount, int veggieCount, int veganCount, string foodIntolerances, string songRequest, string messageToCouple)
    {
        // 1. Zusage prüfen (unterstützt "Ja", "yes", "true" etc.)
        bool isAttending = !string.IsNullOrEmpty(attending) && 
                           (attending.Equals("Ja", StringComparison.OrdinalIgnoreCase) || 
                            attending.Equals("true", StringComparison.OrdinalIgnoreCase) || 
                            attending.Equals("yes", StringComparison.OrdinalIgnoreCase));

        // 2. Erwachsene und Kinder getrennt berechnen
        int adultsCount = 1; // Hauptgast
        if (hasCompanion)
        {
            adultsCount += 1;
        }

        int childrenCount = 0;
        if (!string.IsNullOrEmpty(children))
        {
            if (children.Contains("1"))
            {
                childrenCount = 1;
            }
            else if (children.Contains("2"))
            {
                childrenCount = 2;
            }
            else if (children.Contains("3") || children.Contains("+"))
            {
                childrenCount = 3; 
            }
        }

        // Gesamtsumme der Personen
        int totalPersons = isAttending ? (adultsCount + childrenCount) : 1;

        // Falls abgesagt wurde, Zähler auf 0 bzw. Standard setzen
        if (!isAttending)
        {
            adultsCount = 0;
            childrenCount = 0;
            totalPersons = 1;
            meatCount = 0;
            veggieCount = 0;
            veganCount = 0;
        }

        // Gast-Objekt erstellen inklusive der Catering-Zähler
        var guest = new RsvpGuest
        {
            Name = name,
            Email = email,
            IsAttending = isAttending,
            AdultsCount = adultsCount,
            ChildrenCount = childrenCount,
            NumberOfGuests = totalPersons, 
            DietaryNotes = diet,
            MeatCount = meatCount,
            VeggieCount = veggieCount,
            VeganCount = veganCount,
            FoodIntolerances = foodIntolerances,
            SongRequest = songRequest,
            MessageToCouple = messageToCouple,
            CreatedAt = DateTime.UtcNow
        };

        // In die Datenbank schreiben
        _context.RsvpGuests.Add(guest);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("ThankYou");
    }

    // --- Admin Login (GET) ---
    [HttpGet]
    public IActionResult AdminLogin()
    {
        return View();
    }

    // --- Admin Login (POST) mit Namensprüfung für Katja, Andrea, Heike & Lulu ---
    [HttpPost]
    public IActionResult AdminLogin(string name, string passcode)
    {
        // Berechtigte Personen
        var allowedNames = new[] { "Andrea", "Katja", "Heike", "Lulu" };

        bool isValidName = allowedNames.Any(n => n.Equals(name?.Trim(), StringComparison.OrdinalIgnoreCase));
        string correctPassword = "ToTheMoon2027!"; // Euer gemeinsames Passwort

        if (isValidName && passcode == correctPassword)
        {
            HttpContext.Session.SetString("IsAdmin", "True");
            HttpContext.Session.SetString("AdminName", name.Trim());
            return RedirectToAction("AdminGuests");
        }

        ModelState.AddModelError("", "Ungültiger Name oder falsches Passwort!");
        return View();
    }

    // Admin-Ansicht zum Auslesen aller Gäste (abgesichert)
    public async Task<IActionResult> AdminGuests()
    {
        if (HttpContext.Session.GetString("IsAdmin") != "True")
        {
            return RedirectToAction("AdminLogin");
        }

        var guests = await _context.RsvpGuests.OrderByDescending(g => g.CreatedAt).ToListAsync();
        return View(guests);
    }

    // --- Gast löschen ---
    [HttpPost]
    public async Task<IActionResult> DeleteGuest(int id)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "True")
        {
            return RedirectToAction("AdminLogin");
        }

        var guest = await _context.RsvpGuests.FindAsync(id);
        if (guest != null)
        {
            _context.RsvpGuests.Remove(guest);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("AdminGuests");
    }

    // --- Sitzplan-Verwaltung (Timeout-optimiert) ---
    [HttpGet]
    public async Task<IActionResult> AdminSeatingPlanner()
    {
        if (HttpContext.Session.GetString("IsAdmin") != "True")
        {
            return RedirectToAction("AdminLogin");
        }

        // Getrennte, schnelle Abfragen statt schwerer Joins
        var tables = await _context.WeddingTables.ToListAsync();
        var allGuests = await _context.RsvpGuests.ToListAsync();

        ViewBag.AttendingGuests = allGuests.Where(g => g.IsAttending).ToList();

        foreach (var table in tables)
        {
            table.Guests = allGuests.Where(g => g.WeddingTableId == table.Id).ToList();
        }

        return View(tables);
    }

    [HttpPost]
    public async Task<IActionResult> AddTable(string tableName, int capacity)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "True")
        {
            return RedirectToAction("AdminLogin");
        }

        if (!string.IsNullOrWhiteSpace(tableName))
        {
            var table = new WeddingTable
            {
                Name = tableName.Trim(),
                Capacity = capacity > 0 ? capacity : 8
            };
            _context.WeddingTables.Add(table);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("AdminSeatingPlanner");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTable(int id)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "True")
        {
            return RedirectToAction("AdminLogin");
        }

        var table = await _context.WeddingTables
            .Include(t => t.Guests)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (table != null)
        {
            foreach (var guest in table.Guests)
            {
                guest.WeddingTableId = null;
            }

            _context.WeddingTables.Remove(table);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("AdminSeatingPlanner");
    }

    [HttpPost]
    public async Task<IActionResult> AssignGuestToTable(int guestId, int? tableId)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "True")
        {
            return RedirectToAction("AdminLogin");
        }

        var guest = await _context.RsvpGuests.FindAsync(guestId);
        if (guest != null)
        {
            guest.WeddingTableId = (tableId.HasValue && tableId.Value > 0) ? tableId.Value : (int?)null;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("AdminSeatingPlanner");
    }

    // --- Logout ---
    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("AdminLogin");
    }

    public IActionResult ThankYou()
    {
        return View();
    }

    public IActionResult Schedule()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}