using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace educonnectservice.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMotivoCancelacionToSesion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "motivo_cancelacion",
                table: "sesiones",
                type: "CLOB",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "motivo_cancelacion",
                table: "sesiones");
        }
    }
}
