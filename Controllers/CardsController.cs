using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpMinded.DTOs;
using SharpMinded.Models;
using Supabase.Postgrest;


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
    [HttpGet("{deckId}")]
    public async Task<ActionResult<IEnumerable<CardSummaryDto>>> ListAll(int deckId)
    {
        var res = await GetAuthorizedCardQuery(deckId).Get();

        var cards = res.Models != null
            ? res.Models.Select(c => new CardSummaryDto(c)).ToList()
            : new List<CardSummaryDto>();

        return Ok(cards);
    }

    // criar
    [HttpPost]
    public async Task<ActionResult<CardSummaryDto>> CreateCard([FromBody] Card card)
    {
        var userId = GetUserId().ToString();
        card.UserId = userId;
        var res = await _db_client.From<Card>().Insert(card);
        if (res.Model == null)
            return BadRequest(new { message = "Failed to create card." });
        return Ok(new CardSummaryDto(res.Model));
    }

    // checar
    [HttpGet("{deckId}/{cardId}")]
    public async Task<ActionResult<Card>> CheckCard(int deckId, int cardId)
    {
        var userId = GetUserId();

        var res = await GetAuthorizedCardQuery(deckId, cardId).Get();

        var card = res.Model;

        if (card == null)
            return NotFound(new { message = "Card not found" });

        return Ok(card);
    }


    // editar
    [HttpPut("{deckId}/{cardId}")]
    public async Task<ActionResult<Card>> UpdateCard(int deckId, int cardId, [FromBody] Card card)
    {
        var userId = GetUserId();

        var res = await GetAuthorizedCardQuery(deckId, cardId).Update(card);

        return Ok(res.Model);
    }

    // excluir
    [HttpDelete("{deckId}/{cardId}")]
    public async Task<ActionResult> DeleteCard(int deckId, int cardId)
    {
        var userId = GetUserId();

        await GetAuthorizedCardQuery(deckId, cardId).Delete();
        return Ok();
    }
    private Supabase.Postgrest.Interfaces.IPostgrestTable<Card> GetAuthorizedCardQuery(int deckId, int? cardId = null)
    {
        var userId = GetUserId().ToString();
        var query = _db_client.From<Card>()
            .Where(c => c.DeckId == deckId && c.UserId == userId);

        if (cardId.HasValue)
        {
            query = query.Where(c => c.Id == cardId.Value);
        }

        return query;
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