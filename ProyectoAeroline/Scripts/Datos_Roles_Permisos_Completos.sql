-- =============================================
-- Script: Datos Completos - Roles, Pantallas y Permisos
-- Descripción: 
--   - Borra datos existentes de Roles, Pantallas y RolPantallaOpcion (lógica)
--   - Crea los nuevos roles: SuperAdmin, Admin, Recepcion, Auditoria, Usuario, Mantenimiento, Consulta
--   - Crea todas las pantallas del sistema
--   - Configura permisos detallados para cada rol según la lógica especificada
-- Fecha: 2025-01-11
-- =============================================

USE [AerolineaPruebaDB]
GO

SET NOCOUNT ON;

BEGIN TRANSACTION;

-- =============================================
-- 1. ELIMINAR DATOS EXISTENTES (lógica)
-- =============================================

PRINT '1. Eliminando datos existentes...';

-- Eliminar permisos (RolPantallaOpcion) primero debido a foreign keys
UPDATE [dbo].[RolPantallaOpcion]
SET [FechaEliminacion] = GETDATE(), 
    [FechaActualizacion] = GETDATE(),
    [HoraActualizacion] = CAST(GETDATE() AS TIME(7)),
    [UsuarioActualizacion] = 'SYSTEM'
WHERE [FechaEliminacion] IS NULL;

-- Eliminar pantallas (lógica)
UPDATE [dbo].[Pantallas]
SET [FechaEliminacion] = GETDATE(),
    [FechaActualizacion] = GETDATE(),
    [HoraActualizacion] = CAST(GETDATE() AS TIME(7)),
    [UsuarioActualizacion] = 'SYSTEM'
WHERE [FechaEliminacion] IS NULL;

-- Eliminar roles (lógica)
UPDATE [dbo].[Roles]
SET [FechaEliminacion] = GETDATE(),
    [FechaActualizacion] = GETDATE(),
    [HoraActualizacion] = CAST(GETDATE() AS TIME(7)),
    [UsuarioActualizacion] = 'SYSTEM'
WHERE [FechaEliminacion] IS NULL;

PRINT '   Datos existentes eliminados lógicamente.';

-- =============================================
-- 2. VERIFICAR/INSERTAR OPCIONES
-- =============================================

PRINT '2. Verificando opciones de permiso...';

IF NOT EXISTS (SELECT 1 FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Ver')
BEGIN
    INSERT INTO [dbo].[Opciones] ([NombreOpcion], [FechaCreacion], [HoraCreacion])
    VALUES ('Ver', GETDATE(), CAST(GETDATE() AS TIME(7)));
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Crear')
BEGIN
    INSERT INTO [dbo].[Opciones] ([NombreOpcion], [FechaCreacion], [HoraCreacion])
    VALUES ('Crear', GETDATE(), CAST(GETDATE() AS TIME(7)));
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Editar')
BEGIN
    INSERT INTO [dbo].[Opciones] ([NombreOpcion], [FechaCreacion], [HoraCreacion])
    VALUES ('Editar', GETDATE(), CAST(GETDATE() AS TIME(7)));
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Eliminar')
BEGIN
    INSERT INTO [dbo].[Opciones] ([NombreOpcion], [FechaCreacion], [HoraCreacion])
    VALUES ('Eliminar', GETDATE(), CAST(GETDATE() AS TIME(7)));
END

-- Obtener IDs de opciones
DECLARE @IdOpcionVer INT = (SELECT [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Ver');
DECLARE @IdOpcionCrear INT = (SELECT [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Crear');
DECLARE @IdOpcionEditar INT = (SELECT [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Editar');
DECLARE @IdOpcionEliminar INT = (SELECT [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Eliminar');

PRINT '   Opciones verificadas. IDs: Ver=' + CAST(@IdOpcionVer AS VARCHAR) + ', Crear=' + CAST(@IdOpcionCrear AS VARCHAR) + ', Editar=' + CAST(@IdOpcionEditar AS VARCHAR) + ', Eliminar=' + CAST(@IdOpcionEliminar AS VARCHAR);

-- =============================================
-- 3. INSERTAR ROLES
-- =============================================

PRINT '3. Insertando nuevos roles...';

-- Declarar variables para IDs de roles (antes del bloque TRY/CATCH)
DECLARE @IdRolSuperAdmin INT;
DECLARE @IdRolAdmin INT;
DECLARE @IdRolRecepcion INT;
DECLARE @IdRolAuditoria INT;
DECLARE @IdRolUsuario INT;
DECLARE @IdRolMantenimiento INT;
DECLARE @IdRolConsulta INT;

-- Insertar roles directamente (intentando con IDENTITY_INSERT primero)
BEGIN TRY
    SET IDENTITY_INSERT [dbo].[Roles] ON;
    
    INSERT INTO [dbo].[Roles] ([IdRol], [NombreRol], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES
    (1, 'SuperAdmin', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
    (2, 'Admin', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
    (3, 'Recepcion', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
    (4, 'Auditoria', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
    (5, 'Usuario', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
    (6, 'Mantenimiento', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
    (7, 'Consulta', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));

    SET IDENTITY_INSERT [dbo].[Roles] OFF;
    
    SET @IdRolSuperAdmin = 1;
    SET @IdRolAdmin = 2;
    SET @IdRolRecepcion = 3;
    SET @IdRolAuditoria = 4;
    SET @IdRolUsuario = 5;
    SET @IdRolMantenimiento = 6;
    SET @IdRolConsulta = 7;
END TRY
BEGIN CATCH
    -- Si falla porque IdRol es IDENTITY o no se puede usar IDENTITY_INSERT, insertar sin especificar ID
    IF @@TRANCOUNT > 0
        SET IDENTITY_INSERT [dbo].[Roles] OFF;
    
    INSERT INTO [dbo].[Roles] ([NombreRol], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES
    ('SuperAdmin', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
    ('Admin', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
    ('Recepcion', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
    ('Auditoria', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
    ('Usuario', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
    ('Mantenimiento', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
    ('Consulta', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
    
    -- Obtener IDs generados
    SET @IdRolSuperAdmin = (SELECT [IdRol] FROM [dbo].[Roles] WHERE [NombreRol] = 'SuperAdmin' AND [FechaEliminacion] IS NULL);
    SET @IdRolAdmin = (SELECT [IdRol] FROM [dbo].[Roles] WHERE [NombreRol] = 'Admin' AND [FechaEliminacion] IS NULL);
    SET @IdRolRecepcion = (SELECT [IdRol] FROM [dbo].[Roles] WHERE [NombreRol] = 'Recepcion' AND [FechaEliminacion] IS NULL);
    SET @IdRolAuditoria = (SELECT [IdRol] FROM [dbo].[Roles] WHERE [NombreRol] = 'Auditoria' AND [FechaEliminacion] IS NULL);
    SET @IdRolUsuario = (SELECT [IdRol] FROM [dbo].[Roles] WHERE [NombreRol] = 'Usuario' AND [FechaEliminacion] IS NULL);
    SET @IdRolMantenimiento = (SELECT [IdRol] FROM [dbo].[Roles] WHERE [NombreRol] = 'Mantenimiento' AND [FechaEliminacion] IS NULL);
    SET @IdRolConsulta = (SELECT [IdRol] FROM [dbo].[Roles] WHERE [NombreRol] = 'Consulta' AND [FechaEliminacion] IS NULL);
END CATCH

PRINT '   Roles insertados. IDs: SuperAdmin=' + CAST(@IdRolSuperAdmin AS VARCHAR) + ', Admin=' + CAST(@IdRolAdmin AS VARCHAR) + ', Recepcion=' + CAST(@IdRolRecepcion AS VARCHAR) + ', Auditoria=' + CAST(@IdRolAuditoria AS VARCHAR) + ', Usuario=' + CAST(@IdRolUsuario AS VARCHAR) + ', Mantenimiento=' + CAST(@IdRolMantenimiento AS VARCHAR) + ', Consulta=' + CAST(@IdRolConsulta AS VARCHAR);

-- =============================================
-- 4. INSERTAR/ACTUALIZAR PANTALLAS
-- =============================================

PRINT '4. Insertando/actualizando pantallas...';

-- Crear tabla temporal con todas las pantallas
DECLARE @PantallasTemp TABLE (NombrePantalla NVARCHAR(100));
INSERT INTO @PantallasTemp (NombrePantalla) VALUES
('Empleados'), ('Usuarios'), ('Roles'), ('Pantallas'), ('RolPantallaOpcion'),
('Aeropuertos'), ('Aviones'), ('Mantenimientos'), ('Aerolineas'), ('Vuelos'),
('Horarios'), ('Escalas'), ('Pasajeros'), ('Boletos'), ('Equipaje'),
('Servicios'), ('Reservas'), ('Facturacion'), ('Historiales');

-- Usar MERGE para insertar o actualizar pantallas
MERGE [dbo].[Pantallas] AS Target
USING @PantallasTemp AS Source
ON Target.[NombrePantalla] = Source.[NombrePantalla]
WHEN NOT MATCHED BY Target THEN
    INSERT ([NombrePantalla], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES (Source.[NombrePantalla], 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)))
WHEN MATCHED THEN
    UPDATE SET
        [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [HoraActualizacion] = CAST(GETDATE() AS TIME(7)),
        [UsuarioActualizacion] = 'SYSTEM';

PRINT '   Pantallas insertadas/actualizadas.';

-- Obtener IDs de pantallas (se usarán en la configuración de permisos)
DECLARE @IdPantallaEmpleados INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Empleados' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaUsuarios INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Usuarios' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaRoles INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Roles' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaPantallas INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Pantallas' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaRolPantallaOpcion INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'RolPantallaOpcion' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaAeropuertos INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Aeropuertos' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaAviones INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Aviones' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaMantenimientos INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Mantenimientos' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaAerolineas INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Aerolineas' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaVuelos INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Vuelos' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaHorarios INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Horarios' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaEscalas INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Escalas' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaPasajeros INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Pasajeros' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaBoletos INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Boletos' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaEquipaje INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Equipaje' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaServicios INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Servicios' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaReservas INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Reservas' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaFacturacion INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Facturacion' AND [FechaEliminacion] IS NULL);
DECLARE @IdPantallaHistoriales INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Historiales' AND [FechaEliminacion] IS NULL);

-- =============================================
-- 5. CONFIGURAR PERMISOS
-- =============================================

PRINT '5. Configurando permisos...';

-- Nota: En lugar de un procedimiento temporal, usaremos lógica inline para evitar problemas con GO dentro de transacciones

-- =============================================
-- 6. CONFIGURAR PERMISOS PARA CADA ROL
-- =============================================

-- =============================================
-- 6.1. ROL 1: SuperAdmin - TODAS LAS PANTALLAS: L/G/M/E
-- =============================================
PRINT '   Configurando permisos para SuperAdmin...';

DECLARE @CurrentPantallaId INT;
DECLARE Pantallas_SuperAdmin_Cursor CURSOR FOR
SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [FechaEliminacion] IS NULL;

OPEN Pantallas_SuperAdmin_Cursor;
FETCH NEXT FROM Pantallas_SuperAdmin_Cursor INTO @CurrentPantallaId;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Insertar/Actualizar permiso Ver
    MERGE [dbo].[RolPantallaOpcion] AS Target
    USING (SELECT @IdRolSuperAdmin AS IdRol, @CurrentPantallaId AS IdPantalla, @IdOpcionVer AS IdOpcion) AS Source
    ON Target.[IdRol] = Source.[IdRol] AND Target.[IdPantalla] = Source.[IdPantalla] AND Target.[IdOpcion] = Source.[IdOpcion]
    WHEN NOT MATCHED THEN
        INSERT ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
        VALUES (Source.[IdRol], Source.[IdPantalla], Source.[IdOpcion], 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)))
    WHEN MATCHED AND Target.[FechaEliminacion] IS NOT NULL THEN
        UPDATE SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [HoraActualizacion] = CAST(GETDATE() AS TIME(7)), [UsuarioActualizacion] = 'SYSTEM';

    -- Insertar/Actualizar permiso Crear
    MERGE [dbo].[RolPantallaOpcion] AS Target
    USING (SELECT @IdRolSuperAdmin AS IdRol, @CurrentPantallaId AS IdPantalla, @IdOpcionCrear AS IdOpcion) AS Source
    ON Target.[IdRol] = Source.[IdRol] AND Target.[IdPantalla] = Source.[IdPantalla] AND Target.[IdOpcion] = Source.[IdOpcion]
    WHEN NOT MATCHED THEN
        INSERT ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
        VALUES (Source.[IdRol], Source.[IdPantalla], Source.[IdOpcion], 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)))
    WHEN MATCHED AND Target.[FechaEliminacion] IS NOT NULL THEN
        UPDATE SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [HoraActualizacion] = CAST(GETDATE() AS TIME(7)), [UsuarioActualizacion] = 'SYSTEM';

    -- Insertar/Actualizar permiso Editar
    MERGE [dbo].[RolPantallaOpcion] AS Target
    USING (SELECT @IdRolSuperAdmin AS IdRol, @CurrentPantallaId AS IdPantalla, @IdOpcionEditar AS IdOpcion) AS Source
    ON Target.[IdRol] = Source.[IdRol] AND Target.[IdPantalla] = Source.[IdPantalla] AND Target.[IdOpcion] = Source.[IdOpcion]
    WHEN NOT MATCHED THEN
        INSERT ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
        VALUES (Source.[IdRol], Source.[IdPantalla], Source.[IdOpcion], 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)))
    WHEN MATCHED AND Target.[FechaEliminacion] IS NOT NULL THEN
        UPDATE SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [HoraActualizacion] = CAST(GETDATE() AS TIME(7)), [UsuarioActualizacion] = 'SYSTEM';

    -- Insertar/Actualizar permiso Eliminar
    MERGE [dbo].[RolPantallaOpcion] AS Target
    USING (SELECT @IdRolSuperAdmin AS IdRol, @CurrentPantallaId AS IdPantalla, @IdOpcionEliminar AS IdOpcion) AS Source
    ON Target.[IdRol] = Source.[IdRol] AND Target.[IdPantalla] = Source.[IdPantalla] AND Target.[IdOpcion] = Source.[IdOpcion]
    WHEN NOT MATCHED THEN
        INSERT ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
        VALUES (Source.[IdRol], Source.[IdPantalla], Source.[IdOpcion], 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)))
    WHEN MATCHED AND Target.[FechaEliminacion] IS NOT NULL THEN
        UPDATE SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [HoraActualizacion] = CAST(GETDATE() AS TIME(7)), [UsuarioActualizacion] = 'SYSTEM';

    FETCH NEXT FROM Pantallas_SuperAdmin_Cursor INTO @CurrentPantallaId;
END;

CLOSE Pantallas_SuperAdmin_Cursor;
DEALLOCATE Pantallas_SuperAdmin_Cursor;

PRINT '      Permisos SuperAdmin configurados.';

-- =============================================
-- 6.2. ROL 2: Admin
-- =============================================
PRINT '   Configurando permisos para Admin...';

-- Sistema: Usuarios L/G/M/E, Empleados L/G/M/E
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaUsuarios, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaUsuarios, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaUsuarios, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaUsuarios, @IdOpcionEliminar;

EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaEmpleados, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaEmpleados, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaEmpleados, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaEmpleados, @IdOpcionEliminar;

-- Sistema: Roles L, Pantallas L, Permisos L
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaRoles, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaPantallas, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaRolPantallaOpcion, @IdOpcionVer;

-- Admin (Operación): Aeropuertos, Aerolineas, Aviones, Mantenimientos, Vuelos, Horarios, Escalas: L/G/M/E
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaAeropuertos, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaAeropuertos, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaAeropuertos, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaAeropuertos, @IdOpcionEliminar;

EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaAerolineas, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaAerolineas, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaAerolineas, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaAerolineas, @IdOpcionEliminar;

EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaAviones, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaAviones, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaAviones, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaAviones, @IdOpcionEliminar;

EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaMantenimientos, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaMantenimientos, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaMantenimientos, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaMantenimientos, @IdOpcionEliminar;

EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaVuelos, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaVuelos, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaVuelos, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaVuelos, @IdOpcionEliminar;

EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaHorarios, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaHorarios, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaHorarios, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaHorarios, @IdOpcionEliminar;

EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaEscalas, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaEscalas, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaEscalas, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaEscalas, @IdOpcionEliminar;

-- Atención: Pasajeros, Reservas, Boletos, Servicios, Equipaje: L/G/M/E
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaPasajeros, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaPasajeros, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaPasajeros, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaPasajeros, @IdOpcionEliminar;

EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaReservas, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaReservas, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaReservas, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaReservas, @IdOpcionEliminar;

EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaBoletos, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaBoletos, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaBoletos, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaBoletos, @IdOpcionEliminar;

EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaServicios, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaServicios, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaServicios, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaServicios, @IdOpcionEliminar;

EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaEquipaje, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaEquipaje, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaEquipaje, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaEquipaje, @IdOpcionEliminar;

-- Finanzas: Facturacion L/G/M/E
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaFacturacion, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaFacturacion, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaFacturacion, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaFacturacion, @IdOpcionEliminar;

-- Auditoría: Historiales L/G/M/E
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaHistoriales, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaHistoriales, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaHistoriales, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolAdmin, @IdPantallaHistoriales, @IdOpcionEliminar;

PRINT '      Permisos Admin configurados.';

-- =============================================
-- 6.3. ROL 3: Recepcion
-- =============================================
PRINT '   Configurando permisos para Recepcion...';

-- Atención: Pasajeros L/G/M, Reservas L/G/M, Boletos L/G/M, Servicios L/G/M, Equipaje L
EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaPasajeros, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaPasajeros, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaPasajeros, @IdOpcionEditar;

EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaReservas, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaReservas, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaReservas, @IdOpcionEditar;

EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaBoletos, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaBoletos, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaBoletos, @IdOpcionEditar;

EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaServicios, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaServicios, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaServicios, @IdOpcionEditar;

EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaEquipaje, @IdOpcionVer;

-- Admin: Aeropuertos L, Aerolineas L, Vuelos L, Horarios L, Escalas L (sin Aviones ni Mantenimientos)
EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaAeropuertos, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaAerolineas, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaVuelos, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaHorarios, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaEscalas, @IdOpcionVer;

-- Finanzas: Facturacion L
EXEC #InsertarPermiso @IdRolRecepcion, @IdPantallaFacturacion, @IdOpcionVer;

-- Sistema / Auditoría: Sin acceso (no se insertan permisos para estas pantallas)

PRINT '      Permisos Recepcion configurados.';

-- =============================================
-- 6.4. ROL 4: Auditoria - TODAS LAS PANTALLAS: L ÚNICAMENTE
-- =============================================
PRINT '   Configurando permisos para Auditoria...';

DECLARE Pantallas_Auditoria_Cursor CURSOR FOR
SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [FechaEliminacion] IS NULL;

OPEN Pantallas_Auditoria_Cursor;
FETCH NEXT FROM Pantallas_Auditoria_Cursor INTO @CurrentPantallaId;

WHILE @@FETCH_STATUS = 0
BEGIN
    EXEC #InsertarPermiso @IdRolAuditoria, @CurrentPantallaId, @IdOpcionVer;
    FETCH NEXT FROM Pantallas_Auditoria_Cursor INTO @CurrentPantallaId;
END;

CLOSE Pantallas_Auditoria_Cursor;
DEALLOCATE Pantallas_Auditoria_Cursor;

PRINT '      Permisos Auditoria configurados.';

-- =============================================
-- 6.5. ROL 5: Usuario
-- =============================================
PRINT '   Configurando permisos para Usuario...';

-- Boletos: G (Crear)
EXEC #InsertarPermiso @IdRolUsuario, @IdPantallaBoletos, @IdOpcionCrear;

-- Usuarios: M (Editar) - para modificar su propio usuario
EXEC #InsertarPermiso @IdRolUsuario, @IdPantallaUsuarios, @IdOpcionEditar;

-- Resto de pantallas/acciones: Sin acceso (no se insertan)

-- Si el formulario de "Guardar Boleto" requiere catálogos, descomentar las siguientes líneas:
-- EXEC #InsertarPermiso @IdRolUsuario, @IdPantallaVuelos, @IdOpcionVer;
-- EXEC #InsertarPermiso @IdRolUsuario, @IdPantallaAerolineas, @IdOpcionVer;
-- EXEC #InsertarPermiso @IdRolUsuario, @IdPantallaAeropuertos, @IdOpcionVer;

PRINT '      Permisos Usuario configurados.';

-- =============================================
-- 6.6. ROL 6: Mantenimiento
-- =============================================
PRINT '   Configurando permisos para Mantenimiento...';

-- Admin: Aviones L/G/M/E, Mantenimientos L/G/M/E
EXEC #InsertarPermiso @IdRolMantenimiento, @IdPantallaAviones, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolMantenimiento, @IdPantallaAviones, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolMantenimiento, @IdPantallaAviones, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolMantenimiento, @IdPantallaAviones, @IdOpcionEliminar;

EXEC #InsertarPermiso @IdRolMantenimiento, @IdPantallaMantenimientos, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolMantenimiento, @IdPantallaMantenimientos, @IdOpcionCrear;
EXEC #InsertarPermiso @IdRolMantenimiento, @IdPantallaMantenimientos, @IdOpcionEditar;
EXEC #InsertarPermiso @IdRolMantenimiento, @IdPantallaMantenimientos, @IdOpcionEliminar;

-- Admin: Vuelos L, Horarios L, Escalas L
EXEC #InsertarPermiso @IdRolMantenimiento, @IdPantallaVuelos, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolMantenimiento, @IdPantallaHorarios, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolMantenimiento, @IdPantallaEscalas, @IdOpcionVer;

-- Atención: Equipaje L
EXEC #InsertarPermiso @IdRolMantenimiento, @IdPantallaEquipaje, @IdOpcionVer;

-- Sistema / Finanzas / Auditoría: Sin acceso (no se insertan)

PRINT '      Permisos Mantenimiento configurados.';

-- =============================================
-- 6.7. ROL 7: Consulta
-- =============================================
PRINT '   Configurando permisos para Consulta...';

-- Solo lectura (L) en: Vuelos, Horarios, Aerolineas, Aeropuertos, Escalas, Pasajeros
EXEC #InsertarPermiso @IdRolConsulta, @IdPantallaVuelos, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolConsulta, @IdPantallaHorarios, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolConsulta, @IdPantallaAerolineas, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolConsulta, @IdPantallaAeropuertos, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolConsulta, @IdPantallaEscalas, @IdOpcionVer;
EXEC #InsertarPermiso @IdRolConsulta, @IdPantallaPasajeros, @IdOpcionVer;

-- Resto: Sin acceso (no se insertan)

PRINT '      Permisos Consulta configurados.';

-- =============================================
-- LIMPIAR (No hay procedimiento temporal que eliminar)
-- =============================================

-- =============================================
-- FINALIZAR TRANSACCIÓN
-- =============================================

COMMIT TRANSACTION;

PRINT '============================================';
PRINT 'SCRIPT EJECUTADO EXITOSAMENTE';
PRINT '============================================';
PRINT 'Roles creados: 7 (SuperAdmin, Admin, Recepcion, Auditoria, Usuario, Mantenimiento, Consulta)';
PRINT 'Pantallas creadas/actualizadas: 19';
PRINT 'Permisos configurados correctamente para cada rol';
PRINT '============================================';

GO

