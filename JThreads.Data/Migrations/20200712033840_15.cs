using Microsoft.EntityFrameworkCore.Migrations;

namespace JThreads.Data.Migrations
{
    public partial class _15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentRatings_Comments_CommentId",
                table: "CommentRatings");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentRatings_Comments_CommentId",
                table: "CommentRatings",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "CommentId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentRatings_Comments_CommentId",
                table: "CommentRatings");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentRatings_Comments_CommentId",
                table: "CommentRatings",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "CommentId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
