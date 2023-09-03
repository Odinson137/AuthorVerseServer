using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models
{
    public class Image
    {
        [Key]
        public int ImageId { get; set; }
        public string Url { get; set; } = null!;
    }
}
