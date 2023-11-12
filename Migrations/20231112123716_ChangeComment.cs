using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthorVerseServer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastBookChapterId",
                table: "UserSelectedBooks",
                newName: "LastBookChapterBookChapterId");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "BookChapters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSelectedBooks_LastBookChapterBookChapterId",
                table: "UserSelectedBooks",
                column: "LastBookChapterBookChapterId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSelectedBooks_BookChapters_LastBookChapterBookChapterId",
                table: "UserSelectedBooks",
                column: "LastBookChapterBookChapterId",
                principalTable: "BookChapters",
                principalColumn: "BookChapterId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSelectedBooks_BookChapters_LastBookChapterBookChapterId",
                table: "UserSelectedBooks");

            migrationBuilder.DropIndex(
                name: "IX_UserSelectedBooks_LastBookChapterBookChapterId",
                table: "UserSelectedBooks");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "BookChapters");

            migrationBuilder.RenameColumn(
                name: "LastBookChapterBookChapterId",
                table: "UserSelectedBooks",
                newName: "LastBookChapterId");
        }
    }
}
