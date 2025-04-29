using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WiseHRServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankingInformation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Branch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IFSCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PANNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AadhaarNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankingInformation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FatherName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotherName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfJoining = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfRelieving = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TypeOfEmployment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JoiningLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaritalStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Allergies = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Medications = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhysicallyChallenged = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoOfChildren = table.Column<int>(type: "int", nullable: false),
                    Sons = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Daughters = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentState = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentZip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentMobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermanentAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermanentCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermanentState = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermanentZip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermanentMobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermanentEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassportFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassportNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassportNationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassportIssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PassportExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PassportPlaceOfIssue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContact1Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContact1Relationship = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContact1Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContact1City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContact1State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContact1ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContact1Mobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContact2Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContact2Relationship = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContact2Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContact2City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContact2State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContact2ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContact2Mobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomineeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomineeRelationship = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomineeAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomineeCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomineeState = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomineeZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomineeMobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Experience",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EducationJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkExperienceJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experience", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankingInformation_EmployeeID",
                table: "BankingInformation",
                column: "EmployeeID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDetails_EmployeeID",
                table: "EmployeeDetails",
                column: "EmployeeID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Experience_EmployeeID",
                table: "Experience",
                column: "EmployeeID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankingInformation");

            migrationBuilder.DropTable(
                name: "EmployeeDetails");

            migrationBuilder.DropTable(
                name: "Experience");
        }
    }
}
