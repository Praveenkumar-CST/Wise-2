using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WiseHRServer.Migrations
{
    /// <inheritdoc />
    public partial class AddImageColm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoBase64Content",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoContentType",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoFileName",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoBase64Content",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "PhotoContentType",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "PhotoFileName",
                table: "EmployeeDetails");
        }
    }
}
