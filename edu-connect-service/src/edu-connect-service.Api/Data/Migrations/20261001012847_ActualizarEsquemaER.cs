using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace educonnectservice.Api.Data.Migrations
{
    public partial class ActualizarEsquemaER : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "correo_validado",
                table: "usuarios",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "documento_cv_url",
                table: "tutores",
                type: "NVARCHAR2(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "documento_carnet_url",
                table: "estudiantes",
                type: "NVARCHAR2(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "calificaciones_estudiantes",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    sesion_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    estrellas = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    comentario = table.Column<string>(type: "CLOB", nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calificaciones_estudiantes", x => x.id);
                    table.ForeignKey(
                        name: "FK_calificaciones_estudiantes_sesiones_sesion_id",
                        column: x => x.sesion_id,
                        principalTable: "sesiones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "calificaciones_tutores",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    sesion_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    estrellas = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    comentario = table.Column<string>(type: "CLOB", nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calificaciones_tutores", x => x.id);
                    table.ForeignKey(
                        name: "FK_calificaciones_tutores_sesiones_sesion_id",
                        column: x => x.sesion_id,
                        principalTable: "sesiones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "categorias_reportes_estudiantes",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias_reportes_estudiantes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categorias_reportes_tutores",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias_reportes_tutores", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "planes_estudio",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    sesion_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    dificultades_identificadas = table.Column<string>(type: "CLOB", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planes_estudio", x => x.id);
                    table.ForeignKey(
                        name: "FK_planes_estudio_sesiones_sesion_id",
                        column: x => x.sesion_id,
                        principalTable: "sesiones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "token_correo",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    usuario_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    token = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    fecha_generacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_expiracion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    revocado = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_token_correo", x => x.id);
                    table.ForeignKey(
                        name: "FK_token_correo_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reportes_estudiantes",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    sesion_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    categoria_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    motivo = table.Column<string>(type: "CLOB", nullable: false),
                    fecha_reporte = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    estado = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reportes_estudiantes", x => x.id);
                    table.ForeignKey(
                        name: "FK_reportes_estudiantes_categorias_reportes_estudiantes_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias_reportes_estudiantes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reportes_estudiantes_sesiones_sesion_id",
                        column: x => x.sesion_id,
                        principalTable: "sesiones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reportes_tutores",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    sesion_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    categoria_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    motivo = table.Column<string>(type: "CLOB", nullable: false),
                    fecha_reporte = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    estado = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reportes_tutores", x => x.id);
                    table.ForeignKey(
                        name: "FK_reportes_tutores_categorias_reportes_tutores_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias_reportes_tutores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reportes_tutores_sesiones_sesion_id",
                        column: x => x.sesion_id,
                        principalTable: "sesiones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recursos_plan_estudio",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    plan_estudio_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    nombre = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    tipo = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    descripcion_uso = table.Column<string>(type: "CLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recursos_plan_estudio", x => x.id);
                    table.ForeignKey(
                        name: "FK_recursos_plan_estudio_planes_estudio_plan_estudio_id",
                        column: x => x.plan_estudio_id,
                        principalTable: "planes_estudio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_calificaciones_estudiantes_sesion_id",
                table: "calificaciones_estudiantes",
                column: "sesion_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_calificaciones_tutores_sesion_id",
                table: "calificaciones_tutores",
                column: "sesion_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_categorias_reportes_estudiantes_nombre",
                table: "categorias_reportes_estudiantes",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_categorias_reportes_tutores_nombre",
                table: "categorias_reportes_tutores",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_planes_estudio_sesion_id",
                table: "planes_estudio",
                column: "sesion_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recursos_plan_estudio_plan_estudio_id",
                table: "recursos_plan_estudio",
                column: "plan_estudio_id");

            migrationBuilder.CreateIndex(
                name: "IX_reportes_estudiantes_categoria_id",
                table: "reportes_estudiantes",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "IX_reportes_estudiantes_sesion_id",
                table: "reportes_estudiantes",
                column: "sesion_id");

            migrationBuilder.CreateIndex(
                name: "IX_reportes_tutores_categoria_id",
                table: "reportes_tutores",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "IX_reportes_tutores_sesion_id",
                table: "reportes_tutores",
                column: "sesion_id");

            migrationBuilder.CreateIndex(
                name: "IX_token_correo_usuario_id",
                table: "token_correo",
                column: "usuario_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "calificaciones_estudiantes");

            migrationBuilder.DropTable(
                name: "calificaciones_tutores");

            migrationBuilder.DropTable(
                name: "recursos_plan_estudio");

            migrationBuilder.DropTable(
                name: "reportes_estudiantes");

            migrationBuilder.DropTable(
                name: "reportes_tutores");

            migrationBuilder.DropTable(
                name: "token_correo");

            migrationBuilder.DropTable(
                name: "planes_estudio");

            migrationBuilder.DropTable(
                name: "categorias_reportes_estudiantes");

            migrationBuilder.DropTable(
                name: "categorias_reportes_tutores");

            migrationBuilder.DropColumn(
                name: "correo_validado",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "documento_cv_url",
                table: "tutores");

            migrationBuilder.DropColumn(
                name: "documento_carnet_url",
                table: "estudiantes");
        }
    }
}
