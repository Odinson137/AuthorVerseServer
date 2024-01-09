using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.Models.ContentModels;

namespace AuthorVerseServer.Data.JsonModels
{
    public class ChoiceContent(int chapterId)
    {
        public required ChangeType Operation { get; init; }

        private int _nextChapterId;
        private int[] _choiceValues = null!;

        public string SetChoiceKey
        {
            set => _choiceValues = value.Split(":").Select(int.Parse).ToArray();
        }

        public int GetChapterId() => _choiceValues[0];
        public int GetNumber() => _choiceValues[1];
        public int GetFlow() => _choiceValues[2];
        public int GetChoiceNumber() => _choiceValues[3];
        public int NextChapterId
        {
            get => _nextChapterId == 0 ? chapterId : _nextChapterId;
            init => _nextChapterId = value;
        }
        public int NextNumber { get; init; }
        public int NextFlow { get; init; }
        public string Content { get; set; } = null!;

    }

    public class ExistSection
    {
        public ICollection<int> Choices { get; init; } = null!;
    }
    
    public class ContentBaseJm
    {
        public required ChangeType Operation { get; init; }

        public virtual ContentBase CreateModel()
        {
            throw new AccessViolationException("Not admit object");
        }
        public virtual ContentType GetContentType()
        {
            throw new AccessViolationException("Not admit object");
        }
    }

    public class TextContentJm : ContentBaseJm
    {
        public required string SectionContent { get; init; }
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
        public string GetPath();
        public string GetUrl();
    }

    public class ImageContentJm : ContentBaseJm, IFileContent
    {
        public required byte[] SectionContent { get; set; }
        public required string Expansion { get; set; }
        private string Url { get; set; }
        public override FileContent CreateModel()
        {
            Url = $"Image_{DateTime.Now:yyyyMMdd_HHmmss}{Expansion}";
            var model = new FileContent()
            {
                Url = Url,
            };

            return model;
        }
        public override ContentType GetContentType() => ContentType.Image;
        public string GetPath() => "sectionImages";
        public string GetUrl() => Url;
    }
    
    public class AudioContentJm : ContentBaseJm, IFileContent
    {
        public required byte[] SectionContent { get; set; }
        public required string Expansion { get; set; }
        private string Url { get; set; }
        public override FileContent CreateModel()
        {
            Url = $"Audio_{DateTime.Now:yyyyMMdd_HHmmss}{Expansion}";
            var model = new FileContent()
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
