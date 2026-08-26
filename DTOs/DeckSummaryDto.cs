using SharpMinded.Models;

namespace SharpMinded.DTOs;

public class DeckSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int UserId { get; }
    public DateTime CreatedAt { get; }

    public DeckSummaryDto(Deck deck)
    {
        Id = deck.Id;
        Name = deck.Name;
        Description = deck.Description;
        UserId = deck.Id;
        CreatedAt = deck.CreatedAt;
    }
}