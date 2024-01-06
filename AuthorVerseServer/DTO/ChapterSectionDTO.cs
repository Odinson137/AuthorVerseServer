using AuthorVerseServer.Models.ContentModels;

namespace AuthorVerseServer.DTO
{
    public class AllContentDTO
    {
        public ChoiceBaseDTO? Choice { get; set; }
        public required ICollection<SectionDTO> SectionsDTO { get; set; }
    }

    public class ContentManagerDTO
    {
        public ChoiceBaseDTO? Choice { get; set; }
        public required ICollection<ContentDTO> ContentsDTO { get; set; }
    }
    public class ChoiceBaseDTO
    {
        public int ChoiceFlow { get; set; }
        public int Number { get; set; }
        public required int ContentId { get; set; }
        public required Data.Enums.ContentType ContentType { get; set; }
        public ICollection<SectionChoiceDTO>? SectionChoices { get; set; }
    }

    public class ContentDTO
    {
        public int ChoiceFlow { get; set; }
        public int Number { get; set; }
        public required int ContentId { get; set; } 
        public required Data.Enums.ContentType ContentType { get; set; }
    }

    public class SectionChoiceDTO
    {
        public int ChoiceFlow { get; set; }
        public int Number { get; set; }
        public required string ChoiceText { get; set; }
    }

    public class SectionDTO
    {
        public required string Content { get; set; } // если что то бросать всё в серилизацию и в строку если данные не буду стразу в виде строки. Я не собираюсь для каждого нового типа писать свой DTO для отправки
        public required Data.Enums.ContentType Type { get; set; }
    }

    public class AllContentWithModelDTO
    {
        public ChoiceBaseWithModelDTO? Choice { get; set; }
        public required ICollection<ContentWithModelDTO> ContentsDTO { get; set; }
    }

    public class ContentWithModelDTO
    {
        public int ChoiceFlow { get; set; }
        public int Number { get; set; }
        public required Data.Enums.ContentType ContentType { get; set; }
        public required ContentBase Content { get; set; }
    }

    public class ChoiceBaseWithModelDTO
    {
        public int ChoiceFlow { get; set; }
        public int Number { get; set; }
        public required Data.Enums.ContentType ContentType { get; set; }
        public required ContentBase Content { get; set; }
        public ICollection<SectionChoiceDTO>? SectionChoices { get; set; }
    }

    public class TransferInfoDTO
    {
        public ICollection<int> Chapters { get; set; }
        public ICollection<int> Numbers { get; set; }
        public ICollection<int> Flows { get; set; }
    }
}
