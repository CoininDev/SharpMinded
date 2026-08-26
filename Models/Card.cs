using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SharpMinded.Models;

[Table("cards")]
public class Card : BaseModel
{
    [PrimaryKey("id")]
    public int Id {get; set;}
    
    [Column ("front")]
    public string? Front { get; set; }


    [Column ("back")]
    public string? Back { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; }
}