using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PasajeYa.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Buses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Placa = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TipoServicio = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Capacidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ciudades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ciudades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rutas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrigenId = table.Column<int>(type: "int", nullable: false),
                    DestinoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rutas", x => x.Id);
                    table.CheckConstraint("CK_Rutas_OrigenDistintoDestino", "[OrigenId]<>[DestinoId]");
                    table.ForeignKey(
                        name: "FK_Rutas_Ciudades_DestinoId",
                        column: x => x.DestinoId,
                        principalTable: "Ciudades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rutas_Ciudades_OrigenId",
                        column: x => x.OrigenId,
                        principalTable: "Ciudades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Viajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RutaId = table.Column<int>(type: "int", nullable: false),
                    BusId = table.Column<int>(type: "int", nullable: false),
                    Salida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Llegada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Viajes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Viajes_Buses_BusId",
                        column: x => x.BusId,
                        principalTable: "Buses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Viajes_Rutas_RutaId",
                        column: x => x.RutaId,
                        principalTable: "Rutas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Boletos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ViajeId = table.Column<int>(type: "int", nullable: false),
                    NumeroAsiento = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boletos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Boletos_Viajes_ViajeId",
                        column: x => x.ViajeId,
                        principalTable: "Viajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Buses",
                columns: new[] { "Id", "Capacidad", "Placa", "TipoServicio" },
                values: new object[,]
                {
                    { 1, 32, "BDS275", "Economico" },
                    { 2, 48, "KFC192", "Economico" },
                    { 3, 32, "PIL483", "Ejecutivo" },
                    { 4, 28, "JFK581", "Ejecutivo" },
                    { 5, 16, "MGM397", "Vip" },
                    { 6, 14, "FIT814", "Vip" }
                });

            migrationBuilder.InsertData(
                table: "Ciudades",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Lima" },
                    { 2, "Arequipa" },
                    { 3, "Trujillo" },
                    { 4, "Tacna" },
                    { 5, "Piura" },
                    { 6, "Cajamarca" },
                    { 7, "Cusco" },
                    { 8, "Huancayo" }
                });

            migrationBuilder.InsertData(
                table: "Rutas",
                columns: new[] { "Id", "DestinoId", "OrigenId" },
                values: new object[,]
                {
                    { 1, 2, 1 },
                    { 2, 3, 1 },
                    { 3, 4, 1 },
                    { 4, 5, 1 },
                    { 5, 6, 1 },
                    { 6, 1, 2 },
                    { 7, 3, 2 },
                    { 8, 4, 2 },
                    { 9, 5, 2 },
                    { 10, 1, 3 },
                    { 11, 7, 3 },
                    { 12, 8, 3 },
                    { 13, 4, 3 },
                    { 14, 1, 5 },
                    { 15, 3, 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boletos_ViajeId",
                table: "Boletos",
                column: "ViajeId");

            migrationBuilder.CreateIndex(
                name: "IX_Buses_Placa",
                table: "Buses",
                column: "Placa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ciudades_Nombre",
                table: "Ciudades",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rutas_DestinoId",
                table: "Rutas",
                column: "DestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Rutas_OrigenId_DestinoId",
                table: "Rutas",
                columns: new[] { "OrigenId", "DestinoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Viajes_BusId",
                table: "Viajes",
                column: "BusId");

            migrationBuilder.CreateIndex(
                name: "IX_Viajes_RutaId",
                table: "Viajes",
                column: "RutaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Boletos");

            migrationBuilder.DropTable(
                name: "Viajes");

            migrationBuilder.DropTable(
                name: "Buses");

            migrationBuilder.DropTable(
                name: "Rutas");

            migrationBuilder.DropTable(
                name: "Ciudades");
        }
    }
}
