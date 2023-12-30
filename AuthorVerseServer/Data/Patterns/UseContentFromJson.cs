using AuthorVerseServer.Data.JsonModels;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models.ContentModels;
using AuthorVerseServer.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AuthorVerseServer.Data.Patterns
{
    public static class UseContentFromJson
    {
        //public delegate Task UseContentContent<in T>(T content);

        public static Type GetContent(string contentValue, out string? folder)
        {
            var jsonObject = JObject.Parse(contentValue!);
            var contentType = jsonObject["type"]!.ToObject<Enums.ContentType>();
            switch (contentType)
            {
                case Enums.ContentType.Text:
                    folder = string.Empty;
                    //var textContent = JsonConvert.DeserializeObject<TextContentJM>(contentValue)!;
                    return typeof(TextContentJM);
                case Enums.ContentType.Image:
                    folder = "sectionImages";
                    //var imageContent = JsonConvert.DeserializeObject<ImageContentJM>(contentValue);
                    return typeof(TextContentJM);
                default:
                    throw new NotSupportedException($"Unsupported content type: {contentType}");
            }
        }
    }
}
