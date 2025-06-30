using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "owner_user_id",
                table: "Businesses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_owner_user_id",
                table: "Businesses",
                column: "owner_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_AspNetUsers_owner_user_id",
                table: "Businesses",
                column: "owner_user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_AspNetUsers_owner_user_id",
                table: "Businesses");

            migrationBuilder.DropIndex(
                name: "IX_Businesses_owner_user_id",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "owner_user_id",
                table: "Businesses");
        }
    }
}
