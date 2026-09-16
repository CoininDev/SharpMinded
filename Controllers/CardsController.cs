using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SharpMinded.DTOs;
using SharpMinded.Models;
using static Supabase.Postgrest.Constants;
using System.Reflection.Metadata.Ecma335;

namespace SharpMinded.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CardsController : ControllerBase
{
    private readonly Supabase.Client _db_client;
    public CardsController(Supabase.Client db_client)
    {
        _db_client = db_client;
    }

    // listar
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CardSummaryDto>>> ListAll(int deckId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { message = "Invalid user identity." });

        var res = await _db_client.From<Card>()
            .Filter("deck_id", Operator.Equals, deckId)
            .Filter("user_id", Operator.Equals, userId)
            .Get();
        var cards = res.Models.Select(c => new CardSummaryDto(c)).ToList();
        return Ok(cards);
    }

    // criar
    [HttpPost]
    public async Task<ActionResult<CardSummaryDto>> CreateCard([FromBody] Card card)
    {
        card.UserId = GetUserId().ToString();
        await _db_client.From<Card>().Insert(card);
        return Ok(new CardSummaryDto(card));
    }

    // checar
    [HttpGet("{deckId}/{cardId}")]
    public async Task<ActionResult<Deck>> CheckCard(int deckId, int cardId)
    {
        var userId = GetUserId();

        var res = await _db_client.From<Card>()
            .Where(c => ValidateCard(deckId, cardId, c))
            .Get();

        var card = res.Model;

        if (card == null)
            return NotFound(new { message = "Deck not found" });

        return Ok(card);
    }


    // editar
    [HttpPut("{deckId}/{cardId}")]
    public async Task<ActionResult<Card>> UpdateDeck(int deckId, int cardId, [FromBody] Card card)
    {
        var userId = GetUserId();

        var res = await _db_client.From<Card>()
            .Where(c => ValidateCard(deckId, cardId, c))
            .Update(card);

        return Ok(res.Model);
    }

    // excluir
    [HttpDelete("{deckId}/{cardId}")]
    public async Task<ActionResult> DeleteDeck(int deckId, int cardId)
    {
        try
        {
            var userId = GetUserId();

            await _db_client.From<Card>()
                .Where(c => ValidateCard(deckId, cardId, c))
                .Delete();
            return Ok();
        }
            ?? return Unauthorized("User not logged in");

    }
    private bool ValidateCard(int deckId, int cardId, Card c)
    {
        return c.Id == cardId
            && c.DeckId == deckId
            && c.UserId == GetUserId().ToString();
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new Exception("User not logged in");

        return userId;
    }
}