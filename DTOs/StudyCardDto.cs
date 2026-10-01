using SharpMinded.Models;

namespace SharpMinded.DTOs;

/// <summary>Card as presented during a study session, including its scheduling state.</summary>
public class StudyCardDto
{
    public int Id { get; set; }
    public int DeckId { get; set; }
    public string? Front { get; set; }
    public string? Back { get; set; }
    public double EasinessFactor { get; set; }
    public int Repetitions { get; set; }
    public int IntervalDays { get; set; }
    public DateTime? NextReviewAt { get; set; }

    public StudyCardDto(Card card)
    {
        Id = card.Id;
        DeckId = card.DeckId;
        Front = card.Front;
        Back = card.Back;
        EasinessFactor = card.EasinessFactor;
        Repetitions = card.Repetitions;
        IntervalDays = card.IntervalDays;
        NextReviewAt = card.NextReviewAt;
    }
}
