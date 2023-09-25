using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthorVerseServer.Migrations
{
    /// <inheritdoc />
    public partial class AddChapterSectionRepository : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChapterSection_BookChapters_BookChapterId",
                table: "ChapterSection");

            migrationBuilder.DropForeignKey(
                name: "FK_ChapterSection_Image_ImageId",
                table: "ChapterSection");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_ChapterSection_Sectionid",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_SectionChoice_ChapterSection_ChapterSectionId",
                table: "SectionChoice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChapterSection",
                table: "ChapterSection");

            migrationBuilder.RenameTable(
                name: "ChapterSection",
                newName: "ChapterSections");

            migrationBuilder.RenameIndex(
                name: "IX_ChapterSection_ImageId",
                table: "ChapterSections",
                newName: "IX_ChapterSections_ImageId");

            migrationBuilder.RenameIndex(
                name: "IX_ChapterSection_BookChapterId",
                table: "ChapterSections",
                newName: "IX_ChapterSections_BookChapterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChapterSections",
                table: "ChapterSections",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterSections_BookChapters_BookChapterId",
                table: "ChapterSections",
                column: "BookChapterId",
                principalTable: "BookChapters",
                principalColumn: "BookChapterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterSections_Image_ImageId",
                table: "ChapterSections",
                column: "ImageId",
                principalTable: "Image",
                principalColumn: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_ChapterSections_Sectionid",
                table: "Notes",
                column: "Sectionid",
                principalTable: "ChapterSections",
                principalColumn: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SectionChoice_ChapterSections_ChapterSectionId",
                table: "SectionChoice",
                column: "ChapterSectionId",
                principalTable: "ChapterSections",
                principalColumn: "SectionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChapterSections_BookChapters_BookChapterId",
                table: "ChapterSections");

            migrationBuilder.DropForeignKey(
                name: "FK_ChapterSections_Image_ImageId",
                table: "ChapterSections");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_ChapterSections_Sectionid",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_SectionChoice_ChapterSections_ChapterSectionId",
                table: "SectionChoice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChapterSections",
                table: "ChapterSections");

            migrationBuilder.RenameTable(
                name: "ChapterSections",
                newName: "ChapterSection");

            migrationBuilder.RenameIndex(
                name: "IX_ChapterSections_ImageId",
                table: "ChapterSection",
                newName: "IX_ChapterSection_ImageId");

            migrationBuilder.RenameIndex(
                name: "IX_ChapterSections_BookChapterId",
                table: "ChapterSection",
                newName: "IX_ChapterSection_BookChapterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChapterSection",
                table: "ChapterSection",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterSection_BookChapters_BookChapterId",
                table: "ChapterSection",
                column: "BookChapterId",
                principalTable: "BookChapters",
                principalColumn: "BookChapterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterSection_Image_ImageId",
                table: "ChapterSection",
                column: "ImageId",
                principalTable: "Image",
                principalColumn: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_ChapterSection_Sectionid",
                table: "Notes",
                column: "Sectionid",
                principalTable: "ChapterSection",
                principalColumn: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SectionChoice_ChapterSection_ChapterSectionId",
                table: "SectionChoice",
                column: "ChapterSectionId",
                principalTable: "ChapterSection",
                principalColumn: "SectionId");
        }
    }
}
