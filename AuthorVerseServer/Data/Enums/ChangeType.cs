namespace AuthorVerseServer.Data.Enums
{
    /// <summary>
    /// Types of different operations with a section
    /// </summary>
    public enum ChangeType
    {
        /// <summary>
        /// The type that means creating a new section in the chapter
        /// </summary>
        Create,
        /// <summary>
        /// The type that means updating an already existing 
        /// section in a chapter
        /// </summary>
        Update,
        /// <summary>
        /// The type that means deleting the last section 
        /// from the list in the db
        /// </summary>
        Delete
    }
}
