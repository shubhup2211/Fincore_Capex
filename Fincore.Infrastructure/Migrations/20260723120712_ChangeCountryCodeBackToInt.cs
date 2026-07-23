using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fincore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCountryCodeBackToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CountryCode",
                table: "Countries",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "Countries",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
