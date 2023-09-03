using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthorVerseServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectionChoice_ChapterSection_ChapterSectionSectionId",
                table: "SectionChoice");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBooks_BookChapter_LastChapterBookChapterId",
                table: "UserBooks");

            migrationBuilder.DropIndex(
                name: "IX_SectionChoice_ChapterSectionSectionId",
                table: "SectionChoice");

            migrationBuilder.DropColumn(
                name: "ChapterSectionSectionId",
                table: "SectionChoice");

            migrationBuilder.RenameColumn(
                name: "LastChapterBookChapterId",
                table: "UserBooks",
                newName: "LastBookChapterBookChapterId");

            migrationBuilder.RenameIndex(
                name: "IX_UserBooks_LastChapterBookChapterId",
                table: "UserBooks",
                newName: "IX_UserBooks_LastBookChapterBookChapterId");

            migrationBuilder.RenameColumn(
                name: "TargetChapterId",
                table: "SectionChoice",
                newName: "TargetSectionId");

            migrationBuilder.AddColumn<int>(
                name: "ChapterSectionId",
                table: "SectionChoice",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SectionChoice_ChapterSectionId",
                table: "SectionChoice",
                column: "ChapterSectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SectionChoice_ChapterSection_ChapterSectionId",
                table: "SectionChoice",
                column: "ChapterSectionId",
                principalTable: "ChapterSection",
                principalColumn: "SectionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBooks_BookChapter_LastBookChapterBookChapterId",
                table: "UserBooks",
                column: "LastBookChapterBookChapterId",
                principalTable: "BookChapter",
                principalColumn: "BookChapterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectionChoice_ChapterSection_ChapterSectionId",
                table: "SectionChoice");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBooks_BookChapter_LastBookChapterBookChapterId",
                table: "UserBooks");

            migrationBuilder.DropIndex(
                name: "IX_SectionChoice_ChapterSectionId",
                table: "SectionChoice");

            migrationBuilder.DropColumn(
                name: "ChapterSectionId",
                table: "SectionChoice");

            migrationBuilder.RenameColumn(
                name: "LastBookChapterBookChapterId",
                table: "UserBooks",
                newName: "LastChapterBookChapterId");

            migrationBuilder.RenameIndex(
                name: "IX_UserBooks_LastBookChapterBookChapterId",
                table: "UserBooks",
                newName: "IX_UserBooks_LastChapterBookChapterId");

            migrationBuilder.RenameColumn(
                name: "TargetSectionId",
                table: "SectionChoice",
                newName: "TargetChapterId");

            migrationBuilder.AddColumn<int>(
                name: "ChapterSectionSectionId",
                table: "SectionChoice",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SectionChoice_ChapterSectionSectionId",
                table: "SectionChoice",
                column: "ChapterSectionSectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SectionChoice_ChapterSection_ChapterSectionSectionId",
                table: "SectionChoice",
                column: "ChapterSectionSectionId",
                principalTable: "ChapterSection",
                principalColumn: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBooks_BookChapter_LastChapterBookChapterId",
                table: "UserBooks",
                column: "LastChapterBookChapterId",
                principalTable: "BookChapter",
                principalColumn: "BookChapterId");
        }
    }
}
