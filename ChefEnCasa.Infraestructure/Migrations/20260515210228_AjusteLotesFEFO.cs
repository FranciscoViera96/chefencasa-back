using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChefEnCasa.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AjusteLotesFEFO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ALMACENES_UsuarioId_IngredienteId",
                table: "ALMACENES");

            migrationBuilder.CreateIndex(
                name: "IX_ALMACENES_UsuarioId_IngredienteId_FechaCaducidad",
                table: "ALMACENES",
                columns: new[] { "UsuarioId", "IngredienteId", "FechaCaducidad" },
                unique: true,
                filter: "[FechaCaducidad] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ALMACENES_UsuarioId_IngredienteId_FechaCaducidad",
                table: "ALMACENES");

            migrationBuilder.CreateIndex(
                name: "IX_ALMACENES_UsuarioId_IngredienteId",
                table: "ALMACENES",
                columns: new[] { "UsuarioId", "IngredienteId" },
                unique: true);
        }
    }
}
