namespace AuthorVerseServer.Enums
{
    public enum PublicationPermission
    {
        PendingApproval, // Ожидает одобрения
        Approved,        // Одобрено и опубликовано
        Rejected,        // Отклонено (не опубликовано)
        Draft,           // Черновик (не готово к публикации)
    }

}
