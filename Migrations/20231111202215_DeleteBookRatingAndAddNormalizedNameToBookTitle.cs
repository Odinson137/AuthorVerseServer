using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthorVerseServer.Migrations
{
    /// <inheritdoc />
    public partial class DeleteBookRatingAndAddNormalizedNameToBookTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookRating");

            migrationBuilder.DropColumn(
                name: "DisLikes",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "BookPanoramaImage",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "Likes",
                table: "Comments",
                newName: "ReaderRating");

            migrationBuilder.AddColumn<int>(
                name: "CountRating",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedTitle",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Books",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountRating",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "NormalizedTitle",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "ReaderRating",
                table: "Comments",
                newName: "Likes");

            migrationBuilder.AddColumn<int>(
                name: "DisLikes",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BookPanoramaImage",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BookRating",
                columns: table => new
                {
                    BookRatingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookRating", x => x.BookRatingId);
                    table.ForeignKey(
                        name: "FK_BookRating_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookRating_BookId",
                table: "BookRating",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookRating_BookRatingId",
                table: "BookRating",
                column: "BookRatingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookRating_Rating",
                table: "BookRating",
                column: "Rating");
        }
    }
}
