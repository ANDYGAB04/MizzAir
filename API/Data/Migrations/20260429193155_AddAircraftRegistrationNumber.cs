﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddAircraftRegistrationNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "Aircrafts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE Aircrafts SET RegistrationNumber = 'YR-MZ' || Id WHERE RegistrationNumber = ''");

            migrationBuilder.CreateIndex(
                name: "IX_Aircrafts_RegistrationNumber",
                table: "Aircrafts",
                column: "RegistrationNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Aircrafts_RegistrationNumber",
                table: "Aircrafts");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "Aircrafts");
        }
    }
}
