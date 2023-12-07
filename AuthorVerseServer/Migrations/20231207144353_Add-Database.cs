using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthorVerseServer.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE PROCEDURE AddForumMessage " +
                                "@BookId INT, " +
                                "@UserId NVARCHAR(255), " +
                                "@ParrentMessageId INT NULL, " +
                                "@Text NVARCHAR(MAX), " +
                                "@SendTime DATETIME, " +
                                "@MessageId INT OUTPUT " + // Добавлен OUTPUT
                                "AS " +
                                "BEGIN " +
                                "   INSERT INTO ForumMessages (BookId, UserId, ParrentMessageId, Text, SendTime) " +
                                "   VALUES (@BookId, @UserId, @ParrentMessageId, @Text, @SendTime) " +
                                "   SET @MessageId = SCOPE_IDENTITY(); " + // Установка @MessageId
                                "END;");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE AddForumMessage;");
        }
    }
}
