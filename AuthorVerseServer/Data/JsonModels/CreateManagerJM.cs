using System.Text.Json.Serialization;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Models.ContentModels;

namespace AuthorVerseServer.Data.JsonModels
{
    [JsonPolymorphic(
        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]
    [JsonDerivedType(typeof(TextContentJM))]
    [JsonDerivedType(typeof(ImageContentJM))]
    public class ContentBaseJM
    {
        public required ChangeType Operation { get; set; }

        public virtual ContentBase CreateModel()
        {
            throw new AccessViolationException("Not admit object");
        }
        public virtual ContentType GetContentType()
        {
            throw new AccessViolationException("Not admit object");
        }
    }

    public class TextContentJM : ContentBaseJM
    {
        public required string SectionContent { get; set; }
        public override ContentType GetContentType() => ContentType.Text;
        public override TextContent CreateModel()
        {
            var model = new TextContent()
            {
                Text = this.SectionContent,
            };

            return model;
        }
    }

    public interface IFileContent
    {
        public byte[] SectionContent { get; set; }
        public string Expansion { get; set; }
        // public string Url { get; set; }
        public string GetPath();
        public string GetUrl();
    }

    public class ImageContentJM : ContentBaseJM, IFileContent
    {
        public required byte[] SectionContent { get; set; }
        public required string Expansion { get; set; }
        private string Url { get; set; }
        public override ImageContent CreateModel()
        {
            Url = $"Image_{DateTime.Now:yyyyMMdd_HHmmss}{Expansion}";
            var model = new ImageContent()
            {
                Url = Url,
            };

            return model;
        }
        public override ContentType GetContentType() => ContentType.Image;
        public string GetPath() => "sectionImages";
        public string GetUrl() => Url;
    }
    
    public class AudioContentJM : ContentBaseJM, IFileContent
    {
        public required byte[] SectionContent { get; set; }
        public required string Expansion { get; set; }
        private string Url { get; set; }
        public override ImageContent CreateModel()
        {
            Url = $"Audio_{DateTime.Now:yyyyMMdd_HHmmss}{Expansion}";
            var model = new ImageContent()
            {
                Url = Url,
            };

            return model;
        }
        public override ContentType GetContentType() => ContentType.Audio;
        public string GetPath() => "audio";
        public string GetUrl() => Url;
    }
}
