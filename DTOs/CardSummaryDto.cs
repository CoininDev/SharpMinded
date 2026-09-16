using SharpMinded.Models;

namespace SharpMinded.DTOs;

public class CardSummaryDto
{
    public int Id { get; set; }
    public string? Front { get; set; } = string.Empty;

    public CardSummaryDto(Card card)
    {
        Id = card.Id;
        Front = card.Front.Substring(0, Math.Min(card.Front.Length, 50));
    }
}