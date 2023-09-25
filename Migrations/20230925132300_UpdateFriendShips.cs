using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthorVerseServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFriendShips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectionChoice_ChapterSections_ChapterSectionId",
                table: "SectionChoice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SectionChoice",
                table: "SectionChoice");

            migrationBuilder.RenameTable(
                name: "SectionChoice",
                newName: "SectionChoices");

            migrationBuilder.RenameIndex(
                name: "IX_SectionChoice_ChapterSectionId",
                table: "SectionChoices",
                newName: "IX_SectionChoices_ChapterSectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SectionChoices",
                table: "SectionChoices",
                column: "SectionChoiceId");

            migrationBuilder.CreateTable(
                name: "Friendships",
                columns: table => new
                {
                    User1Id = table.Column<int>(type: "int", nullable: false),
                    User2Id = table.Column<int>(type: "int", nullable: false),
                    User2Id1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_Friendships_AspNetUsers_User2Id1",
                        column: x => x.User2Id1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_User1Id",
                table: "Friendships",
                column: "User1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_User2Id",
                table: "Friendships",
                column: "User2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_User2Id1",
                table: "Friendships",
                column: "User2Id1");

            migrationBuilder.AddForeignKey(
                name: "FK_SectionChoices_ChapterSections_ChapterSectionId",
                table: "SectionChoices",
                column: "ChapterSectionId",
                principalTable: "ChapterSections",
                principalColumn: "SectionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectionChoices_ChapterSections_ChapterSectionId",
                table: "SectionChoices");

            migrationBuilder.DropTable(
                name: "Friendships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SectionChoices",
                table: "SectionChoices");

            migrationBuilder.RenameTable(
                name: "SectionChoices",
                newName: "SectionChoice");

            migrationBuilder.RenameIndex(
                name: "IX_SectionChoices_ChapterSectionId",
                table: "SectionChoice",
                newName: "IX_SectionChoice_ChapterSectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SectionChoice",
                table: "SectionChoice",
                column: "SectionChoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_SectionChoice_ChapterSections_ChapterSectionId",
                table: "SectionChoice",
                column: "ChapterSectionId",
                principalTable: "ChapterSections",
                principalColumn: "SectionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
