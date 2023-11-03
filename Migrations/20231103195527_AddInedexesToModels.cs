using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthorVerseServer.Migrations
{
    /// <inheritdoc />
    public partial class AddInedexesToModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookGenre_Books_BooksBookId",
                table: "BookGenre");

            migrationBuilder.DropForeignKey(
                name: "FK_BookGenre_Genres_GenresGenreId",
                table: "BookGenre");

            migrationBuilder.DropForeignKey(
                name: "FK_BookTag_Books_BooksBookId",
                table: "BookTag");

            migrationBuilder.DropForeignKey(
                name: "FK_BookTag_Tags_TagsTagId",
                table: "BookTag");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Books_BookId",
                table: "Characters");

            migrationBuilder.RenameColumn(
                name: "TagsTagId",
                table: "BookTag",
                newName: "TagId");

            migrationBuilder.RenameColumn(
                name: "BooksBookId",
                table: "BookTag",
                newName: "BookId");

            migrationBuilder.RenameIndex(
                name: "IX_BookTag_TagsTagId",
                table: "BookTag",
                newName: "IX_BookTag_TagId");

            migrationBuilder.RenameColumn(
                name: "GenresGenreId",
                table: "BookGenre",
                newName: "GenreId");

            migrationBuilder.RenameColumn(
                name: "BooksBookId",
                table: "BookGenre",
                newName: "BookId");

            migrationBuilder.RenameIndex(
                name: "IX_BookGenre_GenresGenreId",
                table: "BookGenre",
                newName: "IX_BookGenre_GenreId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Notes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_UserSelectedBooks_UserBookId",
                table: "UserSelectedBooks",
                column: "UserBookId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionChoices_SectionChoiceId",
                table: "SectionChoices",
                column: "SectionChoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_NoteCreatedDateTime",
                table: "Notes",
                column: "NoteCreatedDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_NoteId",
                table: "Notes",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_Permission",
                table: "Notes",
                column: "Permission");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserId",
                table: "Notes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MicrosoftUsers_Id",
                table: "MicrosoftUsers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_Status",
                table: "Friendships",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentId",
                table: "Comments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_Permission",
                table: "Comments",
                column: "Permission");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CharacterId",
                table: "Characters",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterSections_Number",
                table: "ChapterSections",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterSections_SectionId",
                table: "ChapterSections",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_BookTag_BookId",
                table: "BookTag",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookRating_BookRatingId",
                table: "BookRating",
                column: "BookRatingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookGenre_BookId",
                table: "BookGenre",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookChapters_BookChapterId",
                table: "BookChapters",
                column: "BookChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_BookChapters_PublicationData",
                table: "BookChapters",
                column: "PublicationData");

            migrationBuilder.AddForeignKey(
                name: "FK_BookGenre_Books_BookId",
                table: "BookGenre",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookGenre_Genres_GenreId",
                table: "BookGenre",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "GenreId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookTag_Books_BookId",
                table: "BookTag",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookTag_Tags_TagId",
                table: "BookTag",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Books_BookId",
                table: "Characters",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookGenre_Books_BookId",
                table: "BookGenre");

            migrationBuilder.DropForeignKey(
                name: "FK_BookGenre_Genres_GenreId",
                table: "BookGenre");

            migrationBuilder.DropForeignKey(
                name: "FK_BookTag_Books_BookId",
                table: "BookTag");

            migrationBuilder.DropForeignKey(
                name: "FK_BookTag_Tags_TagId",
                table: "BookTag");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Books_BookId",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_UserSelectedBooks_UserBookId",
                table: "UserSelectedBooks");

            migrationBuilder.DropIndex(
                name: "IX_SectionChoices_SectionChoiceId",
                table: "SectionChoices");

            migrationBuilder.DropIndex(
                name: "IX_Notes_NoteCreatedDateTime",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_NoteId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_Permission",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_UserId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_MicrosoftUsers_Id",
                table: "MicrosoftUsers");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_Status",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_Permission",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Characters_CharacterId",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_ChapterSections_Number",
                table: "ChapterSections");

            migrationBuilder.DropIndex(
                name: "IX_ChapterSections_SectionId",
                table: "ChapterSections");

            migrationBuilder.DropIndex(
                name: "IX_BookTag_BookId",
                table: "BookTag");

            migrationBuilder.DropIndex(
                name: "IX_BookRating_BookRatingId",
                table: "BookRating");

            migrationBuilder.DropIndex(
                name: "IX_BookGenre_BookId",
                table: "BookGenre");

            migrationBuilder.DropIndex(
                name: "IX_BookChapters_BookChapterId",
                table: "BookChapters");

            migrationBuilder.DropIndex(
                name: "IX_BookChapters_PublicationData",
                table: "BookChapters");

            migrationBuilder.RenameColumn(
                name: "TagId",
                table: "BookTag",
                newName: "TagsTagId");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "BookTag",
                newName: "BooksBookId");

            migrationBuilder.RenameIndex(
                name: "IX_BookTag_TagId",
                table: "BookTag",
                newName: "IX_BookTag_TagsTagId");

            migrationBuilder.RenameColumn(
                name: "GenreId",
                table: "BookGenre",
                newName: "GenresGenreId");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "BookGenre",
                newName: "BooksBookId");

            migrationBuilder.RenameIndex(
                name: "IX_BookGenre_GenreId",
                table: "BookGenre",
                newName: "IX_BookGenre_GenresGenreId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Notes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_BookGenre_Books_BooksBookId",
                table: "BookGenre",
                column: "BooksBookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookGenre_Genres_GenresGenreId",
                table: "BookGenre",
                column: "GenresGenreId",
                principalTable: "Genres",
                principalColumn: "GenreId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookTag_Books_BooksBookId",
                table: "BookTag",
                column: "BooksBookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookTag_Tags_TagsTagId",
                table: "BookTag",
                column: "TagsTagId",
                principalTable: "Tags",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Books_BookId",
                table: "Characters",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
