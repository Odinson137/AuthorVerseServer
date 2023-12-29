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
        public required byte[] SectionContent { get; set; }
        public required string Expansion { get; set; }

    }
    public class ImageContent : FileContent { }


}
