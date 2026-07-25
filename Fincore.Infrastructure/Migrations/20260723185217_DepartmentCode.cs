using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fincore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DepartmentCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Departments_CompanyId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_DepartmentCode",
                table: "Departments");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CompanyId_DepartmentCode",
                table: "Departments",
                columns: new[] { "CompanyId", "DepartmentCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Departments_CompanyId_DepartmentCode",
                table: "Departments");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CompanyId",
                table: "Departments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DepartmentCode",
                table: "Departments",
                column: "DepartmentCode",
                unique: true);
        }
    }
}
