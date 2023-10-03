using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthorVerseServer.Migrations
{
    /// <inheritdoc />
    public partial class AddTagsToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_User2Id1",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_User2Id1",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "User2Id1",
                table: "Friendships");

            migrationBuilder.AlterColumn<string>(
                name: "User2Id",
                table: "Friendships",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "User1Id",
                table: "Friendships",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "BookTag",
                columns: table => new
                {
                    BooksBookId = table.Column<int>(type: "int", nullable: false),
                    TagsTagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookTag", x => new { x.BooksBookId, x.TagsTagId });
                    table.ForeignKey(
                        name: "FK_BookTag_Books_BooksBookId",
                        column: x => x.BooksBookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookTag_Tag_TagsTagId",
                        column: x => x.TagsTagId,
                        principalTable: "Tag",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookTag_TagsTagId",
                table: "BookTag",
                column: "TagsTagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_User2Id",
                table: "Friendships",
                column: "User2Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_User2Id",
                table: "Friendships");

            migrationBuilder.DropTable(
                name: "BookTag");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.AlterColumn<int>(
                name: "User2Id",
                table: "Friendships",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "User1Id",
                table: "Friendships",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "User2Id1",
                table: "Friendships",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_User2Id1",
                table: "Friendships",
                column: "User2Id1");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_User2Id1",
                table: "Friendships",
                column: "User2Id1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
