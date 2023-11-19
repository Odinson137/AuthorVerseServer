using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthorVerseServer.Migrations
{
    /// <inheritdoc />
    public partial class AddEarnings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Earnings",
                table: "Books",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Earnings",
                table: "Books");
        }
    }
}
