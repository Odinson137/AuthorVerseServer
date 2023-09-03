using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthorVerseServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNamesAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBooks_BookChapter_LastBookChapterBookChapterId",
                table: "UserBooks");

            migrationBuilder.DropIndex(
                name: "IX_UserBooks_LastBookChapterBookChapterId",
                table: "UserBooks");

            migrationBuilder.DropColumn(
                name: "LastBookChapterBookChapterId",
                table: "UserBooks");

            migrationBuilder.AddColumn<int>(
                name: "LastBookChapterId",
                table: "UserBooks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserBooks_LastBookChapterId",
                table: "UserBooks",
                column: "LastBookChapterId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBooks_BookChapter_LastBookChapterId",
                table: "UserBooks",
                column: "LastBookChapterId",
                principalTable: "BookChapter",
                principalColumn: "BookChapterId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBooks_BookChapter_LastBookChapterId",
                table: "UserBooks");

            migrationBuilder.DropIndex(
                name: "IX_UserBooks_LastBookChapterId",
                table: "UserBooks");

            migrationBuilder.DropColumn(
                name: "LastBookChapterId",
                table: "UserBooks");

            migrationBuilder.AddColumn<int>(
                name: "LastBookChapterBookChapterId",
                table: "UserBooks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBooks_LastBookChapterBookChapterId",
                table: "UserBooks",
                column: "LastBookChapterBookChapterId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBooks_BookChapter_LastBookChapterBookChapterId",
                table: "UserBooks",
                column: "LastBookChapterBookChapterId",
                principalTable: "BookChapter",
                principalColumn: "BookChapterId");
        }
    }
}
