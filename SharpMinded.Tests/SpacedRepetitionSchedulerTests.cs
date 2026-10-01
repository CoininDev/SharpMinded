using SharpMinded.Models;
using SharpMinded.Services;
using Xunit;

namespace SharpMinded.Tests;

public class SpacedRepetitionSchedulerTests
{
    private static readonly DateTime Now = new DateTime(2026, 9, 30, 12, 0, 0, DateTimeKind.Utc);

    private static Card NewCard() => new Card
    {
        Id = 1,
        DeckId = 1,
        UserId = Guid.NewGuid().ToString(),
        Front = "front",
        Back = "back",
        EasinessFactor = SpacedRepetitionScheduler.DefaultEasinessFactor
    };

    // ---- Easiness factor ----

    [Fact]
    public void Perfect_Review_Increases_Easiness_Factor_By_0_1()
    {
        var card = NewCard();

        SpacedRepetitionScheduler.Review(card, 5, Now);

        Assert.Equal(2.6, card.EasinessFactor, precision: 6);
    }

    [Fact]
    public void Failed_Review_Decreases_Easiness_Factor()
    {
        var card = NewCard();

        SpacedRepetitionScheduler.Review(card, 1, Now);

        Assert.True(card.EasinessFactor < 2.5);
    }

    [Fact]
    public void Easiness_Factor_Never_Drops_Below_Minimum()
    {
        var card = NewCard();

        for (int i = 0; i < 10; i++)
            SpacedRepetitionScheduler.Review(card, 0, Now);

        Assert.Equal(SpacedRepetitionScheduler.MinEasinessFactor, card.EasinessFactor);
    }

    // ---- Repetitions and intervals ----

    [Fact]
    public void First_Successful_Review_Schedules_Next_Day()
    {
        var card = NewCard();

        SpacedRepetitionScheduler.Review(card, 4, Now);

        Assert.Equal(1, card.Repetitions);
        Assert.Equal(1, card.IntervalDays);
        Assert.Equal(Now.AddDays(1), card.NextReviewAt);
        Assert.Equal(Now, card.LastReviewedAt);
    }

    [Fact]
    public void Second_Successful_Review_Uses_Six_Day_Interval()
    {
        var card = NewCard();
        SpacedRepetitionScheduler.Review(card, 5, Now);

        SpacedRepetitionScheduler.Review(card, 5, Now.AddDays(1));

        Assert.Equal(2, card.Repetitions);
        Assert.Equal(6, card.IntervalDays);
        Assert.Equal(Now.AddDays(1).AddDays(6), card.NextReviewAt);
    }

    [Fact]
    public void Third_And_Later_Reviews_Grow_Interval_By_Easiness_Factor()
    {
        var card = NewCard();
        SpacedRepetitionScheduler.Review(card, 5, Now);           // EF 2.6, interval 1
        SpacedRepetitionScheduler.Review(card, 5, Now.AddDays(1)); // EF 2.7, interval 6

        SpacedRepetitionScheduler.Review(card, 5, Now.AddDays(7)); // interval = round(6 * 2.7) = 16

        Assert.Equal(3, card.Repetitions);
        Assert.Equal(17, card.IntervalDays);
        Assert.Equal(Now.AddDays(7).AddDays(17), card.NextReviewAt);
    }

    [Fact]
    public void Failed_Review_Resets_Repetitions_And_Schedules_Tomorrow()
    {
        var card = NewCard();
        SpacedRepetitionScheduler.Review(card, 5, Now);           // repetitions = 1
        SpacedRepetitionScheduler.Review(card, 5, Now.AddDays(1)); // repetitions = 2

        SpacedRepetitionScheduler.Review(card, 2, Now.AddDays(7));  // failure

        Assert.Equal(0, card.Repetitions);
        Assert.Equal(1, card.IntervalDays);
        Assert.Equal(Now.AddDays(8), card.NextReviewAt);
    }

    // ---- Input validation ----

    [Theory]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(100)]
    public void Quality_Outside_0_To_5_Throws(int quality)
    {
        var card = NewCard();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => SpacedRepetitionScheduler.Review(card, quality, Now));
    }

    // ---- IsDue ----

    [Fact]
    public void Never_Reviewed_Card_Is_Always_Due()
    {
        var card = NewCard();

        Assert.True(SpacedRepetitionScheduler.IsDue(card, Now));
    }

    [Fact]
    public void Card_With_Past_NextReviewAt_Is_Due()
    {
        var card = NewCard();
        SpacedRepetitionScheduler.Review(card, 5, Now.AddDays(-10));

        Assert.True(SpacedRepetitionScheduler.IsDue(card, Now));
    }

    [Fact]
    public void Card_With_Future_NextReviewAt_Is_Not_Due()
    {
        var card = NewCard();
        SpacedRepetitionScheduler.Review(card, 5, Now);

        Assert.False(SpacedRepetitionScheduler.IsDue(card, Now));
        Assert.True(SpacedRepetitionScheduler.IsDue(card, Now.AddDays(2)));
    }
}
