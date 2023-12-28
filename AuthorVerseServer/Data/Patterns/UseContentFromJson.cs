using AuthorVerseServer.DTO;
using Newtonsoft.Json.Linq;

namespace AuthorVerseServer.Data.Patterns
{
    public class UseContentFromJson
    {
        public static ContentBase GetContent(string? contentValue, out string? folder)
        {
            var jsonObject = JObject.Parse(contentValue!);
            var contentType = jsonObject["Type"]!.ToObject<Data.Enums.ContentType>();
            switch (contentType)
            {
                case Data.Enums.ContentType.Text:
                    folder = string.Empty;
                    return jsonObject.ToObject<TextContent>()!;
                case Data.Enums.ContentType.Image:
                    folder = "sectionImages";
                    return jsonObject.ToObject<ImageContent>()!;
                default:
                    throw new NotSupportedException($"Unsupported content type: {contentType}");
            }
        }
    }
}
