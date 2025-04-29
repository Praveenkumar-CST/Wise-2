using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WiseHRServer.Migrations
{
    /// <inheritdoc />
    public partial class AddFileUploadFieldsToBankingInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AadhaarBase64Content",
                table: "BankingInformation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AadhaarContentType",
                table: "BankingInformation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AadhaarFileName",
                table: "BankingInformation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PanBase64Content",
                table: "BankingInformation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PanContentType",
                table: "BankingInformation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PanFileName",
                table: "BankingInformation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PassbookBase64Content",
                table: "BankingInformation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassbookContentType",
                table: "BankingInformation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassbookFileName",
                table: "BankingInformation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AadhaarBase64Content",
                table: "BankingInformation");

            migrationBuilder.DropColumn(
                name: "AadhaarContentType",
                table: "BankingInformation");

            migrationBuilder.DropColumn(
                name: "AadhaarFileName",
                table: "BankingInformation");

            migrationBuilder.DropColumn(
                name: "PanBase64Content",
                table: "BankingInformation");

            migrationBuilder.DropColumn(
                name: "PanContentType",
                table: "BankingInformation");

            migrationBuilder.DropColumn(
                name: "PanFileName",
                table: "BankingInformation");

            migrationBuilder.DropColumn(
                name: "PassbookBase64Content",
                table: "BankingInformation");

            migrationBuilder.DropColumn(
                name: "PassbookContentType",
                table: "BankingInformation");

            migrationBuilder.DropColumn(
                name: "PassbookFileName",
                table: "BankingInformation");
        }
    }
}
