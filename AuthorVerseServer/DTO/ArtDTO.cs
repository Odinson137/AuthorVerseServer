using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.DTO;

public class ArtDTO
{
    public int ArtId { get; set; }
    public required string Url { get; set; }
    public required UserDTO User { get; set; }
}



public class CreateArtDTO
{
    [Required]
    public int BookId { get; set; }
    [Required]
    public required IFormFile Image { get; set; }
    [Required]
    public required int Start { get; set; }
    public required int End { get; set; }
}