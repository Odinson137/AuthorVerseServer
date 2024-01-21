using System.ComponentModel.DataAnnotations;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Models;
using Tag = HotChocolate.Types.Tag;

namespace AuthorVerseServer.GraphQL.Types;

public class MutationBookDTO
{
    public required string Title { get; set; }
    public string Description { get; set; }
    public string AuthorId { get; set; }
    public ICollection<int> Tags { get; set; } 
    public ICollection<int> Genres { get; set; }
    public DateTime PublicationData { get; } 
    public AgeRating AgeRating { get; set; }
    public string? BookCover { get; set; }
    public double Rating { get; set; }
    public int CountRating { get; set; }
    public double Earnings { get; set; }
    public PublicationPermission Permission { get; set; }
}