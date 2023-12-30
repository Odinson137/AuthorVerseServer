using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using AuthorVerseServer.Models.ContentModels;
using Microsoft.AspNetCore.Components;

namespace AuthorVerseServer.Data.JsonModels
{
    [JsonPolymorphic(
        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]
    [JsonDerivedType(typeof(TextContentJM))]
    [JsonDerivedType(typeof(ImageContentJM))]
    public class ContentBaseJM
    {
        public required Enums.ChangeType Operation { get; set; }

        public virtual ContentBase CreateModel()
        {
            throw new AccessViolationException("Not admit object");
        }

        [JsonIgnore]
        public virtual ContentType Type { get; set; } = ContentType.Text;
    }

    public class TextContentJM : ContentBaseJM
    {
        public required string SectionContent { get; set; }
        
        [JsonIgnore]
        public override ContentType Type { get; set; } = ContentType.Text;

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
        [JsonIgnore] public string Url { get; set; }
        [JsonIgnore] public string Path { get; set; }
    }

    public class ImageContentJM : ContentBaseJM, IFileContent
    {
        public required byte[] SectionContent { get; set; }
        public required string Expansion { get; set; }

        public override ImageContent CreateModel()
        {
            Url = $"Image_{DateTime.Now:yyyyMMdd_HHmmss}{Expansion}";
            var model = new ImageContent()
            {
                Url = Url,
            };

            return model;
        }

        [JsonIgnore]
        public override ContentType Type { get; set; } = ContentType.Image;
        [JsonIgnore] public string Url { get; set; }
        [JsonIgnore] public string Path { get; set; } = "sectionImages";
    }
}
