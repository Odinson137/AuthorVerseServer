using AuthorVerseServer.Data.Enums;

namespace AuthorVerseServer.GraphQL.Types;

public class CreateBookGraphDTO
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public ICollection<int>? GenresId { get; set; }
    public ICollection<int>? TagsId { get; set; }
    public AgeRating AgeRating { get; set; }
}

public class UpdateBookGraphDTO
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public ICollection<int>? GenresId { get; set; }
    public ICollection<int>? TagsId { get; set; }
    public AgeRating AgeRating { get; set; }
}

public class UploadBookImage
{
    [GraphQLType(typeof(NonNullType<UploadType>))]
    public IFormFile Image { get; set; }
}