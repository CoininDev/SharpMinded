using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpMinded.DTOs;
using SharpMinded.Models;
using SharpMinded.Services;

namespace SharpMinded.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudyController : ControllerBase
{
    private readonly Supabase.Client _db_client;

    public StudyController(Supabase.Client db_client)
    {
        _db_client = db_client;
    }

    /// <summary>
    /// Lists the cards of a deck that are due for review now.
    /// Cards that were never reviewed are always due.
    /// </summary>
    /// <param name="deckId">Deck to study.</param>
    /// <param name="limit">Max number of cards returned (default 20).</param>
    [HttpGet("due")]
    public async Task<ActionResult<IEnumerable<StudyCardDto>>> GetDueCards(
        [FromQuery] int deckId, [FromQuery] int limit = 20)
    {
        var userId = GetUserId().ToString();

        var res = await _db_client.From<Card>()
            .Where(c => c.DeckId == deckId && c.UserId == userId)
            .Get();

        var now = DateTime.UtcNow;
        var due = res.Models
            .Where(c => SpacedRepetitionScheduler.IsDue(c, now))
            .OrderBy(c => c.NextReviewAt ?? DateTime.MinValue) // oldest due first
            .Take(limit)
            .Select(c => new StudyCardDto(c))
            .ToList();

        return Ok(due);
    }

    /// <summary>
    /// Registers a review for a card and reschedules it using the SM-2 algorithm.
    /// </summary>
    [HttpPost("review")]
    public async Task<ActionResult<StudyCardDto>> ReviewCard([FromBody] ReviewCardRequest request)
    {
        var userId = GetUserId().ToString();

        var res = await _db_client.From<Card>()
            .Where(c => c.Id == request.CardId
                     && c.DeckId == request.DeckId
                     && c.UserId == userId)
            .Get();

        var card = res.Model;
        if (card is null)
            return NotFound(new { message = "Card not found" });

        SpacedRepetitionScheduler.Review(card, request.Quality, DateTime.UtcNow);

        var update = await _db_client.From<Card>().Update(card);
        if (update.Model is null)
            return BadRequest(new { message = "Failed to save review." });

        return Ok(new StudyCardDto(update.Model));
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("User not logged in");

        return userId;
    }
}
