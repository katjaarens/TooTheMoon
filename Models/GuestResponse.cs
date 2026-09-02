namespace TooTheMoon.Models;

public class GuestResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool WillAttend { get; set; }
    public bool IsVegan { get; set; }
    public bool IsVegetarian { get; set; }
    public string FoodIntolerances { get; set; }
    public string? MessageToCouple { get; set; }
}
