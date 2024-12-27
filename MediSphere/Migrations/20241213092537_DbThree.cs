using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediSphere.Migrations
{
    /// <inheritdoc />
    public partial class DbThree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "Users",
                type: "int",
                nullable: true);
        }
    }
}
