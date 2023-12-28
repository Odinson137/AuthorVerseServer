namespace AuthorVerseServer.DTO
{
    public abstract class ContentBase
    {
        public required Data.Enums.ContentType Type { get; set; }
        public required Data.Enums.ChangeType Operation { get; set; }
    }

    public class TextContent : ContentBase
    {
        public required string SectionContent { get; set; }
    }

    public class FileContent : ContentBase
    {
        public required IFormFile SectionContent { get; set; }

    }
    public class ImageContent : FileContent { }


}
