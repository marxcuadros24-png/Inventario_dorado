IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE TABLE [Cargo] (
        [CargoId] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(max) NULL,
        [Activo] bit NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        CONSTRAINT [PK_Cargo] PRIMARY KEY ([CargoId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE TABLE [Categoria] (
        [CategoriaId] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Tipo] nvarchar(15) NOT NULL,
        [Descripcion] nvarchar(max) NULL,
        [Activo] bit NOT NULL,
        CONSTRAINT [PK_Categoria] PRIMARY KEY ([CategoriaId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE TABLE [Rol] (
        [RolId] int NOT NULL IDENTITY,
        [Nombre] nvarchar(50) NOT NULL,
        [Descripcion] nvarchar(max) NULL,
        [Activo] bit NOT NULL,
        CONSTRAINT [PK_Rol] PRIMARY KEY ([RolId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE TABLE [TipoMovimiento] (
        [TipoMovId] int NOT NULL IDENTITY,
        [Codigo] nvarchar(10) NOT NULL,
        [Nombre] nvarchar(50) NOT NULL,
        [EsEntrada] bit NOT NULL,
        [Activo] bit NOT NULL,
        CONSTRAINT [PK_TipoMovimiento] PRIMARY KEY ([TipoMovId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE TABLE [UnidadMedida] (
        [UnidadId] int NOT NULL IDENTITY,
        [Codigo] nvarchar(10) NOT NULL,
        [Nombre] nvarchar(50) NOT NULL,
        [Activo] bit NOT NULL,
        CONSTRAINT [PK_UnidadMedida] PRIMARY KEY ([UnidadId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE TABLE [Empleado] (
        [EmpleadoId] int NOT NULL IDENTITY,
        [Codigo] nvarchar(20) NOT NULL,
        [Nombres] nvarchar(100) NOT NULL,
        [Apellidos] nvarchar(100) NOT NULL,
        [DNI] nvarchar(15) NOT NULL,
        [CargoId] int NOT NULL,
        [Area] nvarchar(100) NULL,
        [Telefono] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [FechaIngreso] datetime2 NOT NULL,
        [Activo] bit NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        CONSTRAINT [PK_Empleado] PRIMARY KEY ([EmpleadoId]),
        CONSTRAINT [FK_Empleado_Cargo_CargoId] FOREIGN KEY ([CargoId]) REFERENCES [Cargo] ([CargoId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE TABLE [EPP] (
        [EPPId] int NOT NULL IDENTITY,
        [Codigo] nvarchar(30) NOT NULL,
        [Nombre] nvarchar(200) NOT NULL,
        [Descripcion] nvarchar(max) NULL,
        [CategoriaId] int NOT NULL,
        [UnidadId] int NOT NULL,
        [Marca] nvarchar(max) NULL,
        [Talla] nvarchar(max) NULL,
        [VidaUtilMeses] int NULL,
        [StockMinimo] decimal(12,3) NOT NULL,
        [StockMaximo] decimal(12,3) NULL,
        [StockActual] decimal(12,3) NOT NULL,
        [PrecioUnitario] decimal(12,2) NOT NULL,
        [Activo] bit NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] int NULL,
        CONSTRAINT [PK_EPP] PRIMARY KEY ([EPPId]),
        CONSTRAINT [FK_EPP_Categoria_CategoriaId] FOREIGN KEY ([CategoriaId]) REFERENCES [Categoria] ([CategoriaId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_EPP_UnidadMedida_UnidadId] FOREIGN KEY ([UnidadId]) REFERENCES [UnidadMedida] ([UnidadId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE TABLE [Herramienta] (
        [HerramientaId] int NOT NULL IDENTITY,
        [Codigo] nvarchar(30) NOT NULL,
        [Nombre] nvarchar(200) NOT NULL,
        [Descripcion] nvarchar(max) NULL,
        [CategoriaId] int NOT NULL,
        [UnidadId] int NOT NULL,
        [Marca] nvarchar(max) NULL,
        [Modelo] nvarchar(max) NULL,
        [StockMinimo] decimal(12,3) NOT NULL,
        [StockMaximo] decimal(12,3) NULL,
        [PrecioUnitario] decimal(12,2) NOT NULL,
        [Imagen] nvarchar(max) NULL,
        [Activo] bit NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] int NULL,
        CONSTRAINT [PK_Herramienta] PRIMARY KEY ([HerramientaId]),
        CONSTRAINT [FK_Herramienta_Categoria_CategoriaId] FOREIGN KEY ([CategoriaId]) REFERENCES [Categoria] ([CategoriaId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Herramienta_UnidadMedida_UnidadId] FOREIGN KEY ([UnidadId]) REFERENCES [UnidadMedida] ([UnidadId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE TABLE [Usuario] (
        [UsuarioId] int NOT NULL IDENTITY,
        [EmpleadoId] int NOT NULL,
        [NombreUsuario] nvarchar(50) NOT NULL,
        [PasswordHash] nvarchar(256) NOT NULL,
        [RolId] int NOT NULL,
        [Activo] bit NOT NULL,
        [UltimoAcceso] datetime2 NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] int NULL,
        CONSTRAINT [PK_Usuario] PRIMARY KEY ([UsuarioId]),
        CONSTRAINT [FK_Usuario_Empleado_EmpleadoId] FOREIGN KEY ([EmpleadoId]) REFERENCES [Empleado] ([EmpleadoId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Usuario_Rol_RolId] FOREIGN KEY ([RolId]) REFERENCES [Rol] ([RolId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE TABLE [StockHerramienta] (
        [StockId] int NOT NULL IDENTITY,
        [HerramientaId] int NOT NULL,
        [CantidadActual] decimal(12,3) NOT NULL,
        [CantidadReservada] decimal(12,3) NOT NULL,
        [UltimaActualizacion] datetime2 NOT NULL,
        CONSTRAINT [PK_StockHerramienta] PRIMARY KEY ([StockId]),
        CONSTRAINT [FK_StockHerramienta_Herramienta_HerramientaId] FOREIGN KEY ([HerramientaId]) REFERENCES [Herramienta] ([HerramientaId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE TABLE [AsignacionHerramienta] (
        [AsignacionId] int NOT NULL IDENTITY,
        [Numero] nvarchar(20) NOT NULL,
        [HerramientaId] int NOT NULL,
        [EmpleadoId] int NOT NULL,
        [Cantidad] decimal(12,3) NOT NULL,
        [FechaAsignacion] datetime2 NOT NULL,
        [FechaDevProgramada] datetime2 NULL,
        [FechaDevReal] datetime2 NULL,
        [CondicionSalida] nvarchar(20) NOT NULL,
        [CondicionRetorno] nvarchar(20) NULL,
        [UsuarioId] int NOT NULL,
        [Observaciones] nvarchar(max) NULL,
        [Estado] nvarchar(20) NOT NULL,
        CONSTRAINT [PK_AsignacionHerramienta] PRIMARY KEY ([AsignacionId]),
        CONSTRAINT [FK_AsignacionHerramienta_Empleado_EmpleadoId] FOREIGN KEY ([EmpleadoId]) REFERENCES [Empleado] ([EmpleadoId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AsignacionHerramienta_Herramienta_HerramientaId] FOREIGN KEY ([HerramientaId]) REFERENCES [Herramienta] ([HerramientaId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AsignacionHerramienta_Usuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario] ([UsuarioId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE TABLE [EntregaEPP] (
        [EntregaId] int NOT NULL IDENTITY,
        [Numero] nvarchar(20) NOT NULL,
        [EmpleadoId] int NOT NULL,
        [UsuarioId] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [Motivo] nvarchar(20) NOT NULL,
        [FirmaEmpleado] bit NOT NULL,
        [Observaciones] nvarchar(max) NULL,
        [Anulado] bit NOT NULL,
        CONSTRAINT [PK_EntregaEPP] PRIMARY KEY ([EntregaId]),
        CONSTRAINT [FK_EntregaEPP_Empleado_EmpleadoId] FOREIGN KEY ([EmpleadoId]) REFERENCES [Empleado] ([EmpleadoId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_EntregaEPP_Usuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario] ([UsuarioId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE TABLE [DetalleEntregaEPP] (
        [DetalleEntregaId] int NOT NULL IDENTITY,
        [EntregaId] int NOT NULL,
        [EPPId] int NOT NULL,
        [Cantidad] decimal(12,3) NOT NULL,
        [PrecioUnitario] decimal(12,2) NOT NULL,
        [FechaVencimiento] datetime2 NULL,
        [Observaciones] nvarchar(max) NULL,
        CONSTRAINT [PK_DetalleEntregaEPP] PRIMARY KEY ([DetalleEntregaId]),
        CONSTRAINT [FK_DetalleEntregaEPP_EPP_EPPId] FOREIGN KEY ([EPPId]) REFERENCES [EPP] ([EPPId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DetalleEntregaEPP_EntregaEPP_EntregaId] FOREIGN KEY ([EntregaId]) REFERENCES [EntregaEPP] ([EntregaId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE TABLE [DetalleMovimiento] (
        [DetalleMovId] int NOT NULL IDENTITY,
        [MovimientoId] int NOT NULL,
        [HerramientaId] int NOT NULL,
        [Cantidad] decimal(12,3) NOT NULL,
        [PrecioUnitario] decimal(12,2) NOT NULL,
        [Observaciones] nvarchar(max) NULL,
        CONSTRAINT [PK_DetalleMovimiento] PRIMARY KEY ([DetalleMovId]),
        CONSTRAINT [FK_DetalleMovimiento_Herramienta_HerramientaId] FOREIGN KEY ([HerramientaId]) REFERENCES [Herramienta] ([HerramientaId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE TABLE [DetalleRequerimiento] (
        [DetalleReqId] int NOT NULL IDENTITY,
        [RequerimientoId] int NOT NULL,
        [TipoItem] nvarchar(15) NOT NULL,
        [HerramientaId] int NULL,
        [EPPId] int NULL,
        [StockActual] decimal(12,3) NOT NULL,
        [StockMinimo] decimal(12,3) NOT NULL,
        [CantidadSugerida] decimal(12,3) NOT NULL,
        [CantidadAprobada] decimal(12,3) NULL,
        [Observaciones] nvarchar(max) NULL,
        CONSTRAINT [PK_DetalleRequerimiento] PRIMARY KEY ([DetalleReqId]),
        CONSTRAINT [FK_DetalleRequerimiento_EPP_EPPId] FOREIGN KEY ([EPPId]) REFERENCES [EPP] ([EPPId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DetalleRequerimiento_Herramienta_HerramientaId] FOREIGN KEY ([HerramientaId]) REFERENCES [Herramienta] ([HerramientaId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE TABLE [Movimiento] (
        [MovimientoId] int NOT NULL IDENTITY,
        [Numero] nvarchar(20) NOT NULL,
        [TipoMovId] int NOT NULL,
        [RequerimientoId] int NULL,
        [Fecha] datetime2 NOT NULL,
        [UsuarioId] int NOT NULL,
        [DocumentoRef] nvarchar(max) NULL,
        [Observaciones] nvarchar(max) NULL,
        [Estado] nvarchar(20) NOT NULL,
        [FechaAnulacion] datetime2 NULL,
        [AnuladoPor] int NULL,
        CONSTRAINT [PK_Movimiento] PRIMARY KEY ([MovimientoId]),
        CONSTRAINT [FK_Movimiento_TipoMovimiento_TipoMovId] FOREIGN KEY ([TipoMovId]) REFERENCES [TipoMovimiento] ([TipoMovId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Movimiento_Usuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario] ([UsuarioId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE TABLE [Requerimiento] (
        [RequerimientoId] int NOT NULL IDENTITY,
        [Numero] nvarchar(20) NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [UsuarioId] int NOT NULL,
        [Area] nvarchar(max) NULL,
        [Observaciones] nvarchar(max) NULL,
        [Estado] nvarchar(20) NOT NULL,
        [FechaImpresion] datetime2 NULL,
        [FechaAtencion] datetime2 NULL,
        [MovimientoId] int NULL,
        [EmpleadoId] int NULL,
        CONSTRAINT [PK_Requerimiento] PRIMARY KEY ([RequerimientoId]),
        CONSTRAINT [FK_Requerimiento_Empleado_EmpleadoId] FOREIGN KEY ([EmpleadoId]) REFERENCES [Empleado] ([EmpleadoId]),
        CONSTRAINT [FK_Requerimiento_Movimiento_MovimientoId] FOREIGN KEY ([MovimientoId]) REFERENCES [Movimiento] ([MovimientoId]) ON DELETE SET NULL,
        CONSTRAINT [FK_Requerimiento_Usuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario] ([UsuarioId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CargoId', N'Activo', N'Descripcion', N'FechaCreacion', N'Nombre') AND [object_id] = OBJECT_ID(N'[Cargo]'))
        SET IDENTITY_INSERT [Cargo] ON;
    EXEC(N'INSERT INTO [Cargo] ([CargoId], [Activo], [Descripcion], [FechaCreacion], [Nombre])
    VALUES (1, CAST(1 AS bit), NULL, ''2026-10-16T18:45:36.0412381-05:00'', N''Jefe de Almacén''),
    (2, CAST(1 AS bit), NULL, ''2026-10-16T18:45:36.0412397-05:00'', N''Almacenero''),
    (3, CAST(1 AS bit), NULL, ''2026-10-16T18:45:36.0412398-05:00'', N''Supervisor de Mina''),
    (4, CAST(1 AS bit), NULL, ''2026-10-16T18:45:36.0412399-05:00'', N''Operador de Equipo''),
    (5, CAST(1 AS bit), NULL, ''2026-10-16T18:45:36.0412400-05:00'', N''Técnico de Mantenimiento''),
    (6, CAST(1 AS bit), NULL, ''2026-10-16T18:45:36.0412401-05:00'', N''Gerente de Operaciones'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CargoId', N'Activo', N'Descripcion', N'FechaCreacion', N'Nombre') AND [object_id] = OBJECT_ID(N'[Cargo]'))
        SET IDENTITY_INSERT [Cargo] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CategoriaId', N'Activo', N'Descripcion', N'Nombre', N'Tipo') AND [object_id] = OBJECT_ID(N'[Categoria]'))
        SET IDENTITY_INSERT [Categoria] ON;
    EXEC(N'INSERT INTO [Categoria] ([CategoriaId], [Activo], [Descripcion], [Nombre], [Tipo])
    VALUES (1, CAST(1 AS bit), NULL, N''Protección de Cabeza'', N''EPP''),
    (2, CAST(1 AS bit), NULL, N''Protección Visual'', N''EPP''),
    (3, CAST(1 AS bit), NULL, N''Protección Auditiva'', N''EPP''),
    (4, CAST(1 AS bit), NULL, N''Protección Respiratoria'', N''EPP''),
    (5, CAST(1 AS bit), NULL, N''Protección de Manos'', N''EPP''),
    (6, CAST(1 AS bit), NULL, N''Protección de Pies'', N''EPP''),
    (7, CAST(1 AS bit), NULL, N''Ropa de Trabajo'', N''EPP''),
    (8, CAST(1 AS bit), NULL, N''Herramientas Manuales'', N''Herramienta''),
    (9, CAST(1 AS bit), NULL, N''Herramientas Eléctricas'', N''Herramienta''),
    (10, CAST(1 AS bit), NULL, N''Herramientas de Medición'', N''Herramienta''),
    (11, CAST(1 AS bit), NULL, N''Equipos de Izaje'', N''Herramienta''),
    (12, CAST(1 AS bit), NULL, N''Instrumentos de Perforación'', N''Herramienta'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CategoriaId', N'Activo', N'Descripcion', N'Nombre', N'Tipo') AND [object_id] = OBJECT_ID(N'[Categoria]'))
        SET IDENTITY_INSERT [Categoria] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RolId', N'Activo', N'Descripcion', N'Nombre') AND [object_id] = OBJECT_ID(N'[Rol]'))
        SET IDENTITY_INSERT [Rol] ON;
    EXEC(N'INSERT INTO [Rol] ([RolId], [Activo], [Descripcion], [Nombre])
    VALUES (1, CAST(1 AS bit), N''Acceso total al sistema'', N''Administrador''),
    (2, CAST(1 AS bit), N''Gestión de entradas, salidas y EPP'', N''Almacenero''),
    (3, CAST(1 AS bit), N''Aprobación y reportes'', N''Supervisor'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RolId', N'Activo', N'Descripcion', N'Nombre') AND [object_id] = OBJECT_ID(N'[Rol]'))
        SET IDENTITY_INSERT [Rol] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'TipoMovId', N'Activo', N'Codigo', N'EsEntrada', N'Nombre') AND [object_id] = OBJECT_ID(N'[TipoMovimiento]'))
        SET IDENTITY_INSERT [TipoMovimiento] ON;
    EXEC(N'INSERT INTO [TipoMovimiento] ([TipoMovId], [Activo], [Codigo], [EsEntrada], [Nombre])
    VALUES (1, CAST(1 AS bit), N''ENT'', CAST(1 AS bit), N''Entrada por Compra''),
    (2, CAST(1 AS bit), N''SAL'', CAST(0 AS bit), N''Salida por Consumo''),
    (3, CAST(1 AS bit), N''AJU'', CAST(1 AS bit), N''Ajuste de Inventario''),
    (4, CAST(1 AS bit), N''DEV'', CAST(1 AS bit), N''Devolución a Almacén'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'TipoMovId', N'Activo', N'Codigo', N'EsEntrada', N'Nombre') AND [object_id] = OBJECT_ID(N'[TipoMovimiento]'))
        SET IDENTITY_INSERT [TipoMovimiento] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UnidadId', N'Activo', N'Codigo', N'Nombre') AND [object_id] = OBJECT_ID(N'[UnidadMedida]'))
        SET IDENTITY_INSERT [UnidadMedida] ON;
    EXEC(N'INSERT INTO [UnidadMedida] ([UnidadId], [Activo], [Codigo], [Nombre])
    VALUES (1, CAST(1 AS bit), N''UND'', N''Unidad''),
    (2, CAST(1 AS bit), N''PAR'', N''Par''),
    (3, CAST(1 AS bit), N''JGO'', N''Juego''),
    (4, CAST(1 AS bit), N''MT'', N''Metro''),
    (5, CAST(1 AS bit), N''KG'', N''Kilogramo''),
    (6, CAST(1 AS bit), N''CJA'', N''Caja'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UnidadId', N'Activo', N'Codigo', N'Nombre') AND [object_id] = OBJECT_ID(N'[UnidadMedida]'))
        SET IDENTITY_INSERT [UnidadMedida] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AsignacionHerramienta_EmpleadoId] ON [AsignacionHerramienta] ([EmpleadoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AsignacionHerramienta_HerramientaId] ON [AsignacionHerramienta] ([HerramientaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AsignacionHerramienta_Numero] ON [AsignacionHerramienta] ([Numero]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AsignacionHerramienta_UsuarioId] ON [AsignacionHerramienta] ([UsuarioId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Cargo_Nombre] ON [Cargo] ([Nombre]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Categoria_Nombre] ON [Categoria] ([Nombre]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_DetalleEntregaEPP_EntregaId] ON [DetalleEntregaEPP] ([EntregaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_DetalleEntregaEPP_EPPId] ON [DetalleEntregaEPP] ([EPPId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_DetalleMovimiento_HerramientaId] ON [DetalleMovimiento] ([HerramientaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_DetalleMovimiento_MovimientoId] ON [DetalleMovimiento] ([MovimientoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_DetalleRequerimiento_EPPId] ON [DetalleRequerimiento] ([EPPId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_DetalleRequerimiento_HerramientaId] ON [DetalleRequerimiento] ([HerramientaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_DetalleRequerimiento_RequerimientoId] ON [DetalleRequerimiento] ([RequerimientoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Empleado_CargoId] ON [Empleado] ([CargoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Empleado_Codigo] ON [Empleado] ([Codigo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Empleado_DNI] ON [Empleado] ([DNI]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_EntregaEPP_EmpleadoId] ON [EntregaEPP] ([EmpleadoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EntregaEPP_Numero] ON [EntregaEPP] ([Numero]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_EntregaEPP_UsuarioId] ON [EntregaEPP] ([UsuarioId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_EPP_CategoriaId] ON [EPP] ([CategoriaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EPP_Codigo] ON [EPP] ([Codigo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_EPP_UnidadId] ON [EPP] ([UnidadId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Herramienta_CategoriaId] ON [Herramienta] ([CategoriaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Herramienta_Codigo] ON [Herramienta] ([Codigo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Herramienta_UnidadId] ON [Herramienta] ([UnidadId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Movimiento_Numero] ON [Movimiento] ([Numero]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Movimiento_RequerimientoId] ON [Movimiento] ([RequerimientoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Movimiento_TipoMovId] ON [Movimiento] ([TipoMovId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Movimiento_UsuarioId] ON [Movimiento] ([UsuarioId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Requerimiento_EmpleadoId] ON [Requerimiento] ([EmpleadoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Requerimiento_MovimientoId] ON [Requerimiento] ([MovimientoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Requerimiento_Numero] ON [Requerimiento] ([Numero]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Requerimiento_UsuarioId] ON [Requerimiento] ([UsuarioId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Rol_Nombre] ON [Rol] ([Nombre]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StockHerramienta_HerramientaId] ON [StockHerramienta] ([HerramientaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TipoMovimiento_Codigo] ON [TipoMovimiento] ([Codigo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UnidadMedida_Codigo] ON [UnidadMedida] ([Codigo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuario_EmpleadoId] ON [Usuario] ([EmpleadoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuario_NombreUsuario] ON [Usuario] ([NombreUsuario]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Usuario_RolId] ON [Usuario] ([RolId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    ALTER TABLE [DetalleMovimiento] ADD CONSTRAINT [FK_DetalleMovimiento_Movimiento_MovimientoId] FOREIGN KEY ([MovimientoId]) REFERENCES [Movimiento] ([MovimientoId]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    ALTER TABLE [DetalleRequerimiento] ADD CONSTRAINT [FK_DetalleRequerimiento_Requerimiento_RequerimientoId] FOREIGN KEY ([RequerimientoId]) REFERENCES [Requerimiento] ([RequerimientoId]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    ALTER TABLE [Movimiento] ADD CONSTRAINT [FK_Movimiento_Requerimiento_RequerimientoId] FOREIGN KEY ([RequerimientoId]) REFERENCES [Requerimiento] ([RequerimientoId]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016234536_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261016234536_InitialCreate', N'8.0.10');
END;
GO

COMMIT;
GO

