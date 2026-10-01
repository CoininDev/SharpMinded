using System.ComponentModel.DataAnnotations;

namespace SharpMinded.DTOs;

public class ReviewCardRequest
{
    [Required]
    public int DeckId { get; set; }

    [Required]
    public int CardId { get; set; }

    /// <summary>Recall quality: 5 = perfect ... 0 = blackout. Below 3 counts as a failure.</summary>
    [Required]
    [Range(0, 5)]
    public int Quality { get; set; }
}
