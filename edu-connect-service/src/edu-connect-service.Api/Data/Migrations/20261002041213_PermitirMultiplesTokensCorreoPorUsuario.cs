using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace educonnectservice.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class PermitirMultiplesTokensCorreoPorUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_token_correo_usuario_id",
                table: "token_correo");

            migrationBuilder.CreateIndex(
                name: "IX_token_correo_usuario_id",
                table: "token_correo",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_token_correo_usuario_id",
                table: "token_correo");

            migrationBuilder.CreateIndex(
                name: "IX_token_correo_usuario_id",
                table: "token_correo",
                column: "usuario_id",
                unique: true);
        }
    }
}
