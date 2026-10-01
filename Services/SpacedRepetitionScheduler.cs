using SharpMinded.Models;

namespace SharpMinded.Services;

/// <summary>
/// Implements the SM-2 spaced repetition algorithm (used by SuperMemo/Anki).
///
/// Quality scale (how well the answer was recalled):
///   5 = perfect, 4 = correct with hesitation, 3 = correct but hard,
///   2 = wrong but familiar, 1 = wrong, 0 = complete blackout.
/// A quality below 3 is a failure: the card is relearned and rescheduled for tomorrow.
/// </summary>
public static class SpacedRepetitionScheduler
{
    public const double DefaultEasinessFactor = 2.5;
    public const double MinEasinessFactor = 1.3;

    /// <summary>Registers a review and updates the card's scheduling state in place.</summary>
    /// <param name="card">Card being reviewed. Mutated by this method.</param>
    /// <param name="quality">Recall quality, 0–5.</param>
    /// <param name="reviewedAtUtc">Moment of the review, in UTC.</param>
    /// <returns>The same card, with updated scheduling fields.</returns>
    public static Card Review(Card card, int quality, DateTime reviewedAtUtc)
    {
        ArgumentNullException.ThrowIfNull(card);
        if (quality is < 0 or > 5)
            throw new ArgumentOutOfRangeException(nameof(quality), quality, "Quality must be between 0 and 5.");

        // 1. Update easiness factor: EF' = EF + (0.1 - (5-q) * (0.08 + (5-q) * 0.02))
        card.EasinessFactor = Math.Max(MinEasinessFactor,
            card.EasinessFactor + (0.1 - (5 - quality) * (0.08 + (5 - quality) * 0.02)));

        // 2. Failed recall: restart the learning cycle, review again tomorrow.
        if (quality < 3)
        {
            card.Repetitions = 0;
            card.IntervalDays = 1;
        }
        else
        {
            card.Repetitions += 1;
            card.IntervalDays = card.Repetitions switch
            {
                1 => 1,                      // first success: tomorrow
                2 => 6,                      // second success: 6 days
                _ => (int)Math.Round(card.IntervalDays * card.EasinessFactor) // then grow by EF
            };
        }

        card.LastReviewedAt = reviewedAtUtc;
        card.NextReviewAt = reviewedAtUtc.AddDays(card.IntervalDays);
        return card;
    }

    /// <summary>A card is due if it has never been reviewed or its next review is in the past.</summary>
    public static bool IsDue(Card card, DateTime utcNow) =>
        card.NextReviewAt is null || card.NextReviewAt <= utcNow;
}
