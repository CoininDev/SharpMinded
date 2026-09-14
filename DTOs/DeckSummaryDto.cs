using SharpMinded.Models;

namespace SharpMinded.DTOs;

public class DeckSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public DeckSummaryDto(Deck deck)
    {
        Id = deck.Id;
        Name = deck.Name;
        Description = deck.Description;
        UserId = deck.UserId;
        CreatedAt = deck.CreatedAt;
    }
}