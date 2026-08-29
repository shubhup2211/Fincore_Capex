using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fincore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class approver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApproverId",
                table: "CapexRequests",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_CapexRequests_ApproverId",
                table: "CapexRequests",
                column: "ApproverId");

            migrationBuilder.AddForeignKey(
                name: "FK_CapexRequests_Users_ApproverId",
                table: "CapexRequests",
                column: "ApproverId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CapexRequests_Users_ApproverId",
                table: "CapexRequests");

            migrationBuilder.DropIndex(
                name: "IX_CapexRequests_ApproverId",
                table: "CapexRequests");

            migrationBuilder.DropColumn(
                name: "ApproverId",
                table: "CapexRequests");
        }
    }
}
