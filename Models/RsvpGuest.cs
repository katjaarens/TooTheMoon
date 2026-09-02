using System;

namespace TooTheMoon.Models;

public class RsvpGuest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsAttending { get; set; }
    
    // Personenaufteilung
    public int AdultsCount { get; set; } = 1;
    public int ChildrenCount { get; set; } = 0;
    public int NumberOfGuests { get; set; } = 1;

    // Catering-Zähler
    public int MeatCount { get; set; } = 0;
    public int VeggieCount { get; set; } = 0;
    public int VeganCount { get; set; } = 0;

    // Ernährung & Wünsche
    public string? DietaryNotes { get; set; }
    public string? FoodIntolerances { get; set; }
    public string? SongRequest { get; set; }
    public string? MessageToCouple { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}