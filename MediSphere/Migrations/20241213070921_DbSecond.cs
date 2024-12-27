using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediSphere.Migrations
{
    /// <inheritdoc />
    public partial class DbSecond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Users");
        }
    }
}
