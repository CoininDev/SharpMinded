namespace SharpMinded.DTOs;

public class CreateDeckRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}