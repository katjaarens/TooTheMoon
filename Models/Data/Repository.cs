using TooTheMoon.Models;

namespace TooTheMoon.Data;

public static class Repository
{
    private static List<GuestResponse> responses = new();
    public static IEnumerable<GuestResponse> Responses => responses;

    public static void AddResponse(GuestResponse r)
    {
        responses.Add(r);
    }
}
