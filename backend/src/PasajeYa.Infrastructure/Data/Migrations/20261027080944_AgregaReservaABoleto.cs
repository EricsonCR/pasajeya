using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasajeYa.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregaReservaABoleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Boletos_ViajeId",
                table: "Boletos");

            migrationBuilder.AddColumn<string>(
                name: "Apellidos",
                table: "Boletos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Celular",
                table: "Boletos",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Correo",
                table: "Boletos",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Dni",
                table: "Boletos",
                type: "nchar(8)",
                fixedLength: true,
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nombres",
                table: "Boletos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservadoHasta",
                table: "Boletos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Boletos_ViajeId_NumeroAsiento",
                table: "Boletos",
                columns: new[] { "ViajeId", "NumeroAsiento" },
                unique: true,
                filter: "[Estado] IN ('Reservado','Pagado')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Boletos_ViajeId_NumeroAsiento",
                table: "Boletos");

            migrationBuilder.DropColumn(
                name: "Apellidos",
                table: "Boletos");

            migrationBuilder.DropColumn(
                name: "Celular",
                table: "Boletos");

            migrationBuilder.DropColumn(
                name: "Correo",
                table: "Boletos");

            migrationBuilder.DropColumn(
                name: "Dni",
                table: "Boletos");

            migrationBuilder.DropColumn(
                name: "Nombres",
                table: "Boletos");

            migrationBuilder.DropColumn(
                name: "ReservadoHasta",
                table: "Boletos");

            migrationBuilder.CreateIndex(
                name: "IX_Boletos_ViajeId",
                table: "Boletos",
                column: "ViajeId");
        }
    }
}
