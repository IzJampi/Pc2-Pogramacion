using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pc2_Pogramacion.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Creditos = table.Column<int>(type: "INTEGER", nullable: false),
                    CupoMaximo = table.Column<int>(type: "INTEGER", nullable: false),
                    HorarioInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HorarioFin = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.Id);
                    table.CheckConstraint("CK_Curso_Horario", "HorarioInicio < HorarioFin");
                });

            migrationBuilder.CreateTable(
                name: "Matriculas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CursoId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioId = table.Column<string>(type: "TEXT", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matriculas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matriculas_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cursos",
                columns: new[] { "Id", "Activo", "Codigo", "Creditos", "CupoMaximo", "HorarioFin", "HorarioInicio", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "CS101", 4, 30, new DateTime(2025, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Programación" },
                    { 2, true, "CS102", 3, 25, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "Base de Datos" },
                    { 3, true, "CS103", 3, 20, new DateTime(2025, 1, 1, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Redes" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_Codigo",
                table: "Cursos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_CursoId_UsuarioId",
                table: "Matriculas",
                columns: new[] { "CursoId", "UsuarioId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matriculas");

            migrationBuilder.DropTable(
                name: "Cursos");
        }
    }
}
