using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthorVerseServer.Migrations
{
    /// <inheritdoc />
    public partial class DeleteImageModelAndAddMethodToUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Image_LogoImageId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Image_BookCoverImageId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_ChapterSections_Image_ImageId",
                table: "ChapterSections");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Image_CharacterImageImageId",
                table: "Characters");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropIndex(
                name: "IX_Characters_CharacterImageImageId",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_ChapterSections_ImageId",
                table: "ChapterSections");

            migrationBuilder.DropIndex(
                name: "IX_Books_BookCoverImageId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LogoImageId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CharacterImageImageId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "ChapterSections");

            migrationBuilder.DropColumn(
                name: "BookCoverImageId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "LogoImageId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "CharacterImageUrl",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ChapterSections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BookCover",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Method",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MicrosoftUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AzureName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MicrosoftUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MicrosoftUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MicrosoftUsers_AzureName",
                table: "MicrosoftUsers",
                column: "AzureName");

            migrationBuilder.CreateIndex(
                name: "IX_MicrosoftUsers_UserId",
                table: "MicrosoftUsers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MicrosoftUsers");

            migrationBuilder.DropColumn(
                name: "CharacterImageUrl",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ChapterSections");

            migrationBuilder.DropColumn(
                name: "BookCover",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "CharacterImageImageId",
                table: "Characters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "ChapterSections",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BookCoverImageId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LogoImageId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.ImageId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CharacterImageImageId",
                table: "Characters",
                column: "CharacterImageImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterSections_ImageId",
                table: "ChapterSections",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_BookCoverImageId",
                table: "Books",
                column: "BookCoverImageId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LogoImageId",
                table: "AspNetUsers",
                column: "LogoImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Image_LogoImageId",
                table: "AspNetUsers",
                column: "LogoImageId",
                principalTable: "Image",
                principalColumn: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Image_BookCoverImageId",
                table: "Books",
                column: "BookCoverImageId",
                principalTable: "Image",
                principalColumn: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterSections_Image_ImageId",
                table: "ChapterSections",
                column: "ImageId",
                principalTable: "Image",
                principalColumn: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Image_CharacterImageImageId",
                table: "Characters",
                column: "CharacterImageImageId",
                principalTable: "Image",
                principalColumn: "ImageId");
        }
    }
}
