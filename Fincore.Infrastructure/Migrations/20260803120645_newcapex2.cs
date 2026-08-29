using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fincore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class newcapex2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequisitions_ApprovalFlows_ApprovalFlowId",
                table: "PurchaseRequisitions");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequisitions_ApprovalFlowId",
                table: "PurchaseRequisitions");

            migrationBuilder.DropColumn(
                name: "ApprovalFlowId",
                table: "PurchaseRequisitions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApprovalFlowId",
                table: "PurchaseRequisitions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequisitions_ApprovalFlowId",
                table: "PurchaseRequisitions",
                column: "ApprovalFlowId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequisitions_ApprovalFlows_ApprovalFlowId",
                table: "PurchaseRequisitions",
                column: "ApprovalFlowId",
                principalTable: "ApprovalFlows",
                principalColumn: "ApprovalFlowId");
        }
    }
}
