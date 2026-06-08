using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeisnerCare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRelativeUserIdToPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RelativeUserId",
                table: "Patients",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "AuditLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_RelativeUserId",
                table: "Patients",
                column: "RelativeUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_AspNetUsers_RelativeUserId",
                table: "Patients",
                column: "RelativeUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_AspNetUsers_RelativeUserId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_RelativeUserId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "RelativeUserId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "AuditLogs");
        }
    }
}
