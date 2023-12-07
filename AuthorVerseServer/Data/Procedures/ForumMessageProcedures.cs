using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthorVerseServer.Data.Procedures
{
    public class ForumMessageProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE PROCEDURE AddForumMessage " +
                                        "@BookId INT, " +
                                        "@UserId NVARCHAR(255), " +
                                        "@ParrentMessageId INT, " +
                                        "@Text NVARCHAR(MAX), " +
                                        "@SendTime DATETIME, " +
                                        "@MessageId INT OUTPUT " +
                                        "AS " +
                                        "BEGIN " +
                                        "   BEGIN TRANSACTION; " +
                                        "   INSERT INTO ForumMessages (BookId, UserId, ParrentMessageId, Text, SendTime) " +
                                        "   VALUES (@BookId, @UserId, @ParrentMessageId, @Text, @SendTime); " +
                                        "   SET @MessageId = SCOPE_IDENTITY(); " +
                                        "   COMMIT; " +
                                        "END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE AddForumMessage;");
        }
    }
}
