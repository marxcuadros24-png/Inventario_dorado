using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InventarioMinero.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cargo",
                columns: table => new
                {
                    CargoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargo", x => x.CargoId);
                });

            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    CategoriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.CategoriaId);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    RolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.RolId);
                });

            migrationBuilder.CreateTable(
                name: "TipoMovimiento",
                columns: table => new
                {
                    TipoMovId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EsEntrada = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoMovimiento", x => x.TipoMovId);
                });

            migrationBuilder.CreateTable(
                name: "UnidadMedida",
                columns: table => new
                {
                    UnidadId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadMedida", x => x.UnidadId);
                });

            migrationBuilder.CreateTable(
                name: "Empleado",
                columns: table => new
                {
                    EmpleadoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DNI = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CargoId = table.Column<int>(type: "int", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleado", x => x.EmpleadoId);
                    table.ForeignKey(
                        name: "FK_Empleado_Cargo_CargoId",
                        column: x => x.CargoId,
                        principalTable: "Cargo",
                        principalColumn: "CargoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EPP",
                columns: table => new
                {
                    EPPId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    UnidadId = table.Column<int>(type: "int", nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Talla = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VidaUtilMeses = table.Column<int>(type: "int", nullable: true),
                    StockMinimo = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    StockMaximo = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: true),
                    StockActual = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EPP", x => x.EPPId);
                    table.ForeignKey(
                        name: "FK_EPP_Categoria_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categoria",
                        principalColumn: "CategoriaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EPP_UnidadMedida_UnidadId",
                        column: x => x.UnidadId,
                        principalTable: "UnidadMedida",
                        principalColumn: "UnidadId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Herramienta",
                columns: table => new
                {
                    HerramientaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    UnidadId = table.Column<int>(type: "int", nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modelo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StockMinimo = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    StockMaximo = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: true),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Imagen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Herramienta", x => x.HerramientaId);
                    table.ForeignKey(
                        name: "FK_Herramienta_Categoria_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categoria",
                        principalColumn: "CategoriaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Herramienta_UnidadMedida_UnidadId",
                        column: x => x.UnidadId,
                        principalTable: "UnidadMedida",
                        principalColumn: "UnidadId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    UltimoAcceso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.UsuarioId);
                    table.ForeignKey(
                        name: "FK_Usuario_Empleado_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleado",
                        principalColumn: "EmpleadoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuario_Rol_RolId",
                        column: x => x.RolId,
                        principalTable: "Rol",
                        principalColumn: "RolId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockHerramienta",
                columns: table => new
                {
                    StockId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HerramientaId = table.Column<int>(type: "int", nullable: false),
                    CantidadActual = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    CantidadReservada = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockHerramienta", x => x.StockId);
                    table.ForeignKey(
                        name: "FK_StockHerramienta_Herramienta_HerramientaId",
                        column: x => x.HerramientaId,
                        principalTable: "Herramienta",
                        principalColumn: "HerramientaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AsignacionHerramienta",
                columns: table => new
                {
                    AsignacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HerramientaId = table.Column<int>(type: "int", nullable: false),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaDevProgramada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaDevReal = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CondicionSalida = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CondicionRetorno = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignacionHerramienta", x => x.AsignacionId);
                    table.ForeignKey(
                        name: "FK_AsignacionHerramienta_Empleado_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleado",
                        principalColumn: "EmpleadoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsignacionHerramienta_Herramienta_HerramientaId",
                        column: x => x.HerramientaId,
                        principalTable: "Herramienta",
                        principalColumn: "HerramientaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsignacionHerramienta_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntregaEPP",
                columns: table => new
                {
                    EntregaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FirmaEmpleado = table.Column<bool>(type: "bit", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Anulado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntregaEPP", x => x.EntregaId);
                    table.ForeignKey(
                        name: "FK_EntregaEPP_Empleado_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleado",
                        principalColumn: "EmpleadoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntregaEPP_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetalleEntregaEPP",
                columns: table => new
                {
                    DetalleEntregaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntregaId = table.Column<int>(type: "int", nullable: false),
                    EPPId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleEntregaEPP", x => x.DetalleEntregaId);
                    table.ForeignKey(
                        name: "FK_DetalleEntregaEPP_EPP_EPPId",
                        column: x => x.EPPId,
                        principalTable: "EPP",
                        principalColumn: "EPPId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleEntregaEPP_EntregaEPP_EntregaId",
                        column: x => x.EntregaId,
                        principalTable: "EntregaEPP",
                        principalColumn: "EntregaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleMovimiento",
                columns: table => new
                {
                    DetalleMovId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovimientoId = table.Column<int>(type: "int", nullable: false),
                    HerramientaId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleMovimiento", x => x.DetalleMovId);
                    table.ForeignKey(
                        name: "FK_DetalleMovimiento_Herramienta_HerramientaId",
                        column: x => x.HerramientaId,
                        principalTable: "Herramienta",
                        principalColumn: "HerramientaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetalleRequerimiento",
                columns: table => new
                {
                    DetalleReqId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequerimientoId = table.Column<int>(type: "int", nullable: false),
                    TipoItem = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    HerramientaId = table.Column<int>(type: "int", nullable: true),
                    EPPId = table.Column<int>(type: "int", nullable: true),
                    StockActual = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    StockMinimo = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    CantidadSugerida = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    CantidadAprobada = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleRequerimiento", x => x.DetalleReqId);
                    table.ForeignKey(
                        name: "FK_DetalleRequerimiento_EPP_EPPId",
                        column: x => x.EPPId,
                        principalTable: "EPP",
                        principalColumn: "EPPId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleRequerimiento_Herramienta_HerramientaId",
                        column: x => x.HerramientaId,
                        principalTable: "Herramienta",
                        principalColumn: "HerramientaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Movimiento",
                columns: table => new
                {
                    MovimientoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TipoMovId = table.Column<int>(type: "int", nullable: false),
                    RequerimientoId = table.Column<int>(type: "int", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    DocumentoRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaAnulacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnuladoPor = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimiento", x => x.MovimientoId);
                    table.ForeignKey(
                        name: "FK_Movimiento_TipoMovimiento_TipoMovId",
                        column: x => x.TipoMovId,
                        principalTable: "TipoMovimiento",
                        principalColumn: "TipoMovId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Movimiento_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Requerimiento",
                columns: table => new
                {
                    RequerimientoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaImpresion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaAtencion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MovimientoId = table.Column<int>(type: "int", nullable: true),
                    EmpleadoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requerimiento", x => x.RequerimientoId);
                    table.ForeignKey(
                        name: "FK_Requerimiento_Empleado_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleado",
                        principalColumn: "EmpleadoId");
                    table.ForeignKey(
                        name: "FK_Requerimiento_Movimiento_MovimientoId",
                        column: x => x.MovimientoId,
                        principalTable: "Movimiento",
                        principalColumn: "MovimientoId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Requerimiento_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Cargo",
                columns: new[] { "CargoId", "Activo", "Descripcion", "FechaCreacion", "Nombre" },
                values: new object[,]
                {
                    { 1, true, null, new DateTime(2026, 10, 16, 18, 45, 36, 41, DateTimeKind.Local).AddTicks(2381), "Jefe de Almacén" },
                    { 2, true, null, new DateTime(2026, 10, 16, 18, 45, 36, 41, DateTimeKind.Local).AddTicks(2397), "Almacenero" },
                    { 3, true, null, new DateTime(2026, 10, 16, 18, 45, 36, 41, DateTimeKind.Local).AddTicks(2398), "Supervisor de Mina" },
                    { 4, true, null, new DateTime(2026, 10, 16, 18, 45, 36, 41, DateTimeKind.Local).AddTicks(2399), "Operador de Equipo" },
                    { 5, true, null, new DateTime(2026, 10, 16, 18, 45, 36, 41, DateTimeKind.Local).AddTicks(2400), "Técnico de Mantenimiento" },
                    { 6, true, null, new DateTime(2026, 10, 16, 18, 45, 36, 41, DateTimeKind.Local).AddTicks(2401), "Gerente de Operaciones" }
                });

            migrationBuilder.InsertData(
                table: "Categoria",
                columns: new[] { "CategoriaId", "Activo", "Descripcion", "Nombre", "Tipo" },
                values: new object[,]
                {
                    { 1, true, null, "Protección de Cabeza", "EPP" },
                    { 2, true, null, "Protección Visual", "EPP" },
                    { 3, true, null, "Protección Auditiva", "EPP" },
                    { 4, true, null, "Protección Respiratoria", "EPP" },
                    { 5, true, null, "Protección de Manos", "EPP" },
                    { 6, true, null, "Protección de Pies", "EPP" },
                    { 7, true, null, "Ropa de Trabajo", "EPP" },
                    { 8, true, null, "Herramientas Manuales", "Herramienta" },
                    { 9, true, null, "Herramientas Eléctricas", "Herramienta" },
                    { 10, true, null, "Herramientas de Medición", "Herramienta" },
                    { 11, true, null, "Equipos de Izaje", "Herramienta" },
                    { 12, true, null, "Instrumentos de Perforación", "Herramienta" }
                });

            migrationBuilder.InsertData(
                table: "Rol",
                columns: new[] { "RolId", "Activo", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Acceso total al sistema", "Administrador" },
                    { 2, true, "Gestión de entradas, salidas y EPP", "Almacenero" },
                    { 3, true, "Aprobación y reportes", "Supervisor" }
                });

            migrationBuilder.InsertData(
                table: "TipoMovimiento",
                columns: new[] { "TipoMovId", "Activo", "Codigo", "EsEntrada", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "ENT", true, "Entrada por Compra" },
                    { 2, true, "SAL", false, "Salida por Consumo" },
                    { 3, true, "AJU", true, "Ajuste de Inventario" },
                    { 4, true, "DEV", true, "Devolución a Almacén" }
                });

            migrationBuilder.InsertData(
                table: "UnidadMedida",
                columns: new[] { "UnidadId", "Activo", "Codigo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "UND", "Unidad" },
                    { 2, true, "PAR", "Par" },
                    { 3, true, "JGO", "Juego" },
                    { 4, true, "MT", "Metro" },
                    { 5, true, "KG", "Kilogramo" },
                    { 6, true, "CJA", "Caja" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionHerramienta_EmpleadoId",
                table: "AsignacionHerramienta",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionHerramienta_HerramientaId",
                table: "AsignacionHerramienta",
                column: "HerramientaId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionHerramienta_Numero",
                table: "AsignacionHerramienta",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionHerramienta_UsuarioId",
                table: "AsignacionHerramienta",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Cargo_Nombre",
                table: "Cargo",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_Nombre",
                table: "Categoria",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetalleEntregaEPP_EntregaId",
                table: "DetalleEntregaEPP",
                column: "EntregaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleEntregaEPP_EPPId",
                table: "DetalleEntregaEPP",
                column: "EPPId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleMovimiento_HerramientaId",
                table: "DetalleMovimiento",
                column: "HerramientaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleMovimiento_MovimientoId",
                table: "DetalleMovimiento",
                column: "MovimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleRequerimiento_EPPId",
                table: "DetalleRequerimiento",
                column: "EPPId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleRequerimiento_HerramientaId",
                table: "DetalleRequerimiento",
                column: "HerramientaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleRequerimiento_RequerimientoId",
                table: "DetalleRequerimiento",
                column: "RequerimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Empleado_CargoId",
                table: "Empleado",
                column: "CargoId");

            migrationBuilder.CreateIndex(
                name: "IX_Empleado_Codigo",
                table: "Empleado",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empleado_DNI",
                table: "Empleado",
                column: "DNI",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntregaEPP_EmpleadoId",
                table: "EntregaEPP",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_EntregaEPP_Numero",
                table: "EntregaEPP",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntregaEPP_UsuarioId",
                table: "EntregaEPP",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_EPP_CategoriaId",
                table: "EPP",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_EPP_Codigo",
                table: "EPP",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EPP_UnidadId",
                table: "EPP",
                column: "UnidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Herramienta_CategoriaId",
                table: "Herramienta",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Herramienta_Codigo",
                table: "Herramienta",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Herramienta_UnidadId",
                table: "Herramienta",
                column: "UnidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimiento_Numero",
                table: "Movimiento",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movimiento_RequerimientoId",
                table: "Movimiento",
                column: "RequerimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimiento_TipoMovId",
                table: "Movimiento",
                column: "TipoMovId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimiento_UsuarioId",
                table: "Movimiento",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Requerimiento_EmpleadoId",
                table: "Requerimiento",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Requerimiento_MovimientoId",
                table: "Requerimiento",
                column: "MovimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Requerimiento_Numero",
                table: "Requerimiento",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requerimiento_UsuarioId",
                table: "Requerimiento",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Rol_Nombre",
                table: "Rol",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockHerramienta_HerramientaId",
                table: "StockHerramienta",
                column: "HerramientaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TipoMovimiento_Codigo",
                table: "TipoMovimiento",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnidadMedida_Codigo",
                table: "UnidadMedida",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_EmpleadoId",
                table: "Usuario",
                column: "EmpleadoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_NombreUsuario",
                table: "Usuario",
                column: "NombreUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_RolId",
                table: "Usuario",
                column: "RolId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleMovimiento_Movimiento_MovimientoId",
                table: "DetalleMovimiento",
                column: "MovimientoId",
                principalTable: "Movimiento",
                principalColumn: "MovimientoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleRequerimiento_Requerimiento_RequerimientoId",
                table: "DetalleRequerimiento",
                column: "RequerimientoId",
                principalTable: "Requerimiento",
                principalColumn: "RequerimientoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimiento_Requerimiento_RequerimientoId",
                table: "Movimiento",
                column: "RequerimientoId",
                principalTable: "Requerimiento",
                principalColumn: "RequerimientoId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requerimiento_Empleado_EmpleadoId",
                table: "Requerimiento");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Empleado_EmpleadoId",
                table: "Usuario");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimiento_Usuario_UsuarioId",
                table: "Movimiento");

            migrationBuilder.DropForeignKey(
                name: "FK_Requerimiento_Usuario_UsuarioId",
                table: "Requerimiento");

            migrationBuilder.DropForeignKey(
                name: "FK_Requerimiento_Movimiento_MovimientoId",
                table: "Requerimiento");

            migrationBuilder.DropTable(
                name: "AsignacionHerramienta");

            migrationBuilder.DropTable(
                name: "DetalleEntregaEPP");

            migrationBuilder.DropTable(
                name: "DetalleMovimiento");

            migrationBuilder.DropTable(
                name: "DetalleRequerimiento");

            migrationBuilder.DropTable(
                name: "StockHerramienta");

            migrationBuilder.DropTable(
                name: "EntregaEPP");

            migrationBuilder.DropTable(
                name: "EPP");

            migrationBuilder.DropTable(
                name: "Herramienta");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "UnidadMedida");

            migrationBuilder.DropTable(
                name: "Empleado");

            migrationBuilder.DropTable(
                name: "Cargo");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Rol");

            migrationBuilder.DropTable(
                name: "Movimiento");

            migrationBuilder.DropTable(
                name: "Requerimiento");

            migrationBuilder.DropTable(
                name: "TipoMovimiento");
        }
    }
}
