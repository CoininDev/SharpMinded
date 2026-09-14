using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SharpMinded.Models;

[Table("decks")]
public class Deck : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Reference(typeof(Card), includeInQuery: false)]
    public List<Card> Cards { get; set; } = new();
}