using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChefEnCasa.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class InitFinalisimo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "INGREDIENTES",
                columns: table => new
                {
                    IngredienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreOriginal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreEspanol = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categoria = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    DiasVidaUtilEstimada = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INGREDIENTES", x => x.IngredienteId);
                });

            migrationBuilder.CreateTable(
                name: "RECETAS",
                columns: table => new
                {
                    RecetaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpoonacularId = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    Resumen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Instrucciones = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagenUrl = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    TiempoMinutos = table.Column<int>(type: "int", nullable: false),
                    Porciones = table.Column<int>(type: "int", nullable: false),
                    EsVegetariano = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsVegano = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsSinGluten = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsSinLacteos = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Calorias = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Carbohidratos = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Proteinas = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Grasas = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RECETAS", x => x.RecetaId);
                });

            migrationBuilder.CreateTable(
                name: "USUARIOS",
                columns: table => new
                {
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    TelefonoPrefijo = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    TelefonoNumero = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Puntos = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PoliciesAccepted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EmailVerificado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TelefonoVerificado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ResetToken = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    ResetTokenExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaUltimaSesion = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIOS", x => x.UsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "VERIFICACIONES",
                columns: table => new
                {
                    VerificacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Tipo = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VERIFICACIONES", x => x.VerificacionId);
                });

            migrationBuilder.CreateTable(
                name: "RECETA_INGREDIENTES",
                columns: table => new
                {
                    RecetaIngredienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecetaId = table.Column<int>(type: "int", nullable: false),
                    IngredienteId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UnidadMedida = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CantidadEnGramosOMl = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RECETA_INGREDIENTES", x => x.RecetaIngredienteId);
                    table.ForeignKey(
                        name: "FK_RECETA_INGREDIENTES_INGREDIENTE",
                        column: x => x.IngredienteId,
                        principalTable: "INGREDIENTES",
                        principalColumn: "IngredienteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RECETA_INGREDIENTES_RECETA",
                        column: x => x.RecetaId,
                        principalTable: "RECETAS",
                        principalColumn: "RecetaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ALMACENES",
                columns: table => new
                {
                    AlmacenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredienteId = table.Column<int>(type: "int", nullable: false),
                    CantidadEnGramosOMl = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EsPerecedero = table.Column<bool>(type: "bit", nullable: false),
                    FechaCaducidad = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALMACENES", x => x.AlmacenId);
                    table.ForeignKey(
                        name: "FK_ALMACENES_INGREDIENTES_IngredienteId",
                        column: x => x.IngredienteId,
                        principalTable: "INGREDIENTES",
                        principalColumn: "IngredienteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ALMACENES_USUARIOS_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "USUARIOS",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LISTAS_COMPRAS",
                columns: table => new
                {
                    ListaCompraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstaCompletada = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LISTAS_COMPRAS", x => x.ListaCompraId);
                    table.ForeignKey(
                        name: "FK_LISTAS_COMPRAS_USUARIOS_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "USUARIOS",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PERFILESSALUD",
                columns: table => new
                {
                    PerfilSaludId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Peso = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Altura = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IMC = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    EsVegetariano = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsVegano = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsCeliaco = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IntoleranteLactosa = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NecesidadCalorica = table.Column<int>(type: "int", nullable: false),
                    TMB = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PERFILESSALUD", x => x.PerfilSaludId);
                    table.ForeignKey(
                        name: "FK_PERFILSALUD_USUARIO",
                        column: x => x.UsuarioId,
                        principalTable: "USUARIOS",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RECETAS_FAVORITAS",
                columns: table => new
                {
                    RecetaFavoritaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecetaId = table.Column<int>(type: "int", nullable: false),
                    FechaGuardado = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RECETAS_FAVORITAS", x => x.RecetaFavoritaId);
                    table.ForeignKey(
                        name: "FK_RECETAS_FAVORITAS_RECETAS_RecetaId",
                        column: x => x.RecetaId,
                        principalTable: "RECETAS",
                        principalColumn: "RecetaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RECETAS_FAVORITAS_USUARIOS_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "USUARIOS",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SUSCRIPCIONES",
                columns: table => new
                {
                    SuscripcionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstadoPremium = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SUSCRIPCIONES", x => x.SuscripcionId);
                    table.ForeignKey(
                        name: "FK_SUSCRIPCION_USUARIO",
                        column: x => x.UsuarioId,
                        principalTable: "USUARIOS",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LISTA_COMPRA_DETALLES",
                columns: table => new
                {
                    ListaCompraDetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListaCompraId = table.Column<int>(type: "int", nullable: false),
                    IngredienteId = table.Column<int>(type: "int", nullable: false),
                    CantidadFaltante = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UnidadMedidaSugerida = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comprado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LISTA_COMPRA_DETALLES", x => x.ListaCompraDetalleId);
                    table.ForeignKey(
                        name: "FK_LISTA_COMPRA_DETALLES_INGREDIENTES_IngredienteId",
                        column: x => x.IngredienteId,
                        principalTable: "INGREDIENTES",
                        principalColumn: "IngredienteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LISTA_COMPRA_DETALLES_LISTAS_COMPRAS_ListaCompraId",
                        column: x => x.ListaCompraId,
                        principalTable: "LISTAS_COMPRAS",
                        principalColumn: "ListaCompraId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PERFIL_ALERGIAS",
                columns: table => new
                {
                    PerfilAlergiaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PerfilSaludId = table.Column<int>(type: "int", nullable: false),
                    IngredienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PERFIL_ALERGIAS", x => x.PerfilAlergiaId);
                    table.ForeignKey(
                        name: "FK_PERFIL_ALERGIAS_INGREDIENTES_IngredienteId",
                        column: x => x.IngredienteId,
                        principalTable: "INGREDIENTES",
                        principalColumn: "IngredienteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PERFIL_ALERGIAS_PERFILESSALUD_PerfilSaludId",
                        column: x => x.PerfilSaludId,
                        principalTable: "PERFILESSALUD",
                        principalColumn: "PerfilSaludId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ALMACENES_IngredienteId",
                table: "ALMACENES",
                column: "IngredienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ALMACENES_UsuarioId_IngredienteId",
                table: "ALMACENES",
                columns: new[] { "UsuarioId", "IngredienteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_INGREDIENTES_NombreEspanol",
                table: "INGREDIENTES",
                column: "NombreEspanol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LISTA_COMPRA_DETALLES_IngredienteId",
                table: "LISTA_COMPRA_DETALLES",
                column: "IngredienteId");

            migrationBuilder.CreateIndex(
                name: "IX_LISTA_COMPRA_DETALLES_ListaCompraId",
                table: "LISTA_COMPRA_DETALLES",
                column: "ListaCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_LISTAS_COMPRAS_UsuarioId",
                table: "LISTAS_COMPRAS",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PERFIL_ALERGIAS_IngredienteId",
                table: "PERFIL_ALERGIAS",
                column: "IngredienteId");

            migrationBuilder.CreateIndex(
                name: "IX_PERFIL_ALERGIAS_PerfilSaludId_IngredienteId",
                table: "PERFIL_ALERGIAS",
                columns: new[] { "PerfilSaludId", "IngredienteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PERFILESSALUD_UsuarioId",
                table: "PERFILESSALUD",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RECETA_INGREDIENTES_IngredienteId",
                table: "RECETA_INGREDIENTES",
                column: "IngredienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RECETA_INGREDIENTES_RecetaId",
                table: "RECETA_INGREDIENTES",
                column: "RecetaId");

            migrationBuilder.CreateIndex(
                name: "IX_RECETAS_Titulo",
                table: "RECETAS",
                column: "Titulo");

            migrationBuilder.CreateIndex(
                name: "IX_RECETAS_FAVORITAS_RecetaId",
                table: "RECETAS_FAVORITAS",
                column: "RecetaId");

            migrationBuilder.CreateIndex(
                name: "IX_RECETAS_FAVORITAS_UsuarioId_RecetaId",
                table: "RECETAS_FAVORITAS",
                columns: new[] { "UsuarioId", "RecetaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SUSCRIPCIONES_UsuarioId",
                table: "SUSCRIPCIONES",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USUARIOS_Email",
                table: "USUARIOS",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ALMACENES");

            migrationBuilder.DropTable(
                name: "LISTA_COMPRA_DETALLES");

            migrationBuilder.DropTable(
                name: "PERFIL_ALERGIAS");

            migrationBuilder.DropTable(
                name: "RECETA_INGREDIENTES");

            migrationBuilder.DropTable(
                name: "RECETAS_FAVORITAS");

            migrationBuilder.DropTable(
                name: "SUSCRIPCIONES");

            migrationBuilder.DropTable(
                name: "VERIFICACIONES");

            migrationBuilder.DropTable(
                name: "LISTAS_COMPRAS");

            migrationBuilder.DropTable(
                name: "PERFILESSALUD");

            migrationBuilder.DropTable(
                name: "INGREDIENTES");

            migrationBuilder.DropTable(
                name: "RECETAS");

            migrationBuilder.DropTable(
                name: "USUARIOS");
        }
    }
}
