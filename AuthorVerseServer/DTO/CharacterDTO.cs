using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.DTO
{
    public class CharacterDTO
    {
        public required int CharacterId { get; set; }
        public required string Name { get; set; }
    }

    public class BookCharacterDTO
    {
        public required int CharacterId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
    }

    public class UpdateCharacterDTO
    {
        [Required]
        public required int CharacterId { get; set; }
        [Required]
        public required string Name { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
    }

    public class UpdateCharacterImageDTO
    {
        [Required]
        public required int CharacterId { get; set; }
        [Required]
        public required IFormFile Image { get; set; }
    }
}
