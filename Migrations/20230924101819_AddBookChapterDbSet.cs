using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthorVerseServer.Migrations
{
    /// <inheritdoc />
    public partial class AddBookChapterDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookChapter_Books_BookId",
                table: "BookChapter");

            migrationBuilder.DropForeignKey(
                name: "FK_ChapterSection_BookChapter_BookChapterId",
                table: "ChapterSection");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSelectedBooks_BookChapter_LastBookChapterId",
                table: "UserSelectedBooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookChapter",
                table: "BookChapter");

            migrationBuilder.RenameTable(
                name: "BookChapter",
                newName: "BookChapters");

            migrationBuilder.RenameIndex(
                name: "IX_BookChapter_BookId",
                table: "BookChapters",
                newName: "IX_BookChapters_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookChapters",
                table: "BookChapters",
                column: "BookChapterId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookChapters_Books_BookId",
                table: "BookChapters",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterSection_BookChapters_BookChapterId",
                table: "ChapterSection",
                column: "BookChapterId",
                principalTable: "BookChapters",
                principalColumn: "BookChapterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSelectedBooks_BookChapters_LastBookChapterId",
                table: "UserSelectedBooks",
                column: "LastBookChapterId",
                principalTable: "BookChapters",
                principalColumn: "BookChapterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookChapters_Books_BookId",
                table: "BookChapters");

            migrationBuilder.DropForeignKey(
                name: "FK_ChapterSection_BookChapters_BookChapterId",
                table: "ChapterSection");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSelectedBooks_BookChapters_LastBookChapterId",
                table: "UserSelectedBooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookChapters",
                table: "BookChapters");

            migrationBuilder.RenameTable(
                name: "BookChapters",
                newName: "BookChapter");

            migrationBuilder.RenameIndex(
                name: "IX_BookChapters_BookId",
                table: "BookChapter",
                newName: "IX_BookChapter_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookChapter",
                table: "BookChapter",
                column: "BookChapterId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookChapter_Books_BookId",
                table: "BookChapter",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterSection_BookChapter_BookChapterId",
                table: "ChapterSection",
                column: "BookChapterId",
                principalTable: "BookChapter",
                principalColumn: "BookChapterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSelectedBooks_BookChapter_LastBookChapterId",
                table: "UserSelectedBooks",
                column: "LastBookChapterId",
                principalTable: "BookChapter",
                principalColumn: "BookChapterId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
