using Microsoft.EntityFrameworkCore.Migrations;

namespace JThreads.Data.Migrations
{
    public partial class _8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentRating_Comments_CommentId",
                table: "CommentRating");

            migrationBuilder.DropForeignKey(
                name: "FK_ThreadRating_Threads_ThreadId",
                table: "ThreadRating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ThreadRating",
                table: "ThreadRating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentRating",
                table: "CommentRating");

            migrationBuilder.RenameTable(
                name: "ThreadRating",
                newName: "ThreadRatings");

            migrationBuilder.RenameTable(
                name: "CommentRating",
                newName: "CommentRatings");

            migrationBuilder.RenameIndex(
                name: "IX_ThreadRating_ThreadId",
                table: "ThreadRatings",
                newName: "IX_ThreadRatings_ThreadId");

            migrationBuilder.RenameIndex(
                name: "IX_CommentRating_CommentId",
                table: "CommentRatings",
                newName: "IX_CommentRatings_CommentId");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ThreadRatings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CommentRatings",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ThreadRatings",
                table: "ThreadRatings",
                column: "ThreadRatingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentRatings",
                table: "CommentRatings",
                column: "CommentRatingId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadRatings_UserId",
                table: "ThreadRatings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentRatings_UserId",
                table: "CommentRatings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentRatings_Comments_CommentId",
                table: "CommentRatings",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "CommentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentRatings_AspNetUsers_UserId",
                table: "CommentRatings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ThreadRatings_Threads_ThreadId",
                table: "ThreadRatings",
                column: "ThreadId",
                principalTable: "Threads",
                principalColumn: "ThreadId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ThreadRatings_AspNetUsers_UserId",
                table: "ThreadRatings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentRatings_Comments_CommentId",
                table: "CommentRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentRatings_AspNetUsers_UserId",
                table: "CommentRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_ThreadRatings_Threads_ThreadId",
                table: "ThreadRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_ThreadRatings_AspNetUsers_UserId",
                table: "ThreadRatings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ThreadRatings",
                table: "ThreadRatings");

            migrationBuilder.DropIndex(
                name: "IX_ThreadRatings_UserId",
                table: "ThreadRatings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentRatings",
                table: "CommentRatings");

            migrationBuilder.DropIndex(
                name: "IX_CommentRatings_UserId",
                table: "CommentRatings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ThreadRatings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CommentRatings");

            migrationBuilder.RenameTable(
                name: "ThreadRatings",
                newName: "ThreadRating");

            migrationBuilder.RenameTable(
                name: "CommentRatings",
                newName: "CommentRating");

            migrationBuilder.RenameIndex(
                name: "IX_ThreadRatings_ThreadId",
                table: "ThreadRating",
                newName: "IX_ThreadRating_ThreadId");

            migrationBuilder.RenameIndex(
                name: "IX_CommentRatings_CommentId",
                table: "CommentRating",
                newName: "IX_CommentRating_CommentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ThreadRating",
                table: "ThreadRating",
                column: "ThreadRatingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentRating",
                table: "CommentRating",
                column: "CommentRatingId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentRating_Comments_CommentId",
                table: "CommentRating",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "CommentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ThreadRating_Threads_ThreadId",
                table: "ThreadRating",
                column: "ThreadId",
                principalTable: "Threads",
                principalColumn: "ThreadId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
