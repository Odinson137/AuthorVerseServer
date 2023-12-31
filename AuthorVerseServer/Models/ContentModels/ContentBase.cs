using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.Models.ContentModels
{
    public class ContentBase
    {
        [Key]
        public int ContentId { get; set; }
        //public int ChapterSectionId { get; set; }
        public ChapterSection ChapterSection { get; set; }
    }
}
