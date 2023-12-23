using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;
using AuthorVerseServer.Interfaces;

namespace AuthorVerseServer.Data.Patterns
{
    public static class UseContentType
    {
        public delegate Task<SectionDTO> UseContentTypeDelegate(int contentId);

        public static UseContentTypeDelegate GetContent(IChapterSection section, ContentType type)
        {
            switch (type)
            {
                case ContentType.Text:
                    return section.GetTextContentAsync;

                case ContentType.Audio:
                    return section.GetAudioContentAsync;

                case ContentType.Video:
                    return section.GetVideoContentAsync;

                case ContentType.Image:
                    return section.GetImageContentAsync;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
