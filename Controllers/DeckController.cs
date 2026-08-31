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
        var res = await _db_client.From<Deck>().Get();
        var decks = res.Models.Select(d => new DeckSummaryDto(d)).ToList();
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
    [HttpGet("{id}")]
    public async Task<ActionResult<Deck>> CheckDeck(int id)
    {
        var res = await _db_client.From<Deck>().Where(d => d.Id == id).Get();
        var deck = res.Model;

        if (deck == null)
            return NotFound(new {message = "Deck not found"});
        
        return Ok(deck);
    }


    // editar
    [HttpPut("{id}")]
    public async Task<ActionResult<Deck>> UpdateDeck(int id, Deck deck)
    {
        var res = await _db_client.From<Deck>().Where(d => d.Id == id).Update(deck);
        return Ok(res.Model);
    }

    // excluir
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDeck(int id)
    {
        await _db_client.From<Deck>().Where(d => d.Id == id).Delete();
        return Ok();
    }
}