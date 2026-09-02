// Models/Guest.cs
public class Guest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsAttending { get; set; }
    public bool HasPlusOne { get; set; }
    public int ChildrenCount { get; set; }
    public string FoodPreference { get; set; } = string.Empty; // z.B. Allesesser, Vegetarisch
    public string Allergies { get; set; } = string.Empty;
    public string SongRequest { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}