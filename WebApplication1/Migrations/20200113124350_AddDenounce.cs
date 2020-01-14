using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class AddDenounce : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Denounces",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DenouncedID = table.Column<string>(nullable: true),
                    IsPost = table.Column<bool>(nullable: false),
                    PostOrCommentID = table.Column<int>(nullable: false),
                    DenouncingID = table.Column<string>(nullable: true),
                    Reason = table.Column<string>(nullable: false),
                    CreatedAtUTC = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Denounces", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Denounces_AspNetUsers_DenouncedID",
                        column: x => x.DenouncedID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Denounces_AspNetUsers_DenouncingID",
                        column: x => x.DenouncingID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Denounces_DenouncedID",
                table: "Denounces",
                column: "DenouncedID");

            migrationBuilder.CreateIndex(
                name: "IX_Denounces_DenouncingID",
                table: "Denounces",
                column: "DenouncingID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Denounces");
        }
    }
}
