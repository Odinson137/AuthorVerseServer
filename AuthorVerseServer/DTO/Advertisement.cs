using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.DTO;

public class EarnDTO
{
    public required double Earn { get; set; }
    public required AdvertisementDTO AdvertisementDTO { get; set; }
}

public class AdvertisementDTO
{
    public required string Url { get; set; }
    public required string Link { get; set; }
}


public class CreateAdvertisementDTO
{
    [Required]
    public required string Link { get; set; }
    [Required]
    public required IFormFile Image { get; set; }
    [Required]
    public required double Cost { get; set; }
    [Required]
    public required DateTime StartDate { get; set; }
    [Required]
    public required DateTime EndDate { get; set; }
}