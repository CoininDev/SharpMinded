using Microsoft.AspNetCore.Mvc;
using SharpMinded.DTOs;
using SharpMinded.Models;

namespace SharpMinded.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DecksController : ControllerBase
{
    private readonly Supabase.Client _db_client;
    public DecksController(Supabase.Client db_client)
    {
        _db_client = db_client;
    }

    // listar
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeckSummaryDto>>> ListAll()
    {
        var response = await _db_client.From<Deck>().Get();
        var decks = response.Models.Select(d => new DeckSummaryDto(d)).ToList();
        return Ok(decks);
    }

    // criar
    [HttpPost]
    public async Task<ActionResult<Deck>> CreateDeck(Deck deck)
    {
        await _db_client.From<Deck>().Insert(deck);
        return Ok(deck);
    }

    // checar
    [HttpGet("[id]")]
    public async Task<ActionResult<Deck>> CheckDeck(int id)
    {
        var deckResponse = await _db_client.From<Deck>().Where(d => d.Id == id).Get();
        var deck = deckResponse.Model;

        if (deck == null)
            return NotFound(new {message = "Deck not found"});
        
        var cardsResponse
    }
    

    // editar
    // excluir
}