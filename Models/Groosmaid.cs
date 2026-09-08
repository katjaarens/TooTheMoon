namespace TooTheMoon.Models
{
    public class Groomsmaid
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string RoleBadge { get; set; } = string.Empty; // z.B. "Hüter der Ringe"
        public string SinceWhen { get; set; } = string.Empty; // z.B. "Seit der Schulzeit"
        public string Speciality { get; set; } = string.Empty;
        public string FirstImpression { get; set; } = string.Empty;
        public string Anecdote { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty; // z.B. "/images/trauzeuge1.jpg"
    }
}