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
            CreatedAt = DateTime.Now
        };

        // In die SQLite-Datenbank schreiben
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
        string correctPassword = "ToTheMoon2027!"; // Hier euer gemeinsames Passwort

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

    // --- Logout ---
    [HttpPost]
    public IActionResult Logout()
    {
        // Session komplett leeren
        HttpContext.Session.Clear();
        
        // Zurück zur Admin-Login-Seite umleiten
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