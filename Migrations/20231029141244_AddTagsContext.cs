using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthorVerseServer.Migrations
{
    /// <inheritdoc />
    public partial class AddTagsContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookGenre_Genres_GenresGenreId",
                table: "BookGenre");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Genres",
                table: "Genres");

            migrationBuilder.RenameTable(
                name: "Genres",
                newName: "Genre");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Genre",
                table: "Genre",
                column: "GenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookGenre_Genre_GenresGenreId",
                table: "BookGenre",
                column: "GenresGenreId",
                principalTable: "Genre",
                principalColumn: "GenreId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookGenre_Genre_GenresGenreId",
                table: "BookGenre");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Genre",
                table: "Genre");

            migrationBuilder.RenameTable(
                name: "Genre",
                newName: "Genres");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Genres",
                table: "Genres",
                column: "GenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookGenre_Genres_GenresGenreId",
                table: "BookGenre",
                column: "GenresGenreId",
                principalTable: "Genres",
                principalColumn: "GenreId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
