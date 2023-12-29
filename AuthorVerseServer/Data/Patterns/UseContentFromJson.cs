using AuthorVerseServer.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AuthorVerseServer.Data.Patterns
{
    public class UseContentFromJson
    {
        public static ContentBase GetContent(string contentValue, out string? folder)
        {
            var jsonObject = JObject.Parse(contentValue!);
            var contentType = jsonObject["type"]!.ToObject<Enums.ContentType>();
            switch (contentType)
            {
                case Enums.ContentType.Text:
                    folder = string.Empty;
                    return JsonConvert.DeserializeObject<TextContent>(contentValue)!;
                    //var content = jsonObject.ToObject<TextContent>()!;
                    //_section.AddContentAsync(content);
                    //return content;
                case Enums.ContentType.Image:
                    folder = "sectionImages";
                    return JsonConvert.DeserializeObject<ImageContent>(contentValue)!;
                default:
                    throw new NotSupportedException($"Unsupported content type: {contentType}");
            }
        }
    }
}
