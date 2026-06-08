using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeisnerCare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientConsent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ConsentDate",
                table: "Patients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ConsentGiven",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsentDate",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ConsentGiven",
                table: "Patients");
        }
    }
}
