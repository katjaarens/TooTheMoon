using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TooTheMoon.Models;

public class WeddingTable
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty; // z.B. "Tisch 1", "Brauttisch", "Familie"
    
    public int Capacity { get; set; } = 8; // Maximale Anzahl an Plätzen

    // Verknüpfung zu den Gästen an diesem Tisch
    public ICollection<RsvpGuest> Guests { get; set; } = new List<RsvpGuest>();

    public int PosX { get; set; } = 100; // Prozent oder Pixel von links
public int PosY { get; set; } = 100; // Prozent oder Pixel von oben
}