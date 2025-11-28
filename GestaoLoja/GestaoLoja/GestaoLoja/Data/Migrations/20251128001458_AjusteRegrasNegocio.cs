using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoLoja.Migrations
{
    /// <inheritdoc />
    public partial class AjusteRegrasNegocio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ParaVenda",
                table: "Produtos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CategoriaPaiId",
                table: "Categorias",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_CategoriaPaiId",
                table: "Categorias",
                column: "CategoriaPaiId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_Categorias_CategoriaPaiId",
                table: "Categorias",
                column: "CategoriaPaiId",
                principalTable: "Categorias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_Categorias_CategoriaPaiId",
                table: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_CategoriaPaiId",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "ParaVenda",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "CategoriaPaiId",
                table: "Categorias");
        }
    }
}
