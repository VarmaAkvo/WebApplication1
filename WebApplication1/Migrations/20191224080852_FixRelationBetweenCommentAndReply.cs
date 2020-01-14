using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class FixRelationBetweenCommentAndReply : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_ReplyToID",
                table: "Comments");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ReplyToID",
                table: "Comments",
                column: "ReplyToID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_ReplyToID",
                table: "Comments");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ReplyToID",
                table: "Comments",
                column: "ReplyToID",
                unique: true,
                filter: "[ReplyToID] IS NOT NULL");
        }
    }
}
