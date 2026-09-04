using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TooTheMoon.Data;
using TooTheMoon.Models;

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
    public async Task<IActionResult> SubmitRsvp(
        string name,
        string email,
        string attending,
        bool hasCompanion,
        string children,
        string diet,
        int meatCount,
        int veggieCount,
        int veganCount,
        string foodIntolerances,
        string songRequest,
        string messageToCouple)
    {
        bool isAttending =
            !string.IsNullOrEmpty(attending) &&
            (
                attending.Equals("Ja", StringComparison.OrdinalIgnoreCase) ||
                attending.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                attending.Equals("yes", StringComparison.OrdinalIgnoreCase)
            );

        int adultsCount = 1;

        if (hasCompanion)
        {
            adultsCount++;
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

        int totalPersons = isAttending
            ? adultsCount + childrenCount
            : 1;

        if (!isAttending)
        {
            adultsCount = 0;
            childrenCount = 0;
            totalPersons = 1;
            meatCount = 0;
            veggieCount = 0;
            veganCount = 0;
        }

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

        _context.RsvpGuests.Add(guest);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(ThankYou));
    }

    [HttpGet]
    public IActionResult AdminLogin()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AdminLogin(string name, string passcode)
    {
        var allowedNames = new[]
        {
            "Andrea",
            "Katja",
            "Heike",
            "Lulu"
        };

        bool isValidName = allowedNames.Any(n =>
            n.Equals(name?.Trim(), StringComparison.OrdinalIgnoreCase));

        string? correctPassword =
            Environment.GetEnvironmentVariable("ToTheMoon2027!");

        bool isValidPassword =
            !string.IsNullOrEmpty(correctPassword) &&
            passcode == correctPassword;

        if (isValidName && isValidPassword)
        {
            HttpContext.Session.SetString("IsAdmin", "True");
            HttpContext.Session.SetString("AdminName", name!.Trim());

            return RedirectToAction(nameof(AdminGuests));
        }

        ModelState.AddModelError(
            string.Empty,
            "Ungültiger Name oder falsches Passwort!");

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> AdminGuests()
    {
        if (!IsAdmin())
        {
            return RedirectToAction(nameof(AdminLogin));
        }

        var guests = await _context.RsvpGuests
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();

        return View(guests);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteGuest(int id)
    {
        if (!IsAdmin())
        {
            return RedirectToAction(nameof(AdminLogin));
        }

        var guest = await _context.RsvpGuests.FindAsync(id);

        if (guest != null)
        {
            _context.RsvpGuests.Remove(guest);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(AdminGuests));
    }

    [HttpGet]
    public async Task<IActionResult> AdminSeatingPlanner()
    {
        if (!IsAdmin())
        {
            return RedirectToAction(nameof(AdminLogin));
        }

        try
        {
            Console.WriteLine("AdminSeatingPlanner gestartet");

            Console.WriteLine("Lade WeddingTables...");
            var tables = await _context.WeddingTables
                .AsNoTracking()
                .ToListAsync();

            Console.WriteLine($"Tische geladen: {tables.Count}");

            Console.WriteLine("Lade RsvpGuests...");
            var allGuests = await _context.RsvpGuests
                .AsNoTracking()
                .ToListAsync();

            Console.WriteLine($"Gäste geladen: {allGuests.Count}");

            ViewBag.AttendingGuests = allGuests
                .Where(g => g.IsAttending)
                .ToList();

            foreach (var table in tables)
            {
                table.Guests = allGuests
                    .Where(g => g.WeddingTableId == table.Id)
                    .ToList();
            }

            Console.WriteLine("View wird geladen");

            return View(tables);
        }
        catch (Exception ex)
        {
            Console.WriteLine("FEHLER IM SITZPLAN:");
            Console.WriteLine(ex.ToString());

            return StatusCode(500, ex.ToString());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTable(
        string tableName,
        int capacity)
    {
        if (!IsAdmin())
        {
            return RedirectToAction(nameof(AdminLogin));
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

        return RedirectToAction(nameof(AdminSeatingPlanner));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTable(int id)
    {
        if (!IsAdmin())
        {
            return RedirectToAction(nameof(AdminLogin));
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

        return RedirectToAction(nameof(AdminSeatingPlanner));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignGuestToTable(
        int guestId,
        int? tableId)
    {
        if (!IsAdmin())
        {
            return RedirectToAction(nameof(AdminLogin));
        }

        var guest = await _context.RsvpGuests
            .FirstOrDefaultAsync(g => g.Id == guestId);

        if (guest != null)
        {
            guest.WeddingTableId =
                tableId.HasValue && tableId.Value > 0
                    ? tableId.Value
                    : null;

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(AdminSeatingPlanner));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction(nameof(AdminLogin));
    }

    public IActionResult ThankYou()
    {
        return View();
    }

    public IActionResult Schedule()
    {
        return View();
    }

    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = HttpContext.TraceIdentifier
        });
    }

    private bool IsAdmin()
    {
        return HttpContext.Session.GetString("IsAdmin") == "True";
    }
}
