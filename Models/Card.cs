using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SharpMinded.Models;

[Table("cards")]
public class Card : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [PrimaryKey("deck_id")]
    public int DeckId { get; set; }

    [Column("user_id")]
    public string? UserId { get; set; }

    [Column("front")]
    public string? Front { get; set; }

    [Column("back")]
    public string? Back { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; }

    // ---- Spaced repetition state (SM-2) ----

    /// <summary>Easiness factor. Default 2.5, never goes below 1.3.</summary>
    [Column("easiness_factor")]
    public double EasinessFactor { get; set; } = 2.5;

    /// <summary>Consecutive successful reviews (reset to 0 on a failed review).</summary>
    [Column("repetitions")]
    public int Repetitions { get; set; }

    /// <summary>Current interval, in days, until the next review.</summary>
    [Column("interval_days")]
    public int IntervalDays { get; set; }

    /// <summary>UTC timestamp of the next scheduled review. Null = never reviewed (due now).</summary>
    [Column("next_review_at")]
    public DateTime? NextReviewAt { get; set; }

    /// <summary>UTC timestamp of the most recent review.</summary>
    [Column("last_reviewed_at")]
    public DateTime? LastReviewedAt { get; set; }
}
