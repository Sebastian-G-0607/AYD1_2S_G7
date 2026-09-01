using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace educonnectservice.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialOracleSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "estados_sesiones",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nombre = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estados_sesiones", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "estados_usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nombre = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estados_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "materias",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_materias", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nombre = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    CategoryId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Price = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    LastUpdatedBy = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    correo = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    password_hash = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    rol_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    estado_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_baja = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    motivo_baja = table.Column<string>(type: "CLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuarios_estados_usuarios_estado_id",
                        column: x => x.estado_id,
                        principalTable: "estados_usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuarios_roles_rol_id",
                        column: x => x.rol_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "administradores",
                columns: table => new
                {
                    usuario_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    password_fase2_hash = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_administradores", x => x.usuario_id);
                    table.ForeignKey(
                        name: "FK_administradores_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "estudiantes",
                columns: table => new
                {
                    usuario_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    apellido = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    carnet = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    genero = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    direccion = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    telefono = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    fecha_nacimiento = table.Column<DateTime>(type: "DATE", nullable: false),
                    fotografia_url = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estudiantes", x => x.usuario_id);
                    table.ForeignKey(
                        name: "FK_estudiantes_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tutores",
                columns: table => new
                {
                    usuario_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    apellido = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    carnet_id = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    numero_identificacion = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    genero = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    direccion = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    telefono = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    fecha_nacimiento = table.Column<DateTime>(type: "DATE", nullable: false),
                    fotografia_url = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    direccion_tutoria = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    anio_inicio = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    universidad = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    hora_inicio = table.Column<TimeSpan>(type: "INTERVAL DAY(0) TO SECOND(0)", nullable: true),
                    hora_fin = table.Column<TimeSpan>(type: "INTERVAL DAY(0) TO SECOND(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tutores", x => x.usuario_id);
                    table.CheckConstraint("check_rango_horario", "\"hora_fin\" > \"hora_inicio\"");
                    table.ForeignKey(
                        name: "FK_tutores_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sesiones",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    estudiante_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    tutor_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    materia_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    estado_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    fecha_sesion = table.Column<DateTime>(type: "DATE", nullable: false),
                    hora_inicio = table.Column<TimeSpan>(type: "INTERVAL DAY(0) TO SECOND(0)", nullable: false),
                    hora_fin = table.Column<TimeSpan>(type: "INTERVAL DAY(0) TO SECOND(0)", nullable: true),
                    motivo = table.Column<string>(type: "CLOB", nullable: false),
                    resumen = table.Column<string>(type: "CLOB", nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sesiones", x => x.id);
                    table.ForeignKey(
                        name: "FK_sesiones_estados_sesiones_estado_id",
                        column: x => x.estado_id,
                        principalTable: "estados_sesiones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sesiones_estudiantes_estudiante_id",
                        column: x => x.estudiante_id,
                        principalTable: "estudiantes",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sesiones_materias_materia_id",
                        column: x => x.materia_id,
                        principalTable: "materias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sesiones_tutores_tutor_id",
                        column: x => x.tutor_id,
                        principalTable: "tutores",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tutores_dias_atencion",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    tutor_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    dia_semana = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tutores_dias_atencion", x => x.id);
                    table.CheckConstraint("check_dia_semana", "\"dia_semana\" BETWEEN 1 AND 7");
                    table.ForeignKey(
                        name: "FK_tutores_dias_atencion_tutores_tutor_id",
                        column: x => x.tutor_id,
                        principalTable: "tutores",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tutores_materias",
                columns: table => new
                {
                    tutor_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    materia_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tutores_materias", x => new { x.tutor_id, x.materia_id });
                    table.ForeignKey(
                        name: "FK_tutores_materias_materias_materia_id",
                        column: x => x.materia_id,
                        principalTable: "materias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tutores_materias_tutores_tutor_id",
                        column: x => x.tutor_id,
                        principalTable: "tutores",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_estados_sesiones_nombre",
                table: "estados_sesiones",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estados_usuarios_nombre",
                table: "estados_usuarios",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estudiantes_carnet",
                table: "estudiantes",
                column: "carnet",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryId",
                table: "Items",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_materias_nombre",
                table: "materias",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_nombre",
                table: "roles",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sesiones_estado_id",
                table: "sesiones",
                column: "estado_id");

            migrationBuilder.CreateIndex(
                name: "IX_sesiones_estudiante_id",
                table: "sesiones",
                column: "estudiante_id");

            migrationBuilder.CreateIndex(
                name: "IX_sesiones_materia_id",
                table: "sesiones",
                column: "materia_id");

            migrationBuilder.CreateIndex(
                name: "IX_sesiones_tutor_id",
                table: "sesiones",
                column: "tutor_id");

            migrationBuilder.CreateIndex(
                name: "IX_tutores_carnet_id",
                table: "tutores",
                column: "carnet_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tutores_numero_identificacion",
                table: "tutores",
                column: "numero_identificacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tutores_dias_atencion_tutor_id_dia_semana",
                table: "tutores_dias_atencion",
                columns: new[] { "tutor_id", "dia_semana" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tutores_materias_materia_id",
                table: "tutores_materias",
                column: "materia_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_correo",
                table: "usuarios",
                column: "correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_estado_id",
                table: "usuarios",
                column: "estado_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_rol_id",
                table: "usuarios",
                column: "rol_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "administradores");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "sesiones");

            migrationBuilder.DropTable(
                name: "tutores_dias_atencion");

            migrationBuilder.DropTable(
                name: "tutores_materias");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "estados_sesiones");

            migrationBuilder.DropTable(
                name: "estudiantes");

            migrationBuilder.DropTable(
                name: "materias");

            migrationBuilder.DropTable(
                name: "tutores");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "estados_usuarios");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
