using Microsoft.EntityFrameworkCore.Migrations;

namespace JThreads.Data.Migrations
{
    public partial class _13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Guest_GuestId",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Guest",
                table: "Guest");

            migrationBuilder.RenameTable(
                name: "Guest",
                newName: "Guests");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Guests",
                table: "Guests",
                column: "GuestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Guests_GuestId",
                table: "Comments",
                column: "GuestId",
                principalTable: "Guests",
                principalColumn: "GuestId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Guests_GuestId",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Guests",
                table: "Guests");

            migrationBuilder.RenameTable(
                name: "Guests",
                newName: "Guest");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Guest",
                table: "Guest",
                column: "GuestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Guest_GuestId",
                table: "Comments",
                column: "GuestId",
                principalTable: "Guest",
                principalColumn: "GuestId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
