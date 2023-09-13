using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewPracticeProject.Migrations
{
    /// <inheritdoc />
    public partial class AddIsConfirmedToStudentRegister : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "studentRegisters",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "studentRegisters");
        }
    }
}
