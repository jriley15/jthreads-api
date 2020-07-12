using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JThreads.Data.Migrations
{
    public partial class _12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GuestId",
                table: "Comments",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Guest",
                columns: table => new
                {
                    GuestId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guest", x => x.GuestId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_GuestId",
                table: "Comments",
                column: "GuestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Guest_GuestId",
                table: "Comments",
                column: "GuestId",
                principalTable: "Guest",
                principalColumn: "GuestId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Guest_GuestId",
                table: "Comments");

            migrationBuilder.DropTable(
                name: "Guest");

            migrationBuilder.DropIndex(
                name: "IX_Comments_GuestId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "GuestId",
                table: "Comments");
        }
    }
}
