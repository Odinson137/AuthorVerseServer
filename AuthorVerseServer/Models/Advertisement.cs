using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models;

public class Advertisement
{
    [Key]
    public int AdvertisementId { get; set; }
    [MaxLength(100)]
    public string Url { get; set; }
    [MaxLength(100)]
    public string Link { get; set; }
    public double Cost { get; set; } = new Random().NextDouble();
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime EndDate { get; set; }
    public Advertisement(string url, string link, DateTime endDate)
    {
        Url = url;
        Link = link;
        EndDate = endDate;
    }

    public Advertisement() { }
}