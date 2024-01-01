using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthorVerseServer.Models.ContentModels
{
    public class ContentBase
    {
        [Key]
        public int ContentId { get; set; }
        [JsonIgnore]
        public ChapterSection ChapterSection { get; set; }
    }
}
